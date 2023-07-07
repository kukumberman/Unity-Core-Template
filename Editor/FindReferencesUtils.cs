using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UnityEditor
{
    public class FindReferencesUtils
    {
        public static References[] Find(UnityEngine.Object[] values)
        {
            var result = new List<References>();

            var index = 0;
            var count = values.Count(v => v != null);
            foreach (var value in values)
            {
                if (value != null)
                {
                    var find = FindReferencesEditorWindow.Find(values, index, count);
                    if (find.Canceled)
                    {
                        break;
                    }
                    result.Add(new References
                    {
                        value = value,
                        references = find.ToArray()
                    });
                    index++;
                }
            }

            return result.ToArray();
        }

        public class References
        {
            public UnityEngine.Object value;
            public string[] references;
        }

        public class Result
        {
            public readonly HashSet<string> Files = new HashSet<string>();

            public int Count { get { return Files.Count; } }
            public bool Canceled { get; set; }

            public string[] ToArray()
            {
                return Files.ToArray();
            }
        }
    }
    public class FindReferencesEditorWindow : EditorWindow
    {
        private sealed class ThreadData
        {
            public int From;
            public int To;
            public string[] Guids;
            public List<string> Files;

            public int Index;
            public bool Completed;
            public FindReferencesUtils.Result Result;
        }

        [MenuItem("Tools/Parse sound names")]
        private static void ParseSoundNames()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in Selection.objects)
            {
                var name = item.name;

                string renamedName = name.Replace(" ", "_");
                for (int i = 0; i < 255; i++)
                {
                    var symbol = (char)i;

                    if (!char.IsLower(symbol))
                        continue;

                    renamedName = renamedName.Replace("_" + symbol, "_" + char.ToUpper(symbol));
                }

                if (char.IsLower(renamedName[0]))
                {
                    renamedName = renamedName.Insert(0, char.ToUpper(renamedName[0]).ToString());
                    renamedName = renamedName.Remove(1, 1);
                }

                if (!name.Equals(renamedName))
                {
                    var path = Application.dataPath + AssetDatabase.GetAssetPath(item);
                    path = path.Replace("AssetsAssets", "Assets");

                    File.Move(path, path.Replace(name, renamedName));
                }

                sb.AppendFormat("public const string {0} = \"{1}\";\n",
                    item.name.Replace("_", string.Empty).Replace(" ", string.Empty), item.name.Replace(" ", "_"));
            }
            Debug.Log(sb.ToString());
        }

        [MenuItem("Tools/Clear References In Project")]
        private static void ClearReferences()
        {
            _allFilesData.Clear();
        }

        [MenuItem("Assets/Find References In Project")]
        private static void FindReferences()
        {
            var result = Find(Selection.objects);
            if (!result.Canceled)
            {
                GetWindow<FindReferencesEditorWindow>("Result").Set(result).Show(true);
            }
        }

        [MenuItem("Assets/Find References In Project", true)]
        private static bool FindReferencesValidate()
        {
            return Selection.objects.Length >= 1;
        }

        private readonly static Dictionary<string, string> _allFilesData = new Dictionary<string, string>();
        private static readonly HashSet<string> _includeExtensions;

        static FindReferencesEditorWindow()
        {
            var include = new[] { ".prefab", ".mat", ".shader", ".asset", ".renderTexture", ".anim", ".spriteatlas", ".bundle", ".unity" };

            _includeExtensions = new HashSet<string>();
            foreach (var extension in include)
            {
                _includeExtensions.Add(extension);
            }
        }

        private static string displayedFile;
        private static bool canceled;

        public static FindReferencesUtils.Result Find(UnityEngine.Object[] values, int fileIndex = 1, int fileCount = 1)
        {
            var result = new FindReferencesUtils.Result();

            string[] guids = new string[values.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(values[i]);
                guids[i] = AssetDatabase.AssetPathToGUID(path);
            }

            EditorUtility.DisplayCancelableProgressBar("FIND", string.Format("[{0}/{1}]", fileIndex, fileCount), 0f);
            var files = AssetDatabase.GetAllAssetPaths().Where(IsValidFileToFind).ToList();
            int count = files.Count;
            int index = 0;
            canceled = false;
            displayedFile = string.Empty;

            int step = count / 4;

            var data = new ThreadData() { Files = files, Guids = guids, Result = result, From = 0, To = step * 1 };
            var data1 = new ThreadData() { Files = files, Guids = guids, Result = result, From = step * 1, To = step * 2 };
            var data2 = new ThreadData() { Files = files, Guids = guids, Result = result, From = step * 2, To = step * 3 };
            var data3 = new ThreadData() { Files = files, Guids = guids, Result = result, From = step * 3, To = count };
            ThreadPool.QueueUserWorkItem(ThreadProc, data);
            ThreadPool.QueueUserWorkItem(ThreadProc, data1);
            ThreadPool.QueueUserWorkItem(ThreadProc, data2);
            ThreadPool.QueueUserWorkItem(ThreadProc, data3);

            while (!canceled && !data.Completed && !data1.Completed && !data2.Completed && !data3.Completed)
            {
                index = data.Index + data1.Index + data2.Index + data3.Index;

                if (EditorUtility.DisplayCancelableProgressBar("FIND",
                string.Format("[{0}/{1}], [{2}/{3}] {4}", fileIndex, fileCount, index, count, displayedFile), index / (float)count))
                {
                    canceled = true;
                    break;
                }
            }

            if (!canceled)
            {
                files = new List<string>(result.Files);

                foreach (var file in files)
                {
                    if (IsMetaFile(file))
                    {
                        var filePath = AssetDatabase.GetAssetPathFromTextMetaFilePath(file);
                        data.Files.Remove(file);
                        data.Files.Add(filePath);
                    }
                }
            }

            result.Canceled = canceled;
            EditorUtility.ClearProgressBar();
            return result;
        }

        private static void ThreadProc(object state)
        {
            ThreadData data = (ThreadData)state;
            var files = data.Files;
            var guids = data.Guids;
            string file;

            for (int j = data.From; j < data.To; j++)
            {
                file = files[j];
                displayedFile = file;

                try
                {
                    if (!_allFilesData.ContainsKey(file))
                    {
                        _allFilesData.Add(file, File.ReadAllText(Path.GetFullPath(file)));
                    }

                    var textAsset = _allFilesData[file];

                    if (!string.IsNullOrEmpty(textAsset))
                    {
                        for (int i = 0; i < guids.Length; i++)
                        {
                            if (textAsset.Contains(guids[i]))
                            {
                                data.Result.Files.Add(file);
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                data.Index++;

                if (canceled)
                {
                    break;
                }
            }

            data.Completed = true;
        }

        private static bool IsMetaFile(string path)
        {
            return Path.GetExtension(path) == ".meta" &&
                   !string.IsNullOrEmpty(AssetDatabase.GetTextMetaFilePathFromAssetPath(path));
        }

        private static bool IsValidFileToFind(string path)
        {
            return _includeExtensions.Contains(Path.GetExtension(path));
        }

        private FindReferencesUtils.Result _result;
        private UnityEngine.Object[] _objects;
        private Vector2 _scroll;

        public FindReferencesEditorWindow Set(FindReferencesUtils.Result result)
        {
            _result = result;
            _objects = null;
            return this;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            var text = "<b><size=11>" + "RESULTS" + "</size></b>";
            GUILayout.Toggle(true, "☼ " + text, "dragtab", GUILayout.MinWidth(20f));
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);

            if (_result == null || _result.Count == 0)
            {
                GUILayout.Label("NO RESULT");
            }
            else
            {
                if (_objects == null)
                {
                    EditorUtility.DisplayProgressBar("WAIT", "PARSE RESOURCES", 0f);
                    try
                    {
                        var temp = new List<UnityEngine.Object>();
                        var index = 0;
                        var count = _result.Count;
                        foreach (var file in _result.ToArray())
                        {
                            temp.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file));
                            index++;
                            EditorUtility.DisplayProgressBar("WAIT", string.Format("PARSE RESOURCES [{0}/{1}]", index, count), index / (float)count);
                        }
                        _objects = temp.ToArray();
                    }
                    catch (Exception exeption)
                    {
                        EditorUtility.DisplayDialog("EXCEPTION", exeption.Message, "CLOSE");
                    }
                    EditorUtility.ClearProgressBar();
                }
                _scroll = EditorGUILayout.BeginScrollView(_scroll);
                foreach (var obj in _objects)
                {
                    EditorGUILayout.ObjectField(obj, typeof(UnityEngine.Object), false);
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}


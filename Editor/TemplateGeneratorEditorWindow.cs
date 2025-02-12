using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class TemplateGeneratorEditorWindow : EditorWindow
{
    private static readonly string kPackageDirectory =
        "Packages/com.kukumberman.unity-core-template";

    private static readonly string kRootNamespace = "_RootNamespace_";
    private static readonly string kClassName = "_Project_";

    private static readonly string Text_CopyMetaFiles = "Copy .meta files";
    private static readonly string Text_PrefixRootNamespace = string.Format(
        "Root namespace prefix ({0})",
        kRootNamespace
    );
    private static readonly string Text_PrefixClassName = string.Format(
        "Class name prefix ({0})",
        kClassName
    );

    private bool _copyMetaFiles = false;
    private string _prefixRootNamespace = "MyProject";
    private string _prefixClassName = "Project";

    [MenuItem("Tools/TemplateGenerator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<TemplateGeneratorEditorWindow>(false, "Template Generator");
    }

    private void OnGUI()
    {
        _copyMetaFiles = GUILayout.Toggle(_copyMetaFiles, Text_CopyMetaFiles);

        var cachedLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 300;

        EditorGUILayout.PrefixLabel(Text_PrefixRootNamespace);
        _prefixRootNamespace = GUILayout.TextField(_prefixRootNamespace);

        EditorGUILayout.PrefixLabel(Text_PrefixClassName);
        _prefixClassName = GUILayout.TextField(_prefixClassName);

        EditorGUIUtility.labelWidth = cachedLabelWidth;

        if (GUILayout.Button("Generate files"))
        {
            GenerateFiles();
        }
    }

    private void GenerateFiles()
    {
        var packageDirectory = Path.GetFullPath(kPackageDirectory);
        var templateDirectory = Path.Combine(packageDirectory, "Runtime", "ProjectTemplate");

        var files = GetFiles(templateDirectory);

        if (!_copyMetaFiles)
        {
            files = files.Where(x => Path.GetExtension(x) != ".meta");
        }

        var fileArray = files
            .Select(x => new MyFile
            {
                AbsolutePath = x,
                RelativePath = x.Substring(templateDirectory.Length + 1)
            })
            .ToArray();

        var outputFolder = EditorUtility.SaveFolderPanel("title", "", "defaultName");

        if (!string.IsNullOrEmpty(outputFolder))
        {
            CreateFiles(fileArray, outputFolder);

            Debug.Log("Done!");
        }
    }

    private void CreateFiles(MyFile[] files, string outputFolder)
    {
        foreach (var file in files)
        {
            CreateFile(file, outputFolder);
        }
    }

    private void CreateFile(MyFile file, string outputFolder)
    {
        var contents = File.ReadAllText(file.AbsolutePath);

        var sb = new StringBuilder(contents);
        sb.Replace(kRootNamespace, _prefixRootNamespace);
        sb.Replace(kClassName, _prefixClassName);

        var text = sb.ToString();

        var relativePath = file.RelativePath.Replace(kClassName, _prefixClassName);
        var destinationPath = Path.Combine(outputFolder, relativePath);

        var dir = Path.GetDirectoryName(destinationPath);

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(destinationPath, text);
    }

    private static IEnumerable<string> GetFiles(string path)
    {
        var queue = new Queue<string>();

        queue.Enqueue(path);

        while (queue.TryDequeue(out var item))
        {
            var directories = Directory.GetDirectories(item);

            foreach (var dir in directories)
            {
                queue.Enqueue(dir);
            }

            var files = Directory.GetFiles(item);

            foreach (var file in files)
            {
                yield return file;
            }
        }
    }

    private class MyFile
    {
        public string AbsolutePath;
        public string RelativePath;
    }
}

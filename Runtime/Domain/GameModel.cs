using System;
using Core;
using Game.Core;
using Game.Config;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Domain
{
    public abstract class GameModel : Observable
    {
        private const string SAVE_KEY = "save.json";

        public static ISaveSystem CurrentSaveSystem = null;
        public static ISaveEncoderDecoder CurrentSaveEncoderDecoder = null;

        protected abstract void PopulateDefaultModel(GameConfig config);

        public static T Load<T>(GameConfig config) where T : GameModel
        {
            T result = Activator.CreateInstance<T>();

            try
            {
                var data = CurrentSaveSystem.GetString(SAVE_KEY);
                if (CurrentSaveEncoderDecoder != null)
                {
                    data = CurrentSaveEncoderDecoder.Decode(data);
                }
                if (string.IsNullOrEmpty(data))
                {
                    result.PopulateDefaultModel(config);

                    return result;
                }

                result = JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception e)
            {
                result.PopulateDefaultModel(config);

                Logger.LogException(e);
            }

            return result;
        }

        public void Save()
        {
            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            try
            {
                var data = JsonConvert.SerializeObject(this, settings);
                if (CurrentSaveEncoderDecoder != null)
                {
                    data = CurrentSaveEncoderDecoder.Encode(data);
                }
                CurrentSaveSystem.SetString(SAVE_KEY, data);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        public static void Reset()
        {
            CurrentSaveSystem.Remove(SAVE_KEY);
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Remove save")]
#endif
        private static void Remove()
        {
            var saveSystems = new ISaveSystem[]
            {
                new FileSaveSystem(),
                new PlayerPrefsSaveSystem()
            };

            foreach (var system in saveSystems)
            {
                system.Remove(SAVE_KEY);
            }

            PlayerPrefs.DeleteAll();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Show save file")]
        private static void Show()
        {
            string arguments;
            var path = FileSaveSystem.GetSavePath(SAVE_KEY).Replace("/", @"\");
            if (!System.IO.File.Exists(path))
            {
                path = System.IO.Path.GetDirectoryName(path);
                arguments = $"\"{path}\"";
            }
            else
            {
                arguments = $"/select,\"{path}\"";
            }

            var info = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = arguments,
            };

            var process = System.Diagnostics.Process.Start(info);
            process.WaitForExit();
            process.Close();
        }
#endif
    }
}

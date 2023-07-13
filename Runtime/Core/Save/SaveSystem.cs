using System.IO;
using UnityEngine;

namespace Game.Core
{
    public interface ISaveEncoderDecoder
    {
        string Encode(string data);

        string Decode(string data);
    }

    public interface ISaveSystem
    {
        string GetString(string key);

        void SetString(string key, string value);

        bool Remove(string key);
    }

    public sealed class PlayerPrefsSaveSystem : ISaveSystem
    {
        public string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public bool Remove(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();

            return true;
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }
    }

    public sealed class FileSaveSystem : ISaveSystem
    {
        public string GetString(string key)
        {
            var path = GetSavePath(key);

            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return null;
        }

        public bool Remove(string key)
        {
            var path = GetSavePath(key);

            if (File.Exists(path))
            {
                File.Delete(path);

                return true;
            }

            return false;
        }

        public void SetString(string key, string value)
        {
            var path = GetSavePath(key);
            File.WriteAllText(path, value);
        }

        public static string GetSavePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    }
}

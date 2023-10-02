namespace Game.Core
{
    public interface ISaveSystem
    {
        string GetString(string key);

        void SetString(string key, string value);

        bool Remove(string key);
    }
}

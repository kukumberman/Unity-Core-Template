using UnityEngine;

namespace Game.Managers
{
    public class SpriteManager
    {
        public Sprite GetSprite(string name)
        {
            var result = Resources.Load<Sprite>(string.Format("Sprites/{0}", name));

            if (null == result)
            {
                Logger.LogWarning("Can't load sprite " + name);
            }
            return result;
        }
    }
}

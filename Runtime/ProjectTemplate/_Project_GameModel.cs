using Game.Config;
using Game.Domain;
using UnityEngine;

namespace _RootNamespace_
{
    public sealed class _Project_GameModel : GameModel
    {
        public int Foobar;

        protected override void PopulateDefaultModel(GameConfig baseConfig)
        {
            if (baseConfig is not _Project_GameConfig config)
            {
                Debug.LogWarning(
                    $"[{nameof(_Project_GameModel)}] requires [{nameof(GameConfig)}] of type [{nameof(_Project_GameConfig)}]"
                );

                return;
            }

            Foobar = config.Foobar;
        }
    }
}

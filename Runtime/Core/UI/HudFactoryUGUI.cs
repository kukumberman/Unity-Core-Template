using System;
using Game.Enums;
using Game.UI.Hud;
using UnityEngine;

namespace Game.Core.UI
{
    public sealed class HudFactoryUGUI : IHudFactory
    {
        private const string kHudsPath = "Huds/{0}";

        private readonly Canvas _canvas;

        public HudOrientation Orientation { get; set; }

        public HudFactoryUGUI(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void Create(Mediator hud)
        {
            var suffix = Orientation == HudOrientation.Default ? string.Empty : $"_{Orientation}";
            var fileName = hud.GetType().Name.Replace("HudMediator", "") + suffix;
            var hudPrefab = Resources.Load<BaseHud>(string.Format(kHudsPath, fileName));

            if (null == hudPrefab)
            {
                throw new Exception("Not found " + fileName);
            }
            var parent = _canvas.transform;
            var hudView = GameObject.Instantiate(hudPrefab, parent);

            Sort(parent);

            hud.Mediate(hudView);
            hud.InternalShow();
        }

        private void Sort(Transform parent)
        {
            var existingHuds = parent.GetComponentsInChildren<BaseHud>();

            if (existingHuds.Length != parent.childCount)
            {
                Debug.LogWarning("<b>HudFactoryUGUI.Sort</b> hierarchy mismatch", parent);
            }

            Array.Sort(existingHuds, (lhs, rhs) => lhs.HierarchyOrder - rhs.HierarchyOrder);

            for (int i = 0; i < existingHuds.Length; i++)
            {
                existingHuds[i].transform.SetSiblingIndex(i);
            }
        }
    }
}

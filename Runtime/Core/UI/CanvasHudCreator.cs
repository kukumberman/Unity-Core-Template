using System;
using Game.UI.Hud;
using UnityEngine;

namespace Game.Core.UI
{
    public sealed class CanvasHudCreator : IHudCreator
    {
        private const string kHudsPath = "Huds/{0}";

        private readonly Canvas _canvas;

        public CanvasHudCreator(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void Create(Mediator hud)
        {
            var fileName = hud.GetType().Name.Replace("HudMediator", "");
            var hudPrefab = Resources.Load<BaseHud>(string.Format(kHudsPath, fileName));

            if (null == hudPrefab)
            {
                throw new Exception("Not found " + fileName);
            }
            var parent = _canvas.transform;
            var hudView = GameObject.Instantiate(hudPrefab, parent);
            hud.Mediate(hudView);
            hud.InternalShow();

            //test Dima: sort others huds
            if (hud.HierarchyOrder == -1)
            {
                hudView.transform.SetAsLastSibling();
            }
            else
            {
                hudView.transform.SetSiblingIndex(hud.HierarchyOrder);
            }
        }
    }
}

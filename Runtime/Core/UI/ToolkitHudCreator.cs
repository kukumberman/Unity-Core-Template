using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Core.UI
{
    public class ToolkitHudCreator : IHudCreator
    {
        private const string kHudsPath = "Huds/{0}";

        private readonly UIDocument _document;

        public ToolkitHudCreator(UIDocument document)
        {
            _document = document;
        }

        public void Create(Mediator hud)
        {
            var fileName = hud.GetType().Name.Replace("HudMediator", "");
            var treeAsset = Resources.Load<VisualTreeAsset>(string.Format(kHudsPath, fileName));

            if (treeAsset == null)
            {
                throw new Exception("Not found " + fileName);
            }

            var root = _document.rootVisualElement.Q<VisualElement>("root");
            var view = treeAsset.Instantiate().Children().ElementAt(0);
            root.Add(view);

            hud.Mediate(view);
            hud.InternalShow();
        }
    }
}

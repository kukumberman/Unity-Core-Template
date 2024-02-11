using Game.Enums;
using Game.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Core.UI
{
    public class HudFactoryUITK : IHudFactory
    {
        private const string kHudsPath = "Huds/{0}";

        private readonly UIDocument _document;

        public HudOrientation Orientation { get; set; }

        private Dictionary<Type, Func<VisualElement>> _factoryMap = new();

        public HudFactoryUITK(UIDocument document)
        {
            _document = document;
        }

        public void Create(Mediator hud)
        {
            var suffix = Orientation == HudOrientation.Default ? string.Empty : $"_{Orientation}";
            var fileName = hud.GetType().Name.Replace("HudMediator", "") + suffix;
            var treeAsset = Resources.Load<VisualTreeAsset>(string.Format(kHudsPath, fileName));

            if (treeAsset == null)
            {
                throw new Exception("Not found " + fileName);
            }

            var root = _document.rootVisualElement.Q<VisualElement>("root");
            var templateView = treeAsset.Instantiate()[0];

            var view = _factoryMap[hud.ViewType]();
            view.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            view.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            view.style.position = new StyleEnum<Position>(Position.Absolute);
            view.pickingMode = PickingMode.Ignore;

            foreach (var styleSheet in treeAsset.stylesheets)
            {
                view.styleSheets.Add(styleSheet);
            }

            view.Add(templateView);

            var methodInfo = view.GetType()
                .GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(view, null);
            root.Add(view);

            Sort(root);

            hud.Mediate(view);
            hud.InternalShow();
        }

        public void Bind(Type type, Func<VisualElement> element)
        {
            if (!_factoryMap.ContainsKey(type))
            {
                _factoryMap.Add(type, element);
            }
        }

        private void Sort(VisualElement parent)
        {
            parent.Sort(Comparison);
        }

        private static int Comparison(VisualElement lhs, VisualElement rhs)
        {
            var lhsHud = lhs as BaseHudUITK;
            var rhsHud = rhs as BaseHudUITK;

            return lhsHud.HierarchyOrder - rhsHud.HierarchyOrder;
        }
    }
}

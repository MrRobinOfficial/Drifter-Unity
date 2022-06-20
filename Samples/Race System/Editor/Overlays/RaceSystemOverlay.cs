using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Drifter.Samples.RaceSystem.Editor
{
    [Overlay(typeof(SceneView), displayName: "Race System [Utility]", id = k_Id)]
    [Icon(path: "Assets/Drifter/Samples/Race System/Resources/Textures/Icons/flag.png")]
    public class RaceSystemOverlay : Overlay
    {
        private const string k_Id = "raceSystem-utility-overlay";

        private VisualElement root;

        public override VisualElement CreatePanelContent()
        {
            root = new VisualElement();

            root.Add(DrawDefault());

            return root;
        }

        private VisualElement DrawDefault()
        {
            var defaultRoot = new VisualElement();

            var showAllCheckpoints = new Toggle()
            {
                label = "Show All Checkpoints",
                value = false,
            };

            defaultRoot.Add(showAllCheckpoints);

            return defaultRoot;
        }
    }
}
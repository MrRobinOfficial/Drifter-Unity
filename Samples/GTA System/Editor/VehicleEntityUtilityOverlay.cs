using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Drifter.Samples.GTASystem.Editor
{
    [Overlay(typeof(SceneView), displayName: "Vehicle Entity [Utility]", id = k_Id)]
    [Icon("Assets/Drifter/Samples/GTA System/Resources/Textures/Icons/ID.png")]
    public class VehicleEntityUtilityOverlay : Overlay
    {
        private const string k_Id = "vehicleEntity-utility-overlay";

        private VisualElement root;

        public override VisualElement CreatePanelContent()
        {
            root = new VisualElement();

            Selection.selectionChanged += HandleSelection;
            HandleSelection();

            return root;
        }

        private void HandleSelection()
        {
            root.Clear();

            if (Selection.activeGameObject == null)
                return;

            var entity = Selection.activeGameObject.GetComponent<VehicleEntity>();

            if (entity != null)
                root.Add(DrawDefault(entity));
        }

        private VisualElement DrawDefault(VehicleEntity entity)
        {
            var defaultRoot = new VisualElement();

            var healthBar = new ProgressBar()
            {
                value = entity.GetHealthNormalized(),
                lowValue = 0f,
                highValue = 1f,
            };

            var destroyBtn = new Button(entity.Destroy)
            {
                text = "Destroy",
            };

            defaultRoot.Add(healthBar);
            defaultRoot.Add(destroyBtn);

            return defaultRoot;
        }
    }
}
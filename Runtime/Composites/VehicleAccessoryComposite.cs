using UnityEngine;

namespace Drifter.Composites
{
    [AddComponentMenu("Tools/Drifter/Composites/Vehicle Accessory [Composite]"), DisallowMultipleComponent]
    public class VehicleAccessoryComposite : MonoBehaviour
    {
        [ContextMenu("Open", isValidateFunction: false, priority: 1000150)]
        public void Open() { }

        [ContextMenu("Close", isValidateFunction: false, priority: 1000150)]
        public void Close() { }
    }
}
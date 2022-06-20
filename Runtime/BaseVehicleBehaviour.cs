using UnityEngine;

namespace Drifter
{
    public abstract class BaseVehicleBehaviour : MonoBehaviour
    {
        private BaseVehicle _vehicle = default;

        public BaseVehicle Vehicle
        {
            get
            {
                if (_vehicle == null)
                    _vehicle = transform.root.GetComponent<BaseVehicle>();

                return _vehicle;
            }
        }
    }
}
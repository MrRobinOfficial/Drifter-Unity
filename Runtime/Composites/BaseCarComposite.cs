using Drifter.Vehicles;
using UnityEngine;

namespace Drifter.Composites
{
    public abstract class BaseCarComposite : MonoBehaviour
    {
        protected CarVehicle carVehicle = default;

        protected virtual void Awake() => carVehicle = GetComponent<CarVehicle>();
    }
}
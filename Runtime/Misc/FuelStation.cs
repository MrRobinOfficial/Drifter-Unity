using Drifter.Vehicles;
using System.Collections;
using UnityEngine;

namespace Drifter.Misc
{
    [AddComponentMenu("Tools/Drifter/Misc/Fuel Station [Misc]"), DisallowMultipleComponent]
    public class FuelStation : MonoBehaviour, IInteractable
    {
        [SerializeField] float m_FuelPerFrame = 0.00016f;

        public string Prompt => $"Hold {KeyCode.F} to refuel";

        public void EnterInteract(Interactor interactor)
        {
            throw new System.NotImplementedException();
        }

        public void ExitInteract(Interactor interactor)
        {
            throw new System.NotImplementedException();
        }
    }
}
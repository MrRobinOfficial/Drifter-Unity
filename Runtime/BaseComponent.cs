using UnityEngine;
using UnityEngine.Events;

namespace Drifter
{
    public abstract class BaseComponent : IDatatable, IDamageable
    {
        public event UnityAction OnDestroyed;

        public int Health { get; protected set; }

        public float GetHealthNormalized() => 1f; // Need max health variable

        public bool IsAlive => Health > 0;

        public virtual void TakeDamage(Object sender, ushort damage)
        {
            if (!IsAlive)
                return;

            Health -= damage;

            if (Health <= 0)
            {
                Health = 0;
                OnDestroyed?.Invoke();
            }
        }

        public abstract void OnEnable(BaseVehicle vehicle);

        public abstract void OnDisable(BaseVehicle vehicle);

        #region Data Saving
        public abstract void Load(FileData data);

        public abstract FileData Save(); 
        #endregion
    }
}
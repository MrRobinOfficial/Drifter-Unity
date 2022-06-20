using UnityEngine;

namespace Drifter
{
    public interface IDamageable
    {
        public void TakeDamage(Object sender, ushort damage);
    } 
}
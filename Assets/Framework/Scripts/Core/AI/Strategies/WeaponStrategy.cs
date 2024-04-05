using Framework.Scripts.Core.Damage;
using Framework.Scripts.Core.Player;
using UnityEngine;
namespace Framework.Scripts.Core.AI.Strategies
{
    public abstract class WeaponStrategy : ScriptableObject
    {
        public float AttackRate = 5.0f; 
        public float Damage = 10.0f; 
        public float Force = 10.0f; 
        public float AttackRange = 5.0f; 
        public LayerMask DamageableLayer; 
        
        public virtual void Attack(StateContext Context, Transform AttackOrigin, Vector3? TargetPosition = null)
        {
        }
        
        public virtual void DrawGizmos(IEntity Entity) { }
    }
}
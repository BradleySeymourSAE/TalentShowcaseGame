using Framework.Scripts.Core.AI;
using Framework.Scripts.Core.AI.Strategies;
using Framework.Scripts.Core.Damage;
using Framework.Scripts.Core.Player;
using UnityEngine;

namespace Framework.Scripts.Core.Weapon
{
    public class WeaponComponent : MonoBehaviour, IWeapon, IDamageProvider
    {
        [field:SerializeField] public Transform WeaponAttackPoint { get; set; }
        public WeaponStrategy WeaponStrategy { get; protected set; } 
        public StateContext StateContext { get; protected set; }

        public virtual void Initialize(StateContext StateContext)
        {
            this.StateContext = StateContext;
            this.StateContext.SourceProvider = this; 
            this.StateContext.LastAttackTime = Time.time; 
        }

        public virtual void Attack()
        {
            WeaponStrategy.Attack(StateContext, WeaponAttackPoint);
        }

        public virtual bool CanAttack()
        {
            if (WeaponStrategy == null)
            {
                return false; 
            }
            return(Time.time - StateContext.LastAttackTime) > WeaponStrategy.AttackRate; 
        } 
        

        #region Interface Implementations 

        float IDamageProvider.GetDamage()
        {
            return WeaponStrategy.Damage; 
        } 
        
        float IDamageProvider.GetForce()
        {
            return WeaponStrategy.Force; 
        } 
        
        WeaponStrategy IWeapon.WeaponStrategy
        {
            get { return WeaponStrategy; }
        }

        void IWeapon.SetWeaponStrategy(WeaponStrategy Strategy)
        {
            this.WeaponStrategy = Strategy; 
        }
        #endregion 
    }
}
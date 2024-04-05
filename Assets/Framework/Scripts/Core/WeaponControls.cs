using Framework.Common;
using Framework.Scripts.Common.Injection;
using Framework.Scripts.Core.Damage;
using Framework.Scripts.Core.Player;
using UnityEngine;
namespace Framework.Scripts.Core
{
    public class WeaponControls : MonoBehaviour
    {
        [Injection, SerializeField] protected HeroController Character;
        public Transform ArmL;
        public Transform ArmR;
        
        
        private void OnMeleeAttack()
        {
            Character.Weapon.Attack();
            // console.log(this, "Melee Attack..."); 
            // Collider2D[] hits = Physics2D.OverlapCircleAll(Character.Weapon.WeaponAttackPoint.position, Character.Weapon.WeaponStrategy.AttackRange, Character.Weapon.WeaponStrategy.DamageableLayer);
            // foreach (Collider2D hit in hits)
            // {
            //     if (hit.TryGetComponent(out IDamageReceiver receiver) && hit.gameObject != Character.gameObject)
            //     {
            //         console.log(this, "OnMeleeAttack", "receiver", receiver.gameObject);
            //         DamageProcessor.ProcessDamage(Character.Weapon as IDamageProvider, receiver); 
            //     }  
            // }
        }
    } 
}
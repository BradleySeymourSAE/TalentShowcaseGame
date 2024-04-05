using Framework.Common;
using Framework.Scripts.Common;
using Framework.Scripts.Core.Damage;
using Framework.Scripts.Core.Player;
using UnityEngine;
namespace Framework.Scripts.Core.AI.Strategies
{
    [CreateAssetMenu(fileName = "MeleeWeaponStrategy", menuName = "Attack Strategies/Melee Weapon")]

    public class MeleeWeaponStrategy : WeaponStrategy
    {
        
        public override void Attack(StateContext Context, Transform AttackOrigin, Vector3? TargetPosition = null)
        {
            console.log(this, "ATTACKING WITH MELEE WEAPON:"); 
            Collider2D[] colliders = Physics2D.OverlapCircleAll(AttackOrigin.position, AttackRange, DamageableLayer); 
            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out IDamageReceiver receiver) && receiver.gameObject != Context.SourceProvider.gameObject)
                {
                    DamageProcessor.ProcessDamage(Context.SourceProvider, receiver); 
                }
            }
        } 
    }
}
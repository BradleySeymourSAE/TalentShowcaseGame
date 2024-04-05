using Framework.Scripts.Core.ObjectPool;
using Framework.Scripts.Core.Player;
using UnityEngine;
namespace Framework.Scripts.Core.AI.Strategies
{
    [CreateAssetMenu(fileName = "ProjectileAttackStrategy", menuName = "Attack Strategies/Projectile")]
    public class ProjectileAttackStrategy : WeaponStrategy
    {
        public ProjectileSettings ProjectileSettings; 
        public float Speed = 10f; 
        public float Duration = 1f; 
        

        public override void Attack(StateContext Context, Transform AttackOrigin, Vector3? TargetPosition = null)
        {
            FlyweightBehaviour instance = ObjectPoolFactory.Spawn(ProjectileSettings); 
            if (instance is ObjectPoolProjectile projectile)
            {
                projectile.transform.position = AttackOrigin.position; 
                projectile.transform.right = TargetPosition.HasValue ? (TargetPosition.Value - AttackOrigin.position).normalized : AttackOrigin.right;
            } 
        }
        
        public override void DrawGizmos(IEntity Entity)
        {
            if (Entity is AbstractAIEntity entity && entity.Weapon != null && entity.Weapon.WeaponAttackPoint)
            {
                // Draw a line going from the entity's attack origin to the target position 
                Gizmos.color = Color.red; 
                Gizmos.DrawLine(Entity.Weapon.WeaponAttackPoint.position, entity.TargetPosition + Vector3.up * 1.5f);
            }
        }
    }

}
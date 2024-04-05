using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using Framework.Scripts.Core.Damage;
using Framework.Scripts.Core.ObjectPool;
using Framework.Scripts.Core.Player;
using UnityEngine;
namespace Framework.Scripts.Core.AI.Strategies
{
    [CreateAssetMenu(fileName = "BombAttackStrategy", menuName = "Attack Strategies/Bomb Attack")]
    public class BombAttackStrategy : WeaponStrategy
    {
        public FlyweightSettings BombSettings;
        public int Count = 5;
        public float SpreadAngle = 45f;
        public float ThrowForce = 10f;
        public float Height = 5f;
        public float Speed = 1f;
        public float TimeBetweenBombs = 0.0f; 
        [Range(0.01f, 5.0f)] public float Length = 2.0f; 

        public override void Attack(StateContext Context, Transform AttackOrigin, Vector3? TargetPosition = null)
        {
            float step = SpreadAngle / (Count - 1);
            float startAngle = -SpreadAngle / 2;
            
            for (int i = 0; i < Count; i++)
            {
                FlyweightBehaviour instance = ObjectPoolFactory.Spawn(BombSettings); 
                instance.transform.position = AttackOrigin.position; 
                instance.transform.rotation = Quaternion.identity; 
            
                SpawnBombInSequence(AttackOrigin, instance, startAngle + (step * i)); 
                // DamageOnCollision bomb = Instantiate(BombPrefab, AttackOrigin.position, Quaternion.identity) as DamageOnCollision; 
                // MoveBomb(AttackOrigin, bomb.transform, startAngle + (step * i));
            }
        }
        
        private async void SpawnBombInSequence(Transform Origin, FlyweightBehaviour Bomb, float Angle)
        {
            await new WaitForSeconds(TimeBetweenBombs); 
            MoveBomb(Origin, Bomb, Angle); 
        }

        private async void MoveBomb(Transform Origin, FlyweightBehaviour bomb, float Angle)
        {
            Vector3 targetPosition = CalculateTargetPosition(Origin.position, Angle);
            Vector3 from = bomb.transform.position;
            Vector3 arcMid = (from + targetPosition) / 2 + Vector3.up * Height;
            for (float t = 0; t <= Length; t += (Time.deltaTime / Speed))
            {
                bomb.transform.position = Utility.CalculateBezierPoint(t, from, arcMid, targetPosition);
                await new WaitForUpdate(); 
            }
            ObjectPoolFactory.Despawn(bomb); 
        }

        private Vector3 CalculateTargetPosition(Vector3 Position, float Angle)
        {
            return Position + Quaternion.Euler(0, 0, Angle) * Vector3.up * ThrowForce;
        }

        public override void DrawGizmos(IEntity Entity)
        {
            Gizmos.color = Color.red;
            float angleStep = SpreadAngle / (Count - 1);
            float startingAngle = -SpreadAngle / 2;

            for (int i = 0; i < Count; i++)
            {
                Vector2 targetPosition = CalculateTargetPosition(Entity.transform.position,startingAngle + (angleStep * i));
                Vector2 from = Entity.transform.position;
                Vector2 arcMid = (from + targetPosition) / 2 + Vector2.up * Height;

                for (float t = 0; t <= Length; t += 0.1f)
                {
                    Vector3 drawPoint = Utility.CalculateBezierPoint(t, from, arcMid, targetPosition);
                    Gizmos.DrawSphere(drawPoint, 0.1f);
                }
            }
        }
    }
}
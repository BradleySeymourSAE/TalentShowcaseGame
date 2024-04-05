using Framework.Common;
using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using Framework.Scripts.Core.ObjectPool;
using Framework.Scripts.Core.Player;
using UnityEngine;
namespace Framework.Scripts.Core.AI.Strategies
{
    [CreateAssetMenu(fileName = "AxeThrowAttackStrategy", menuName = "Attack Strategies/Axe Throw")]
    public class AxeThrowAttackStrategy : WeaponStrategy
    {
        public FlyweightSettings AxeSettings;
        public float MinHeight = 5; 
        public float MaxHeight = 25;
        public float Duration = 1.5f;
        public float Length = 2.0f; 
        public float Speed = 1.0f; 
        public float SpinningRate = 720;
        public float DestroyDelay = 0.5f;

        public override void Attack(StateContext Context, Transform AttackOrigin, Vector3? TargetPosition = null)
        { 
            if (TargetPosition == null)
            {
                console.warn(this, "Target position is null");
                return;
            }
            FlyweightBehaviour instance = ObjectPoolFactory.Spawn(AxeSettings); 
            instance.transform.position = AttackOrigin.position; 
            instance.transform.rotation = Quaternion.identity;
            ThrowAxeTorwardsTarget(instance, TargetPosition.Value); 
        }

        private async void ThrowAxeTorwardsTarget(FlyweightBehaviour AxeInstance, Vector3 TargetPosition)
        {
            Vector3 from = AxeInstance.transform.position;
            float height = Random.Range(MinHeight, MaxHeight); 
            Vector3 arcMid = (from + TargetPosition) / 2 + Vector3.up * height;
            
            for (float t = 0; t <= Length; t += (Time.deltaTime / Speed))
            {
                AxeInstance.transform.position = Utility.CalculateBezierPoint(t, from, arcMid, TargetPosition);
                AxeInstance.transform.Rotate(Vector3.forward, SpinningRate * Time.deltaTime); 
                await new WaitForUpdate(); 
            }
            ObjectPoolFactory.DespawnAfterDelay(AxeInstance, DestroyDelay);
        }


        public override void DrawGizmos(IEntity Entity)
        {
            AbstractAIEntity entity = Entity as AbstractAIEntity; 
            if (entity == null || entity.ActiveTarget == null)
            {
                return;
            } 
            Gizmos.color = Color.red; 
            Vector3 from = Entity.transform.position;
            Vector3 to = ((AbstractAIEntity) Entity).ActiveTarget.transform.position; 
            from.z = 0; 
            to.z = 0; 
            Vector3 arcMid = (from + to) / 2; 
            arcMid.y += MaxHeight; 
            Vector3 controlPoint = arcMid + (Vector3.up * MaxHeight); 
            for (float t = 0; t <= 1; t += 0.1f)
            {
                Vector3 drawPoint = Mathf.Pow(1 - t, 2) * from + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * to;
                Gizmos.DrawSphere(drawPoint, 0.1f);
            } 
        }
    }
}
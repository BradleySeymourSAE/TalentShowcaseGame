using Framework.Common;
using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using Framework.Scripts.Core.Damage;
using UnityEngine;
namespace Framework.Scripts.Core.ObjectPool
{
    public class ObjectPoolProjectile : FlyweightBehaviour
    {
        protected bool m_Released = false;
        public new ProjectileSettings Settings
        {
            get => (ProjectileSettings) base.Settings;
            set => base.Settings = value;
        }

        protected virtual void OnEnable()
        {
            m_Released = false;
            Despawn(Settings.DespawnDelay);
        }

        protected virtual void Update()
        {
            transform.position += transform.right * (Settings.Speed * Time.deltaTime);

            // get all overlapping colliders 
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.0f);
            foreach (var collider in colliders)
            {
                if (collider.gameObject == gameObject)
                {
                    continue;
                }
                if (collider.gameObject.layer == Layers.Ground && !m_Released)
                {
                    console.log(this, "Hit ground", collider.gameObject.name);
                    m_Released = true;
                    ObjectPoolFactory.Despawn(this);
                }
            }
        }

        public void Despawn(float Delay)
        {
            DespawnWithDelay(Delay);
        }

        private async void DespawnWithDelay(float delay)
        {
            await new WaitForSeconds(delay);
            if (!m_Released)
            {
                m_Released = true;
                ObjectPoolFactory.Despawn(this);
            }
        }
    }
} 
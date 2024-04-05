using System.Collections.Generic;
using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using UnityEngine;
using UnityEngine.Pool;
namespace Framework.Scripts.Core.ObjectPool
{
    /// <summary>
    /// The ObjectPoolFactory class is responsible for spawning and despawning objects from object pools.
    /// </summary>
    public sealed class ObjectPoolFactory : Singleton<ObjectPoolFactory>
    {
        /// <summary>
        /// Whether to use collection checks when retrieving and releasing objects from the object pool.
        /// </summary> 
        [SerializeField] private bool m_UseCollectionChecks = true;

        ///  The default capacity of the object pool. 
        [SerializeField] private int m_DefaultCapacity = 10;

        /// <summary>
        /// The maximum size of the object pool.
        /// </summary>
        [SerializeField] private int m_MaximumPoolSize = 100;

        /// <summary>
        /// The dictionary that stores the object pools for each object pooling behaviour type.
        /// </summary>
        private readonly Dictionary<ObjectPoolingBehaviourType, IObjectPool<FlyweightBehaviour>> m_ObjectPools = new();

        /// <summary>
        ///    After the instance is enabled, it is marked as DontDestroyOnLoad. 
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            DontDestroyOnLoad(this.gameObject); 
        }

        /// <summary>
        /// Spawns a <see cref="FlyweightBehaviour"/> from the appropriate object pool based on the specified <see cref="FlyweightSettings"/>.
        /// </summary>
        /// <param name="Settings">The <see cref="FlyweightSettings"/> for the spawned object.</param>
        /// <returns>The spawned <see cref="FlyweightBehaviour"/>.</returns>
        public static FlyweightBehaviour Spawn(FlyweightSettings Settings) => Instance?.GetPoolFor(Settings).Get();

        /// <summary>
        /// Despawns a pooled object.
        /// </summary>
        /// <param name="Projectile">The pooled object to despawn.</param>
        public static void Despawn(FlyweightBehaviour Projectile) => Instance?.GetPoolFor(Projectile.Settings).Release(Projectile);

        /// <summary>
        /// Delays despawning of the given FlyweightBehaviour object after a specified delay.
        /// </summary>
        /// <param name="Projectile">The FlyweightBehaviour object to despawn.</param>
        /// <param name="Delay">The delay in seconds before despawning the object.</param>
        public static async void DespawnAfterDelay(FlyweightBehaviour Projectile, float Delay)
        {
            await new WaitForSeconds(Delay);
            Despawn(Projectile);
        } 
        /// <summary>
        /// Gets the object pool for the provided settings.
        /// </summary>
        /// <param name="Settings">The settings for the object pool.</param>
        /// <returns>The object pool for the provided settings.</returns>
        private IObjectPool<FlyweightBehaviour> GetPoolFor(FlyweightSettings Settings)
        {
            IObjectPool<FlyweightBehaviour> candidate; 
            if (m_ObjectPools.TryGetValue(Settings.Type, out candidate))
            {
                return candidate; 
            }
            candidate = new ObjectPool<FlyweightBehaviour>(
                Settings.Create,
                Settings.OnRetrieve,
                Settings.OnRelease,
                Settings.OnDestroyed,
                m_UseCollectionChecks,
                m_DefaultCapacity,
                m_MaximumPoolSize
            );
            m_ObjectPools.Add(Settings.Type, candidate);
            return candidate; 
        }
    }

}
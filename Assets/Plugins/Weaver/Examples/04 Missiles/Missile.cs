// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// A projectile that flies straight and creates an <see cref="Explosion"/> when it hits something or takes damage.
    /// </summary>
    public sealed class Missile : PoolableBehaviour<Missile>, ITakeDamage
    {
        /************************************************************************************************************************/

        [SerializeField]
        private Rigidbody _Rigidbody;

        /// <summary>The <see cref="Rigidbody"/> which moves this missile.</summary>
        public Rigidbody Rigidbody
        {
            get { return _Rigidbody; }
            set { _Rigidbody = value; }
        }

        [SerializeField]
        private Explosion _ExplosionPrefab;

        /// <summary>The <see cref="Explosion"/> prefab to instantiate on impact or when damaged.</summary>
        public Explosion ExplosionPrefab
        {
            get { return _ExplosionPrefab; }
            set { _ExplosionPrefab = value; }
        }

        [SerializeField]
        private float _SpeedMultiplier = 1;

        /// <summary>The movement speed specified in <see cref="Fire"/> is multiplied by this value.</summary>
        public float SpeedMultiplier
        {
            get { return _SpeedMultiplier; }
            set { _SpeedMultiplier = value; }
        }

        /************************************************************************************************************************/

        /// <summary>A pool made from the <see cref="_ExplosionPrefab"/> which instances will be acquired from.</summary>
        private ObjectPool<Explosion> _ExplosionPool;

        /************************************************************************************************************************/

        /// <summary>The owner of this missile.</summary>
        public IDealDamage Source { get; private set; }

        /// <summary>The speed at which this missile is moving.</summary>
        public float Speed { get; private set; }

        /// <summary>Indicates whether this missile will increase the player's score when destroyed.</summary>
        public bool IsWorthPoints { get; private set; }

        /************************************************************************************************************************/

        /// <summary>Initializes and activates this missile.</summary>
        public void Fire(IDealDamage source, Vector3 position, Quaternion rotation, float speed, bool isWorthPoints)
        {
            Source = source;
            transform.SetPositionAndRotation(position, rotation);
            Speed = speed * _SpeedMultiplier;
            IsWorthPoints = isWorthPoints;
            gameObject.SetActive(true);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when this component first becomes enabled and active.
        /// Gets the shared object pool for the <see cref="_ExplosionPrefab"/> prefab.
        /// </summary>
        private void Awake()
        {
            // Make the missile layer ignore itself so missiles don't hit each other.
            // Normally you would just set this in the physics settings,
            // but we don't want to actually change any project settings just for this example.
            Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);

            _ExplosionPool = ObjectPool.GetSharedComponentPool(_ExplosionPrefab);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per physics update by Unity.
        /// Keeps the <see cref="_Rigidbody"/> flying forward with the correct <see cref="Speed"/>.
        /// </summary>
        private void FixedUpdate()
        {
            _Rigidbody.velocity = transform.forward * Speed;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when this object collides with something.
        /// Causes this missile to <see cref="Explode"/>.
        /// </summary>
        public void OnCollisionEnter()
        {
            OnTakeDamage(Source, 1);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called when this object is damaged by something.
        /// Spawns an explosion and releases this missile back to its <see cref="ObjectPool{T}"/>.
        /// </summary>
        public void OnTakeDamage(IDealDamage source, int pointValue)
        {
            if (!gameObject.activeSelf)
                return;

            if (source == null)
                source = Source;
            else if (IsWorthPoints)
                pointValue++;

            var explosion = _ExplosionPool.Acquire();
            explosion.Explode(source, transform.position, pointValue);

            this.TryReleaseOrDestroyGameObject();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called when this object is released back to its <see cref="ObjectPool{T}"/>.
        /// Resets its values back to their defaults.
        /// </summary>
        public override void OnRelease()
        {
            base.OnRelease();
            Source = null;
        }

        /************************************************************************************************************************/
    }
}

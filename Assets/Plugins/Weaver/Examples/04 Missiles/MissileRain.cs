// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// Randomly spawns missiles at a gradually increasing rate over an area.
    /// </summary>
    public sealed class MissileRain : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private float _SpawnRadius;

        /// <summary>The range in either direction within which missiles are spawned.</summary>
        public float SpawnRadius
        {
            get { return _SpawnRadius; }
            set { _SpawnRadius = value; }
        }

        [SerializeField]
        private float _FireRate;

        /// <summary>The base number of missiles to fire per second.</summary>
        public float FireRate
        {
            get { return _FireRate; }
            set { _FireRate = value; }
        }

        [SerializeField]
        private float _FireRateAcceleration;

        /// <summary>The amount to increase the fire rate for each missile fired.</summary>
        public float FireRateAcceleration
        {
            get { return _FireRateAcceleration; }
            set { _FireRateAcceleration = value; }
        }

        [SerializeField]
        private float _MissileSpeed;

        /// <summary>The base speed of each missile when it is fired.</summary>
        public float MissileSpeed
        {
            get { return _MissileSpeed; }
            set { _MissileSpeed = value; }
        }

        [SerializeField]
        private float _MissileSpeedAcceleration;

        /// <summary>The additional speed to give missiles for each one fired previously.</summary>
        public float MissileSpeedAcceleration
        {
            get { return _MissileSpeedAcceleration; }
            set { _MissileSpeedAcceleration = value; }
        }

        [SerializeField]
        private float _MissileSpeedAccelerationDecay;

        /// <summary>The rate at which <see cref="_MissileSpeedAcceleration"/> diminishes for each missile fired.</summary>
        public float MissileSpeedAccelerationDecay
        {
            get { return _MissileSpeedAccelerationDecay; }
            set { _MissileSpeedAccelerationDecay = value; }
        }

        [SerializeField]
        private MissileList _MissilePrefabs;

        /// <summary>The missile types to choose from each time one is fired.</summary>
        public MissileList MissilePrefabs
        {
            get { return _MissilePrefabs; }
            set { _MissilePrefabs = value; }
        }

        /************************************************************************************************************************/

        /// <summary>Counts up to determine when the next missile will be fired.</summary>
        private float _FireTimer = 1;

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per physics update by Unity.
        /// Periodically fires a missile and accelerates the fire rate and missile speed.
        /// </summary>
        private void FixedUpdate()
        {
            _FireTimer += _FireRate * Time.deltaTime;

            if (_FireTimer >= 1)
            {
                Fire();
                _FireRate += _FireRateAcceleration;

                _MissileSpeed += _MissileSpeedAcceleration;
                _MissileSpeedAcceleration *= 1 - _MissileSpeedAccelerationDecay;

                _FireTimer--;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Spawns a missile at a random horizontal position within the <see cref="_SpawnRadius"/> and fires it at a
        /// random position on the ground within the same area.
        /// </summary>
        private void Fire()
        {
            // Spawn at a random horizontal position.
            var position = transform.position;
            position.x += Random.Range(-_SpawnRadius, _SpawnRadius);

            // Aim at a random position on the ground.
            var target = new Vector3(Random.Range(-_SpawnRadius, _SpawnRadius), 0, 0);
            var rotation = Quaternion.LookRotation(target - position, Vector3.forward);

            // Pick a random missile prefab and get the shared pool of its instances.
            var prefab = _MissilePrefabs.GetRandomElement();
            var pool = ObjectPool.GetSharedComponentPool(prefab);

            // Get a missile from that pool and fire it.
            var missile = pool.Acquire();
            missile.Fire(null, position, rotation, _MissileSpeed, true);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when the scene view draws its gizmos.
        /// Draws a line to visualise the <see cref="_SpawnRadius"/>.
        /// </summary>
        private void OnDrawGizmos()
        {
            var left = transform.position;
            left.x -= _SpawnRadius;

            var right = transform.position;
            right.x += _SpawnRadius;

            Gizmos.DrawLine(left, right);
        }

        /************************************************************************************************************************/
    }
}

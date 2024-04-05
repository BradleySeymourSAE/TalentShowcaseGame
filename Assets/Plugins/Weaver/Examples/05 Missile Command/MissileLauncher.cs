// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using System.Collections;
using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// Rotates the <see cref="_Launcher"/> to face the mouse cursor and fires a <see cref="Missile"/> whenever the
    /// player clicks the mouse.
    /// </summary>
    public sealed class MissileLauncher : MonoBehaviour, IDealDamage
    {
        /************************************************************************************************************************/

        [SerializeField]
        private Transform _Launcher;

        /// <summary>The object to rotate to aim.</summary>
        public Transform Launcher
        {
            get { return _Launcher; }
            set { _Launcher = value; }
        }

        [SerializeField]
        private Missile _MissilePrefab;

        /// <summary>The prefab to clone when firing missiles.</summary>
        public Missile MissilePrefab
        {
            get { return _MissilePrefab; }
            set { _MissilePrefab = value; }
        }

        [SerializeField]
        private float _MissileSpeed;

        /// <summary>The movement speed of missiles that are fired.</summary>
        public float MissileSpeed
        {
            get { return _MissileSpeed; }
            set { _MissileSpeed = value; }
        }

        /************************************************************************************************************************/

        /// <summary>A pool made from the <see cref="_MissilePrefab"/> which instances will be acquired from.</summary>
        private ObjectPool<Missile> _MissilePool;

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity the first time this component becomes enabled and active.
        /// Initializes the <see cref="_MissilePool"/>.
        /// </summary>
        private void Awake()
        {
            _MissilePool = ObjectPool.GetSharedComponentPool(_MissilePrefab);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per frame by Unity.
        /// Rotates the <see cref="_Launcher"/> to face the mouse cursor and fires a missile if the user clicked.
        /// </summary>
        private void Update()
        {
            var target = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
            target.z = 0;

            _Launcher.LookAt(target, Vector3.forward);

            if (Input.GetMouseButtonDown(0))
            {
                Fire(target);
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Fires a <see cref="Missile"/> from the <see cref="_MissilePool"/> and starts a coroutine to cause it to
        /// explode exactly at the target point.
        /// </summary>
        private void Fire(Vector3 target)
        {
            var position = _Launcher.position;

            var rotation = Quaternion.LookRotation(target - position, Vector3.forward);

            var missile = _MissilePool.Acquire();
            missile.Fire(this, position, rotation, _MissileSpeed, false);
            missile.StartCoroutine(ExplodeAtTarget(missile, target));
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calculates how long the `missile` will take to reach the `target` and waits that long, then causes it to
        /// explode as if it had hit something.
        /// </summary>
        private IEnumerator ExplodeAtTarget(Missile missile, Vector3 target)
        {
            float distance = Vector3.Distance(missile.transform.position, target);
            float timeToTarget = distance / missile.Speed;
            yield return new WaitForSeconds(timeToTarget);

            missile.OnCollisionEnter();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Whenever a player missile destroys a non-player one, the player gets points.
        /// </summary>
        public void OnDealDamage(ITakeDamage target, int pointValue)
        {
            if (target.IsWorthPoints)
                Score.AddPoints(pointValue, target.transform.position);
        }

        /************************************************************************************************************************/
    }
}

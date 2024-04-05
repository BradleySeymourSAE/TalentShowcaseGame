// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// A simple spherical explosion which grows then shrinks over time and uses the <see cref="DamageSystem"/> to
    /// notify objects that it hits.
    /// </summary>
    public sealed class Explosion : PoolableBehaviour<Explosion>
    {
        /************************************************************************************************************************/

        [SerializeField]
        private float _Duration;

        /// <summary>The length of time this explosion will be active for (in seconds).</summary>
        public float Duration
        {
            get { return _Duration; }
            set { _Duration = value; }
        }

        [SerializeField]
        private float _MaxRadius;

        /// <summary>The maximum radius this explosion will grow to.</summary>
        public float MaxRadius
        {
            get { return _MaxRadius; }
            set { _MaxRadius = value; }
        }

        /************************************************************************************************************************/

        /// <summary>The amount of time that has passed since this explosion became active.</summary>
        private float _Timer;

        /// <summary>The owner of this explosion.</summary>
        public IDealDamage Source { get; private set; }

        /// <summary>The number of points this explosion will give the player for each new object it destroys.</summary>
        public int PointValue { get; private set; }

        /************************************************************************************************************************/

        /// <summary>Initializes and activates this explosion.</summary>
        public void Explode(IDealDamage source, Vector3 position, int pointValue)
        {
            Source = source;
            transform.position = position;
            PointValue = pointValue;
            gameObject.SetActive(true);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when this component becomes enabled and active.
        /// Resets the scale to zero.
        /// </summary>
        private void OnEnable()
        {
            transform.localScale = Vector3.zero;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per physics update by Unity.
        /// Updates the size of this explosion and checks if any objects are hit.
        /// </summary>
        private void FixedUpdate()
        {
            _Timer += Time.deltaTime;

            if (_Timer < _Duration)
            {
                float radius = _Timer / _Duration;// Scale to 0-1 range.
                radius = Mathf.Sin(radius * Mathf.PI);// Grow then shrink using a sine wave.
                radius *= _MaxRadius;// Grow to the specified radius.

                transform.localScale = Vector3.one * (radius * 2);

                var hits = Physics.OverlapSphere(transform.position, radius);
                for (int i = 0; i < hits.Length; i++)
                {
                    DamageSystem.DealDamage(Source, hits[i], PointValue);
                }
            }
            else
            {
                this.TryReleaseOrDestroyGameObject();
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called when this object is released back to its <see cref="ObjectPool{T}"/>.
        /// Resets its values back to their defaults.
        /// </summary>
        public override void OnRelease()
        {
            base.OnRelease();
            _Timer = 0;
            Source = null;
            PointValue = 0;
        }

        /************************************************************************************************************************/
    }
}

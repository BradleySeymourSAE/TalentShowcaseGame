// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using System.Collections;
using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// When collided with, this component shows a <see cref="FloatingText"/> to indicate that the colliding object is
    /// stunned and temporarily freezes all its <see cref="Rigidbody.constraints"/> so it can't move.
    /// </summary>
    public sealed class StunOnContact : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>The amount of time the colliding object will be stunned for (in seconds).</summary>
        public float duration;

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity whenever a collision is detected between this object and another.
        /// Starts the <see cref="Stun"/> coroutine.
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            StartCoroutine(Stun(collision.rigidbody));
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Shows a <see cref="FloatingText"/> to indicate that the colliding object is stunned and freezes all its
        /// <see cref="Rigidbody.constraints"/> so it can't move for the <see cref="duration"/>.
        /// </summary>
        private IEnumerator Stun(Rigidbody body)
        {
            TextManager.ShowStatusText("Stunned", body.position, duration);

            var constraints = body.constraints;

            body.constraints = RigidbodyConstraints.FreezeAll;

            yield return new WaitForSeconds(duration);

            body.constraints = constraints;
        }

        /************************************************************************************************************************/
    }
}

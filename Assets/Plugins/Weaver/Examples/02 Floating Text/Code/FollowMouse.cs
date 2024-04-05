// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// Constantly accelerates a <see cref="Rigidbody"/> towards the mouse cursor on the XZ plane.
    /// </summary>
    public sealed class FollowMouse : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>The amount of acceleration force to apply every frame.</summary>
        public float acceleration;

        /// <summary>The <see cref="Rigidbody"/> to apply the acceleration to.</summary>
        public Rigidbody body;

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per physics update by Unity.
        /// Accelerates the target <see cref="body"/> towards the <see cref="MouseTarget"/>.
        /// </summary>
        private void FixedUpdate()
        {
            // Figure out where on the ground the mouse is pointing and accelerate towards it.

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var direction = hit.point - transform.position;
                direction.y = 0;
                direction.Normalize();

                body.AddForce(direction * acceleration, ForceMode.Acceleration);
            }
        }

        /************************************************************************************************************************/
    }
}

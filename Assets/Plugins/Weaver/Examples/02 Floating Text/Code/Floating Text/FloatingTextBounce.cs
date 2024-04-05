// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// Causes the <see cref="Transform.localScale"/> of a <see cref="FloatingText"/> to smoothly bounce in and out
    /// over the course of its lifetime using a sine curve.
    /// </summary>
    public class FloatingTextBounce : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>The target <see cref="FloatingText"/> to control.</summary>
        public FloatingText text;

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per frame by Unity.
        /// Updates the <see cref="Transform.localScale"/> of the target <see cref="text"/>.
        /// </summary>
        private void LateUpdate()
        {
            var progress = text.Progress;

            progress = 1 - progress;
            progress *= progress;
            progress = 1 - progress;
            progress = Mathf.Sin(progress * Mathf.PI);

            text.Transform.localScale = Vector3.one * progress;
        }

        /************************************************************************************************************************/
    }
}

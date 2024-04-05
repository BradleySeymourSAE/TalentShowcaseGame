// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// Causes a <see cref="FloatingText.Color"/> to fade its alpha value from 1 to 0 over the course of its lifetime.
    /// </summary>
    public class FloatingTextFade : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>The target <see cref="FloatingText"/> to control.</summary>
        public FloatingText text;

        /// <summary>The original <see cref="FloatingText.Color"/>.</summary>
        private Color _Color;

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when this component becomes enabled and active.
        /// Stores the <see cref="FloatingText.Color"/> so it can be restored by <see cref="OnDisable"/>.
        /// </summary>
        private void OnEnable()
        {
            _Color = text.Color;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per frame by Unity.
        /// Updates the <see cref="FloatingText.Color"/> of the target <see cref="text"/>.
        /// </summary>
        private void LateUpdate()
        {
            var color = _Color;
            color.a *= 1 - text.Progress;

            text.Color = color;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when this component becomes disabled or inactive.
        /// Restores the original <see cref="FloatingText.Color"/> captured by <see cref="OnEnable"/>.
        /// </summary>
        private void OnDisable()
        {
            text.Color = _Color;
        }

        /************************************************************************************************************************/
    }
}

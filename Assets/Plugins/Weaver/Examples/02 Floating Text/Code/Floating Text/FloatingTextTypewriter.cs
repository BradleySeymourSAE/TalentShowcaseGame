// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// Causes a <see cref="FloatingText"/> to gradually show each of its character one at a time with a short delay.
    /// </summary>
    public class FloatingTextTypewriter : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>The target <see cref="FloatingText"/> to control.</summary>
        public FloatingText text;

        /// <summary>The amount of time that will pass between each character appearing.</summary>
        public float interval;

        /// <summary>The amount of time that the full text will remain visible after all characters have appeared.</summary>
        public float finalDuration;

        /************************************************************************************************************************/

        /// <summary>The whole string that will eventually be shown.</summary>
        private string _FullText;

        /// <summary>The number of currently visible characters.</summary>
        private int _VisibleChars;

        /// <summary>A timer that determines when to show the next character.</summary>
        private float _CharTimer;

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when this component becomes enabled and active.
        /// Stores the full <see cref="FloatingText.Text"/> and calculates the <see cref="FloatingText.LifeTime"/>
        /// based on its length.
        /// </summary>
        private void OnEnable()
        {
            _FullText = text.Text;
            text.Text = "";
            text.LifeTime = _FullText.Length * interval + finalDuration;

            _VisibleChars = 0;
            _CharTimer = 0;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called once per frame by Unity.
        /// Updates the timer and checks if it is time to show another character of the string.
        /// </summary>
        private void Update()
        {
            _CharTimer += Time.deltaTime;
            if (_CharTimer < interval)
                return;

            _CharTimer -= interval;
            _VisibleChars++;

            if (_VisibleChars < _FullText.Length)
            {
                text.Text = _FullText.Substring(0, _VisibleChars);
            }
            else
            {
                text.Text = _FullText;
                _CharTimer = float.NegativeInfinity;
            }
        }

        /************************************************************************************************************************/
    }
}

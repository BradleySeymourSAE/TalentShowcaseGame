// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// A simple destructible building for Example 05 Missile Command which also serves as an example of how to create
    /// a procedural asset.
    /// </summary>
    // This is a partial class with the procedural asset code in the 03 Building example.
    public sealed partial class Building : MonoBehaviour, ITakeDamage
    {
        /************************************************************************************************************************/

        /// <summary>The number of <see cref="Building"/>s that currently exist in the scene.</summary>
        private static int _BuildingCount;

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity the first time this component becomes enabled and active.
        /// Increments the <see cref="_BuildingCount"/>.
        /// </summary>
        private void Awake()
        {
            _BuildingCount++;
        }

        /************************************************************************************************************************/

        /// <summary>Destroying buildings is not worth any points.</summary>
        public bool IsWorthPoints { get { return false; } }

        /// <summary>
        /// Called when something damages this object via <see cref="ITakeDamage"/>.
        /// Destroys this object and shows some <see cref="FloatingText"/>.
        /// </summary>
        public void OnTakeDamage(IDealDamage source, int pointValue)
        {
            // We could use a different text prefab or explosion or something,
            // but for the sake of simplicity we just reuse the score text.
            Score.Text.Acquire().Show("Boom", transform.position);

            Destroy(gameObject);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when this component is destroyed.
        /// Decrements the <see cref="_BuildingCount"/> and shows the <see cref="GameOverScreen"/> if none are left.
        /// </summary>
        private void OnDestroy()
        {
            _BuildingCount--;

#if UNITY_EDITOR
            // Don't show the game over screen if we are leaving Play Mode.
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return;
#endif

            if (_BuildingCount == 0)
                GameOverScreen.Show();
        }

        /************************************************************************************************************************/
    }
}

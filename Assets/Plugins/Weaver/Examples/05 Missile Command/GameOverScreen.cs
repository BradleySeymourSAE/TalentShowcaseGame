// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to.

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Weaver.Examples
{
    /// <summary>
    /// Pauses the game when shown and has a function to reload the current scene.
    /// <para></para>
    /// The <see cref="Prefab"/> field serves as an example of Lazy Asset Injection.
    /// </summary>
    public sealed class GameOverScreen : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Rather than being a direct reference to a <see cref="GameOverScreen"/> prefab, this field uses
        /// <see cref="Asset{T}"/> so that Weaver will inject the prefab's resource path on startup, meaning the prefab
        /// itself doesn't have to be loaded until it is actually used.
        /// </summary>
        [AssetReference]
        private static readonly Asset<GameOverScreen> Prefab;

        /************************************************************************************************************************/

        /// <summary>
        /// Instantiates the <see cref="Prefab"/> and freezes time.
        /// </summary>
        public static void Show()
        {
            Instantiate(Prefab.Target);
            Time.timeScale = 0;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Unfreezes time and reloads the current scene.
        /// </summary>
        public void PlayAgain()
        {
            Time.timeScale = 1;
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }

        /************************************************************************************************************************/
    }
}

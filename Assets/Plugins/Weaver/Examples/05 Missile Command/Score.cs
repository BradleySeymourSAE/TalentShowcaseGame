// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Weaver.Examples
{
    /// <summary>
    /// A simple scoring system with a high score and <see cref="FloatingText"/> to indicate when points are gained.
    /// </summary>
    public sealed class Score : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="ObjectPool{T}"/> made from an injected <see cref="FloatingText"/> prefab which is used for
        /// indicating score gains.
        /// </summary>
        [AssetPool]
        public static readonly ObjectPool<FloatingText> Text;

        /// <summary>
        /// The highest store that has been achieved.
        /// <para></para>
        /// This property uses the <see cref="PrefAttribute"/> to automatically save its value in
        /// <see cref="PlayerPrefs"/> using its full name as the key: "Weaver.Examples.Score.HighScore".
        /// </summary>
        [Pref]
        public static int HighScore { get; private set; }

        /// <summary>
        /// A callback which is triggered whenever the current score changes.
        /// </summary>
        public static event Action OnPointsChanged;

        /************************************************************************************************************************/

        private static int _Points;

        /// <summary>The current score.</summary>
        public static int Points
        {
            get { return _Points; }
            set
            {
                _Points = value;

                if (HighScore < value)
                    HighScore = value;

                if (OnPointsChanged != null)
                    OnPointsChanged();
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Adds to the current <see cref="Points"/> and shows a <see cref="FloatingText"/> to indicate the increase.
        /// </summary>
        public static void AddPoints(int points, Vector3 position)
        {
            Points += points;

            Text.Show("+" + points, position);
        }

        /************************************************************************************************************************/

        [SerializeField]
        private Text _ScoreText;

        /// <summary>The UI text component used to display the score.</summary>
        public Text ScoreText
        {
            get { return _ScoreText; }
            set { _ScoreText = value; }
        }

        /// <summary>
        /// Called by Unity when this component becomes enabled and active.
        /// Registers a callback to <see cref="OnPointsChanged"/> to update the UI text and resets the
        /// <see cref="Points"/>.
        /// </summary>
        private void OnEnable()
        {
            OnPointsChanged += UpdateScoreText;
            Points = 0;
        }

        /// <summary>
        /// Called by Unity when this component becomes disabled or inactive.
        /// Unregisters the text update callback from <see cref="OnPointsChanged"/>.
        /// </summary>
        private void OnDisable()
        {
            OnPointsChanged -= UpdateScoreText;
        }

        /// <summary>
        /// Changes the <see cref="_ScoreText"/> to show the current score and high score.
        /// </summary>
        private void UpdateScoreText()
        {
            _ScoreText.text =
                "Score: " + Points +
                "\nHigh Score: " + HighScore;
        }

        /************************************************************************************************************************/
    }
}

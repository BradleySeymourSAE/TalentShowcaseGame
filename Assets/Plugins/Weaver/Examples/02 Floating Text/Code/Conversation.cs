// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using System.Collections;
using UnityEngine;

namespace Weaver.Examples
{
    /// <summary>
    /// A simple sequence that shows a series of pre-defined lines as speech text above specific objects as if they
    /// were having a conversation.
    /// </summary>
    public sealed class Conversation : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>The target objects above which the text will be shown.</summary>
        public Transform[] speakers;

        /// <summary>The individual lines of text to show.</summary>
        public string[] lines;

        /// <summary>The amount of time to pause between each line.</summary>
        public float pauseTime;

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity the first time this component becomes enabled and active.
        /// Starts the <see cref="SpeechSequence"/> coroutine.
        /// </summary>
        private void Awake()
        {
            StartCoroutine(SpeechSequence());
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Shows one of the <see cref="lines"/> above one of the <see cref="speakers"/>, then waits for it to finish
        /// and shows the next line above the next speaker and continues looping indefinitely.
        /// </summary>
        private IEnumerator SpeechSequence()
        {
            int currentSpeaker = 0;
            int currentLine = 0;

            while (true)
            {
                var text = TextManager.ShowSpeechText(lines[currentLine], speakers[currentSpeaker].position);

                yield return new WaitForSeconds(text.LifeTime + pauseTime);

                currentSpeaker++;
                if (currentSpeaker >= speakers.Length)
                    currentSpeaker = 0;

                currentLine++;
                if (currentLine >= lines.Length)
                    currentLine = 0;
            }
        }

        /************************************************************************************************************************/
    }
}

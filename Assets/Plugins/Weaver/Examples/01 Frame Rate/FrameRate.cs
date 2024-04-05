// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using UnityEngine;
using UnityEngine.UI;

namespace Weaver.Examples
{
    /// <summary>
    /// A system that uses a <see cref="Text"/> component to display the current Frame Rate (Frames Per Second).
    /// The displayed value is updated once per second.
    /// </summary>
    public sealed class FrameRate : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>
        /// The <see cref="AssetInstanceAttribute"/> causes this field to show up in the Weaver Window so the user can
        /// assign a prefab to it (specifically, a prefab with a <see cref="FrameRate"/> component on it). Then Weaver
        /// will automatically instantiate a copy of that prefab on startup and assign it to this field.
        /// </summary>
        /// <remarks>
        /// This could be a private field, but having it public means other scripts can disable it if you want. We
        /// could have also used an <see cref="AssetReferenceAttribute"/> to just get a reference to the chosen prefab
        /// and then instantiate it ourselves only if the user actually enables it.
        /// </remarks>
        [AssetInstance(Optional = true)]
        public static readonly FrameRate Instance;

        /************************************************************************************************************************/

        [SerializeField]
        private Text _Text;

        /// <summary>The <see cref="Text"/> component that will display the current value.</summary>
        public ref Text Text => ref _Text;

        /// <summary>A count of the number of frames that have been rendered in the current interval.</summary>
        private int _FrameCount;

        /// <summary>The amount of time that has passed in the current interval.</summary>
        private double _Timer;

        /************************************************************************************************************************/

        /// <summary>Called once per frame by Unity.</summary>
        private void Update()
        {
            _FrameCount++;
            _Timer += Time.unscaledDeltaTime;

            // Every time one second passes, update the text to show the number of frames that were counted.
            if (_Timer >= 1)
            {
                _Text.text = _FrameCount.ToString();// This will unfortunately allocate a tiny bit of garbage every time.
                _FrameCount = 0;
                _Timer -= 1;
            }
        }

        /************************************************************************************************************************/
    }

    /************************************************************************************************************************/
#if UNITY_EDITOR
    /************************************************************************************************************************/

    /// <summary>[Editor-Only] Custom inspector for <see cref="FrameRate"/>.</summary>
    [UnityEditor.CustomEditor(typeof(FrameRate))]
    internal sealed class FrameRateEditor : UnityEditor.Editor
    {
        /************************************************************************************************************************/

        /// <summary>Displays some additional information below the regular inspector.</summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UnityEditor.EditorGUILayout.HelpBox("This FPS counter is an example included in Weaver." +
                "\n\nIt can be disabled by removing the 'FrameRate.Instance' prefab reference" +
                " from the 'Injectors' panel in the Weaver Window (or by deleting the FrameRate.cs script).",
                UnityEditor.MessageType.Info);

            if (GUILayout.Button($"Open Weaver Window ({WeaverUtilities.WeaverWindowPath})"))
                UnityEditor.EditorApplication.ExecuteMenuItem(WeaverUtilities.WeaverWindowPath);
        }

        /************************************************************************************************************************/
    }

    /************************************************************************************************************************/
#endif
    /************************************************************************************************************************/
}

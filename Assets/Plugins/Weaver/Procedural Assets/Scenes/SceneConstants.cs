// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

#if UNITY_EDITOR

using System.Text;
using UnityEditor;

namespace Weaver.Editor.Procedural
{
    /// <summary>[Editor-Only, Internal]
    /// Procedurally generates a script containing constants corresponding to the scenes in your build settings.
    /// </summary>
    internal static class SceneConstants
    {
        /************************************************************************************************************************/

        [AssetReference(FileName = "Scenes", DisableAutoFind = true, Optional = true,
            Tooltip = "This script can be customised via the Scene Constants panel in the Weaver Window")]
        [Window.ShowInPanel(typeof(Window.ScenesPanel), ShowInMain = true)]
        [ProceduralAsset(AutoGenerateOnBuild = true, AutoGenerateOnSave = true,
            CheckShouldShow = nameof(ShouldShowScript),
            CheckShouldGenerate = nameof(ShouldGenerateScript))]
        private static MonoScript Script { get; set; }

        /************************************************************************************************************************/

        private static readonly ScenesScriptBuilder
            ScriptBuilder = new ScenesScriptBuilder(GenerateScript);

        /************************************************************************************************************************/

        private static bool ShouldShowScript => ScriptBuilder.Enabled;

        private static bool ShouldGenerateScript => ScriptBuilder.ShouldBuild();

        [ScriptGenerator.Alias(nameof(Weaver))]
        private static void GenerateScript(StringBuilder text) => ScriptBuilder.BuildScript(text);

        /************************************************************************************************************************/
    }
}

#endif


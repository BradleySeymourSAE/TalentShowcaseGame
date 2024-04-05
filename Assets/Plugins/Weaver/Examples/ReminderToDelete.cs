// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

#if UNITY_EDITOR

using UnityEditor.Build;

namespace Weaver.Editor
{
    /// <summary>[Editor-Only]
    /// Reminds the user to delete the Weaver Examples (including this script) once they are done with them to avoid
    /// including unnecessary clutter in builds.
    /// </summary>
    internal sealed class ReminderToDelete : IPreprocessBuildWithReport
    {
        /************************************************************************************************************************/

        public int callbackOrder => -100;

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            UnityEngine.Debug.Log("Remember to <B>delete the Weaver Examples</B> once you are done with them" +
                " to avoid including unnecessary clutter in your build.",
                UnityScripts.GetScript(typeof(ReminderToDelete)));
        }

        /************************************************************************************************************************/
    }
}

#endif

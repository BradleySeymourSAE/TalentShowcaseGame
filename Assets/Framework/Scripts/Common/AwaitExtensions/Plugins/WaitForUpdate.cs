using System.Collections;
using Framework.Common.Tools.AwaitExtensions.Plugins.Engine;
namespace Framework.Common.Tools.AwaitExtensions.Plugins
{
    /// <summary>
    /// This class can be awaited.
    /// Run code after awaiting in update cycle from main thread.
    /// </summary>
    public class WaitForUpdate
    {
        /// <summary>
        /// Gets awaiter object.
        /// </summary>
        /// <returns><see cref="ManualAwaiter"/> object.</returns>
        public ManualAwaiter GetAwaiter()
        {
            var awaiter = new ManualAwaiter();

            if (ContextHelper.IsMainThread) 
                RoutineHelper.Instance.StartCoroutine(WaitOneFrameAndRunContinuationRoutine(awaiter));
            else ContextHelper.UnitySynchronizationContext.Post((state) => { awaiter.RunContinuation(); }, null);

            return awaiter;
        }

        private IEnumerator WaitOneFrameAndRunContinuationRoutine(ManualAwaiter awaiter)
        {
            yield return null;
            awaiter.RunContinuation();
        }
    }
}
using System.Collections.Generic;
using Framework.Scripts.Common;
namespace Framework.Scripts.Core.AI
{
    public class DetectableTargetManager : Singleton<DetectableTargetManager>
    {
        public List<DetectableTarget> AllDetectableTargets { get; private set; } = new List<DetectableTarget>();

        public void Register(DetectableTarget target)
        {
            AllDetectableTargets.Add(target);
        }

        public void Deregister(DetectableTarget target)
        {
            AllDetectableTargets.Remove(target);
        }
    }
}
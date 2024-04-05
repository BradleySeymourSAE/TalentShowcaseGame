using Framework.Scripts.Common;
using UnityEngine;
namespace Framework.Scripts.Core.AI
{
    public class DetectableTarget : MonoBehaviour, ITransform
    {
        [SerializeField] private bool UseLocalTargetManagerOnly = false;

        private void Start()
        {
            if (UseLocalTargetManagerOnly == false)
            {
                DetectableTargetManager.Instance.Register(this);
            }
        }

        private void OnDestroy()
        {
            if (UseLocalTargetManagerOnly == false && DetectableTargetManager.Instance)
            {
                DetectableTargetManager.Instance.Deregister(this);
            }
        }
    }
}
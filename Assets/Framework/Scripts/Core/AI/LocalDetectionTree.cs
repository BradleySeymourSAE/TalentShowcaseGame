using System.Linq;
using Framework.Scripts.Common;
using UnityEngine;
namespace Framework.Scripts.Core.AI
{
    public class LocalDetectionTree : MonoBehaviour
    {
        [SerializeField] private LayerMask DetectionLayers;
        public KdTree<DetectableTarget> Detections = new KdTree<DetectableTarget>(false);

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == gameObject || other.gameObject.HasLayerMask(DetectionLayers) == false)
            {
                return;
            }
            if (other.TryGetComponent(out DetectableTarget target) && Detections.Contains(target) == false)
            {
                Detections?.Add(target);
            }
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out DetectableTarget target) && Detections.Contains(target))
            {
                Detections?.RemoveWhere(x => x == target);
            }
        }
        
        #if UNITY_EDITOR 
        private void OnDrawGizmos()
        {
            if (Detections == null)
            {
                return;
            }
            
            Detections.DrawAllBounds();
            Detections.DrawAllObjects();
            
            foreach (var detection in Detections)
            {
                if (detection == null)
                {
                    continue;
                }
                UnityEditor.Handles.Label(detection.transform.position + Vector3.up * 2.0f, detection.name); 
            }
        } 
        #endif 
    }
}
using UnityEngine;
namespace Framework.Scripts.Core
{
    public sealed class Climbable : MonoBehaviour
    {
        [SerializeField] private float m_PathLength = 10.0f;
        [SerializeField] private Vector3 m_PathOffset = new Vector3(0f, 0f, -0.5f);
        [SerializeField] private Transform m_TopPoint;
        [SerializeField] private Transform m_BottomPoint;
        
        public Vector3 BottomAnchorPoint => transform.position + transform.TransformVector(m_PathOffset);

        public Vector3 TopAnchorPoint => BottomAnchorPoint + transform.up * m_PathLength;

        public Transform GetTopPoint()
        {
            return m_TopPoint; 
        }
        
        public Transform GetBottomPoint()
        {
            return m_BottomPoint; 
        } 
        
        public Vector3 ClosestPointAlongPath(Vector3 InPosition, out float NormalizedPathPosition)
        {
            Vector3 path = TopAnchorPoint - BottomAnchorPoint;
            Vector3 directionToPoint = InPosition - BottomAnchorPoint;
            
            float height = Vector3.Dot(directionToPoint, path.normalized);

            if (height > 0.0f)
            {
                // If below top point
                if (height <= path.magnitude)
                {
                    NormalizedPathPosition = 0;
                    return BottomAnchorPoint + path.normalized * height;
                }
                // If we are higher than top point
                NormalizedPathPosition = height - path.magnitude;
                return TopAnchorPoint;
            }
            // Below bottom point
            NormalizedPathPosition = height;
            return BottomAnchorPoint;
        }
        
        
        #if UNITY_EDITOR 
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(BottomAnchorPoint, TopAnchorPoint);

            if (m_BottomPoint == null || m_TopPoint == null)
                return;

            Gizmos.DrawWireCube(m_BottomPoint.position, Vector3.one * 0.25f);
            Gizmos.DrawWireCube(m_TopPoint.position, Vector3.one * 0.25f);
        }

        #endif 
    }
}
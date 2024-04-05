using UnityEngine;
namespace Framework.Scripts.Common
{
    public static class Utility
    {
        /// <summary>
        ///		Find's the first component of type T on the target game object or within its children. 
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="ComponentReference"></param>
        /// <typeparam name="T"></typeparam>
        public static void Assign<T>(this GameObject Target, ref T ComponentReference, bool IncludeInactive = false)
            where T : Component => ComponentReference = Target.GetComponent<T>()
            ? Target.GetComponent<T>()
            : Target.GetComponentInChildren<T>(IncludeInactive);
        
        public static void Assign<T>(this Transform Target, ref T ComponentReference, bool IncludeInactive = false) 
            where T : Component => Target.gameObject.Assign<T>(ref ComponentReference, IncludeInactive);  
        
        public static void Assign<T>(this GameObject Target, ref T[] ComponentReferences, bool IncludeInactive = false)
            where T : Component => ComponentReferences = Target.GetComponents<T>()
            .Length > 0
            ? Target.GetComponents<T>()
            : Target.GetComponentsInChildren<T>(IncludeInactive); 
        
        public static void AssignIfNull<T>(this GameObject Target, ref T ComponentReference, bool IncludeInactive = false)
            where T : Component => ComponentReference = ComponentReference == null
            ? Target.GetComponent<T>()
            : ComponentReference; 
        
        public static void AssignIfNull<T>(this Transform Target, ref T ComponentReference, bool IncludeInactive = false)
            where T : Component => Target.gameObject.AssignIfNull<T>(ref ComponentReference, IncludeInactive); 
        
        public static float Clamp0360(float eulerAngles)
        {
            float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
            if (result < 0)
            {
                result += 360f;
            }
            return result;
        }
        
        public static bool HasLayerMask(this GameObject gameObject, LayerMask Mask)
        {
            return Mask == (Mask | (1 << gameObject.layer)); 
        }
        
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        } 
        
        public static Vector2 With(this Vector2 vector, float? x = null, float? y = null)
        {
            return new Vector2(x ?? vector.x, y ?? vector.y);
        } 
        
        public static Vector3 onlyX(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, 0);
        } 
        
        public static Vector3 onlyY(this Vector3 vector)
        {
            return new Vector3(0, vector.y, 0);
        } 
        
        public static Vector3 onlyZ(this Vector3 vector)
        {
            return new Vector3(0, 0, vector.z);
        } 
        
        public static Vector3 onlyXY(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, 0); 
        } 
        
        public static Vector3 onlyXZ(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        } 
        
        public static Vector2 onlyX(this Vector2 vector)
        {
            return new Vector2(vector.x, 0);
        } 
        
        public static Vector2 onlyY(this Vector2 vector)
        {
            return new Vector2(0, vector.y);
        } 
        
        public static Vector3 CalculateBezierPoint(float t, Vector3 start, Vector3 control, Vector3 end)
        {
            return Mathf.Pow(1 - t, 2) * start + 2 * (1 - t) * t * control + Mathf.Pow(t, 2) * end;
        }
    }
}
using UnityEngine;
namespace Framework.Scripts.Core.ObjectPool
{
    [CreateAssetMenu(fileName = "ObjectPoolSettings", menuName = "ObjectPooling/Settings", order = 0)]
    public class FlyweightSettings : ScriptableObject
    {
        public ObjectPoolingBehaviourType Type; 
        public GameObject Prefab;
        
        public virtual FlyweightBehaviour Create()
        {
            var instance = Instantiate(Prefab);
            instance.SetActive(false);
            instance.name = Prefab.name; 
            FlyweightBehaviour component = instance.AddComponent<FlyweightBehaviour>();
            component.Settings = this; 
            return component; 
        }

        public virtual void OnRetrieve(FlyweightBehaviour Projectile) => Projectile.gameObject.SetActive(true); 
        public virtual void OnRelease(FlyweightBehaviour Projectile) => Projectile.gameObject.SetActive(false); 
        public virtual void OnDestroyed(FlyweightBehaviour Projectile) => Destroy(Projectile.gameObject); 
    }

}
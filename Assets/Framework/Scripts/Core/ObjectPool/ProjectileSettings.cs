using UnityEngine;
namespace Framework.Scripts.Core.ObjectPool
{
    [CreateAssetMenu(fileName = "ProjectileSettings", menuName = "ObjectPooling/ProjectileSettings", order = 0)]
    public class ProjectileSettings : FlyweightSettings
    {
        public float DespawnDelay = 5.0f;
        public float Speed = 10.0f;
        public float Damage = 10.0f; 
        public float Force = 10.0f; 
        
        public override FlyweightBehaviour Create()
        {
            GameObject instance = Instantiate(Prefab);
            instance.SetActive(false);
            instance.name = Prefab.name; 
            ObjectPoolProjectile component = instance.AddComponent<ObjectPoolProjectile>();
            component.Settings = this; 
            return component; 
        }
    }
}
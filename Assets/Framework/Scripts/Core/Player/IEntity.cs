using Framework.Scripts.Core.AI;
using Framework.Scripts.Core.Weapon;
using UnityEngine;
namespace Framework.Scripts.Core.Player
{
    public interface IEntity
    {
        public Transform transform { get; }
        public IWeapon Weapon { get; }
    }
}
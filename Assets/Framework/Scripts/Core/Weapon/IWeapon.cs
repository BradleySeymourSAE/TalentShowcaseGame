using Framework.Scripts.Core.AI.Strategies;
using Framework.Scripts.Core.Player;
using UnityEngine;
namespace Framework.Scripts.Core.Weapon
{
    public interface IWeapon
    {
        public void Initialize(StateContext StateContext); 
        public void Attack(); 
        public bool CanAttack(); 
        public Transform WeaponAttackPoint { get; set; } 
        public WeaponStrategy WeaponStrategy { get; }
        public void SetWeaponStrategy(WeaponStrategy Strategy); 
    }
}
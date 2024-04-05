using Animancer;
using UnityEngine;
namespace Framework.Scripts.Core.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private HybridAnimancerComponent m_Animator;
        [SerializeField] private MovementController m_MovementController;

        private void LateUpdate()
        {
            m_Animator.SetFloat(Animations.VelocityY, m_MovementController.GetVelocity().y);

            bool wallHolding = m_MovementController.IsWallHolding(); 
            bool wallSliding = m_MovementController.IsWallSliding();
            int wallDirection = m_MovementController.GetWallDirection(); 
            if (m_MovementController.IsWallHolding() || m_MovementController.IsWallSliding())
            {
                m_Animator.SetBool(Animations.WallHolding, wallHolding); 
                m_Animator.SetBool(Animations.WallSliding, wallSliding); 
                m_Animator.SetInteger(Animations.WallDirection, wallDirection); 
            }
            else
            {
                m_Animator.SetBool(Animations.WallHolding, false); 
                m_Animator.SetBool(Animations.WallSliding, false); 
                m_Animator.SetInteger(Animations.WallDirection, 0); 
                m_Animator.SetBool(Animations.Run, m_MovementController.IsGrounded() && Mathf.Abs(m_MovementController.GetVelocity().x) > 0.1f); 
            }
            m_Animator.SetBool(Animations.Jump, m_MovementController.IsJumping()); 
        }
        
    }
}
using System;
using System.Collections.Generic;
using Framework.Common;
using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using Framework.Scripts.Core.AI.Strategies;
using Framework.Scripts.Core.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Scripts.Core.AI
{
    public class GolemBossAI : AbstractAIEntity
    {
        [SerializeField] protected UnityEvent OnIntroAnimationCompleted = new(); 
        private bool m_Initialized = false;
        private bool m_IsInvulnerable = false; 
        private int m_CurrentStrategyIndex;

        public void PlayIntroAnimation()
        {
            StateContext.LinkedAnimator.SetTrigger(Animations.Boss_Intro);
            StateContext.LinkedAnimator.SetInteger(Animations.State, 1);
        }

        public void OnIntroAnimationCompleted_Callback()
        {
            console.log(this, "READY!");
            m_Initialized = true; 
            OnIntroAnimationCompleted.Invoke(); 
        }
        
        protected override void OnDeathAnimationCompleted_Callback()
        {
            console.log(this, "OnDeathAnimationCompleted_Callback");
            
        } 

        protected void OnBecameInvulnerable_Callback()
        {
            m_IsInvulnerable = true;  
        }
        
        protected void OnBecameVulnerable_Callback()
        {
            m_IsInvulnerable = false; 
        }

        protected override void Awake()
        {
            base.Awake(); 
            StateContext.LinkedAnimator.SetTrigger(Animations.Hidden); 
            StateContext.LinkedAnimator.SetInteger(Animations.State, 0);
            StateContext.LinkedAnimator.SetBool(Animations.Action, true); 
        }

        protected override void Update()
        {
            base.Update();
            if (IsTargetVisible())
            {
                ExecuteAttack();
            } 
        }

        protected override void HandleOnDamageTaken()
        {
            base.HandleOnDamageTaken();
            StateContext.LinkedAnimator.SetTrigger(Animations.Hurt);
        }

        protected override void HandleOnDeath(IEntity Entity)
        {
            base.HandleOnDeath(Entity);
            console.log(this, "GOLEM BOSS ENTITY", gameObject.name, "HAS DIED");
            StateContext.LinkedAnimator.SetInteger(Animations.State, 9);
        }
        
        private bool CanAttack()
        {
            WeaponStrategy strategy = GetCurrentAttackStrategy(); 
            
            return ActiveTarget != null && 
                   AttackOrigin != null && 
                   strategy != null && 
                   Time.time - StateContext.LastAttackTime >= strategy.AttackRate && 
                   DamageBuffer <= 0.0f; 
        }

        private void ExecuteAttack()
        {
            if (!CanAttack())
            {
                return;
            }
            StateContext.LastAttackTime = Time.time;
            StateContext.LinkedAnimator.SetTrigger(Animations.Attack);
        }
        
        public WeaponStrategy GetCurrentAttackStrategy()
        {
            if (m_CurrentStrategyIndex < 0 || m_CurrentStrategyIndex >= Settings.AttackStrategies.Length)
            {
                console.warn(this, "Invalid strategy index: " + m_CurrentStrategyIndex); 
                return null;
            } 
            return Settings.AttackStrategies[m_CurrentStrategyIndex]; 
        } 


        public void OnAnimationAttack()
        {
            console.log(this, "OnAnimationAttack");
            StateContext.LinkedAnimator.ResetTrigger(Animations.Attack);
            // if (!CanAttack())
            // {
            //     return;
            // }
            GetCurrentAttackStrategy().Attack(StateContext, AttackOrigin, TargetPosition);
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (Settings != null)
            {
                foreach (WeaponStrategy strategy in Settings.AttackStrategies)
                {
                    if (strategy == null)
                    {
                        continue;
                    }
                    strategy.DrawGizmos(this);
                }
            }
            if (EyesTransform != null && ActiveTarget && Settings)
            {
                Gizmos.color = Color.green;

                // check if the player is within the arc of vision 
                Vector3 direction = TargetPosition - EyesTransform.position;
                UnityEditor.Handles.Label(EyesTransform.position + Vector3.up * 0.5f, $"Detection Range: {direction.magnitude}");
                float angle = Vector3.Angle(EyesTransform.right, direction);

                bool isVisible = angle < Settings.VisionAngle && direction.sqrMagnitude <= Settings.VisionRange * Settings.VisionRange;
                Color redColor = new Color(1, 0, 0, 0.05f);
                Color greenColor = new Color(0, 1, 0, 0.05f);
                Gizmos.color = isVisible ? greenColor : redColor;
                UnityEditor.Handles.color = isVisible ? greenColor : redColor;

                // For 2D, use up instead of right
                Gizmos.DrawLine(EyesTransform.position, EyesTransform.position + EyesTransform.right * Settings.VisionRange);
                Vector3 leftDirection = Quaternion.Euler(0, 0, -Settings.VisionAngle) * EyesTransform.right;
                UnityEditor.Handles.DrawSolidArc(
                    EyesTransform.position,
                    Vector3.forward,
                    leftDirection, 2 * Settings.VisionAngle,
                    Settings.VisionRange);
            }
        }
    }
}
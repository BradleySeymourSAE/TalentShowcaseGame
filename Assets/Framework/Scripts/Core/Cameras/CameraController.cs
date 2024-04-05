using System;
using Cinemachine;
using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using UnityEngine;
namespace Framework.Scripts.Core.Cameras
{
    public class CameraController : Singleton<CameraController>
    {
        [SerializeField] private CinemachineVirtualCamera[] m_VirtualCameras = Array.Empty<CinemachineVirtualCamera>();
        [SerializeField] private float m_FallingPanAmount = 0.25f;
        [SerializeField] private float m_NormalPanAmount = 0.1f; 
        [SerializeField] private float m_FallingYPanTime = 0.35f;
        
        public bool IsLerpingYDampening { get; private set; } 
        public bool IsLerpedFromFalling { get; private set; }
        
        public CinemachineVirtualCamera CurrentVirtualCamera { get; private set; } = null; 
        public CinemachineFramingTransposer CurrentFramingTransposer { get; private set; } = null; 
        

        private void Awake()
        {
            for (int i = 0; i < m_VirtualCameras.Length; i++)
            {
                if (m_VirtualCameras[i].enabled)
                {
                    CurrentVirtualCamera = m_VirtualCameras[i]; 
                    CurrentFramingTransposer = CurrentVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>(); 
                    break; 
                }
            } 
        }
        
        public void SetVirtualCamera(CinemachineVirtualCamera virtualCamera)
        {
            CurrentVirtualCamera.enabled = false; 
            virtualCamera.enabled = true; 
            CurrentVirtualCamera = virtualCamera; 
            CurrentFramingTransposer = CurrentVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>(); 
        }
        
        public void InterpolateYDampening(bool falling)
        {
            if (IsLerpingYDampening)
            {
                return; 
            }
            ProcessFallingDampening(falling); 
        } 

        private async void ProcessFallingDampening(bool falling)
        {
            console.log(this, "Falling: ", falling);
            IsLerpingYDampening = falling; 
            float startDampening = CurrentFramingTransposer.m_YDamping;
            float targetDampening = 0.0f; 
            if (falling) 
            {
                targetDampening = m_FallingPanAmount; 
                IsLerpedFromFalling = true; 
            }
            else
            {
                targetDampening = m_NormalPanAmount; 
            } 
            float elapsed = 0.0f; 
            while (elapsed < m_FallingYPanTime)
            {
                elapsed += Time.deltaTime; 
                CurrentFramingTransposer.m_YDamping = Mathf.Lerp(startDampening, targetDampening, elapsed / m_FallingYPanTime);
                
                console.log(this, "CurrentFramingTransposer.m_YDamping: " + CurrentFramingTransposer.m_YDamping);
                await new WaitForUpdate(); 
            }
            IsLerpingYDampening = false; 
            console.log(this, "Falling: ", IsLerpingYDampening);
        }
    }
}
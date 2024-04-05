using Framework.Scripts.Common.Attributes;
using UnityEngine;
namespace Framework.Scripts.Core
{
	[ExecuteInEditMode()]
	public class ParallaxLayer : MonoBehaviour 
	{
		public enum EParallaxMode
		{
			Uniform,
			ByAxis,
		}
		
		public enum EMasterCameraSelectionMode
		{
			MainCamera,
			ByReference,
			ByName,
			ByTag,
		}
		
		[System.Serializable]
		public class FreezeAxes
		{
			public bool x;
			public bool y;
		}
		
		[System.Serializable]
		public class ParallaxByAxis
		{
			public float x = 0.5f;
			public float y = 0.5f;
		}
		
		// Parallax
		
		public EParallaxMode ParallaxMode;
		
		// Uniform mode
		public float Parallax = 0.5f;
		public FreezeAxes FreezeAxesSettings;
		
		// By Axis mode
		public ParallaxByAxis ParallaxAxisSettings;
		
		// Selected camera
		
		public EMasterCameraSelectionMode SelectionMode;
		public Camera MasterCameraReference;
		public string MasterCameraName;
		
		[TagSelector] public string MasterCameraTag;
		
		private Vector3 m_LastCameraPosition;
		
		private void Start()
		{
			SelectCamera();
			m_LastCameraPosition = GetMasterCameraPosition();
		}
		
		private void LateUpdate()
		{
			UpdateParallax();
		}
		
		private void UpdateParallax()
		{
			if(MasterCameraReference == null)
			{
				return;
			}
			Vector3 cameraPosition = GetMasterCameraPosition();
			Vector3 cameraMovement = cameraPosition - m_LastCameraPosition;
			
			// Apply parallax
			cameraMovement.z = 0.0f;
			switch(ParallaxMode)
			{
				case ParallaxLayer.EParallaxMode.Uniform:
				{
					cameraMovement *= Parallax;
					
					if(FreezeAxesSettings.x)
					{
						cameraMovement.x = 0.0f;
					}
					
					if(FreezeAxesSettings.y)
					{
						cameraMovement.y = 0.0f;
					}
				}
				break;
					
				case ParallaxLayer.EParallaxMode.ByAxis:
				{
					cameraMovement.x *= ParallaxAxisSettings.x;
					cameraMovement.y *= ParallaxAxisSettings.y;
				}
				break;
			}
			
			transform.position += cameraMovement;
			
			m_LastCameraPosition = cameraPosition;
		}
		
		private Vector3 GetMasterCameraPosition()
		{
			if(MasterCameraReference == null)
			{
				return Vector3.zero;
			}
			
			return MasterCameraReference.transform.position;
		}
		
		private void SelectCamera()
		{
			// Select camera
			Camera rSelectedCamera = null;
			switch(SelectionMode)
			{
				case EMasterCameraSelectionMode.MainCamera:
				{
					rSelectedCamera = Camera.main;
				}
				break;
				
				case EMasterCameraSelectionMode.ByReference:
				{
					rSelectedCamera = MasterCameraReference;
				}
				break;
				
				case EMasterCameraSelectionMode.ByName:
				{
					GameObject rMasterCameraGameObject = GameObject.Find(MasterCameraName);
					if(rMasterCameraGameObject == null)
					{
						Debug.LogError("Can't find the master camera named : " + MasterCameraName);
					}
					else if(rMasterCameraGameObject.GetComponent<Camera>() == null)
					{
						Debug.LogError(rMasterCameraGameObject + " doesn't have a camera, and thus can't be selected as a master camera");
					}
					else
					{
						rSelectedCamera = rMasterCameraGameObject.GetComponent<Camera>();
					}
				}
				break;
				
				case EMasterCameraSelectionMode.ByTag:
				{
					GameObject rMasterCameraGameObject = GameObject.FindGameObjectWithTag(MasterCameraTag);
					if(rMasterCameraGameObject == null)
					{
						Debug.LogError("Can't find the master camera with the tag : " + MasterCameraName);
					}
					else if(rMasterCameraGameObject.GetComponent<Camera>() == null)
					{
						Debug.LogError(rMasterCameraGameObject + " doesn't have a camera, and thus can't be selected as a master camera");
					}
					else
					{
						rSelectedCamera = rMasterCameraGameObject.GetComponent<Camera>();
					}
				}
				break;
			}
			
			// Once a camera has been selected update the public field to reflect the selection
			if(rSelectedCamera == null)
			{
				MasterCameraReference = null;
				MasterCameraName = "";
				MasterCameraTag = "";
			}
			else
			{
				MasterCameraReference = rSelectedCamera;
				MasterCameraName = rSelectedCamera.name;
				MasterCameraTag = rSelectedCamera.tag;
			}
		}
	}
}
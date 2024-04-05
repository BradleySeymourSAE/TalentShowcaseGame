using System.Collections.Generic;
using UnityEngine;
namespace Framework.Scripts.Common
{
	
	#if UNITY_EDITOR

	[ExecuteInEditMode]
	#endif
	public class DrawCollidersInEditor : MonoBehaviour
	{
		[Range(0.0f, 1.0f)] public float Alpha = 1.0f;
		public bool DrawFill = true;
		public bool DrawWire = true;
		public bool DrawCenter = true;

		public bool UseSeparateColorForTrigger = true;
		private Color m_GizmoTriggerColor = new Color(0.0f, 200.0f, 0.0f, 0.85f);

		public bool DrawMeshColliders = true;
		public bool DrawMeshCollidersAsWire = false;

		[Range(0.01f, 10.0f)] public float CenterMarkerRadius = 1.0f;
		public bool IncludeChildColliders;
		[SerializeField] private bool ManualRefresh;

		private List<EdgeCollider2D> m_CachedEdgeColliders2D;
		private List<BoxCollider2D> m_CachedBoxColliders2D;
		private List<CircleCollider2D> m_CachedCircleColliders2D;
		private List<BoxCollider> m_CachedBoxColliders;
		private List<SphereCollider> m_CachedSphereColliders;
		private List<MeshCollider> m_CachedMeshColliders;

		private readonly HashSet<Transform> m_CachedColliderTransforms = new HashSet<Transform>();

		[SerializeField] private Color m_GizmoWireColor = new Color(.6f, .6f, 1f, .5f);
		[SerializeField] private Color m_GizmoFillColor = new Color(.6f, .7f, 1f, .1f);
		[SerializeField] private Color m_GizmoCenterColor = new Color(.6f, .7f, 1f, .7f);

		[SerializeField] private Color m_GizmoMeshWireColor = new Color(250.0f, 0.0f, 0.0f, 0.75f);
		private bool HasRefreshed;

		public void SetGizmoWireColor(Color NewColor) => m_GizmoWireColor = NewColor;
		public void SetGizmoFillColor(Color NewColor) => m_GizmoFillColor = NewColor;
		public void SetGizmoCenterColor(Color NewColor) => m_GizmoCenterColor = NewColor;
		public void SetGizmoMeshColliderColor(Color NewColor) => m_GizmoMeshWireColor = NewColor;
		public void SetGizmoTriggerColor(Color NewColor) => m_GizmoTriggerColor = NewColor;

		[ContextMenu("Disable All")]
		private void DisableAll()
		{
			foreach (var drawInEditor in FindObjectsOfType<DrawCollidersInEditor>(true))
			{
				drawInEditor.enabled = false;
			}
		}

		[ContextMenu("Enable All")]
		private void EnableAll()
		{
			foreach (var drawInEditor in FindObjectsOfType<DrawCollidersInEditor>(true))
			{
				drawInEditor.enabled = true;
			}
		}

		private void OnDrawGizmos()
		{
			if (enabled == false)
			{
				return;
			}
			if (HasRefreshed == false || ManualRefresh)
			{
				ManualRefresh = false;
				Refresh();
			}
			BeginDrawingColliders();
		}

		#region Refresh

		public void Refresh()
		{
			HasRefreshed = true;

			m_GizmoWireColor = new Color(m_GizmoWireColor.r, m_GizmoWireColor.g, m_GizmoWireColor.b, m_GizmoWireColor.a * Alpha);
			m_GizmoFillColor = new Color(m_GizmoFillColor.r, m_GizmoFillColor.g, m_GizmoFillColor.b, m_GizmoFillColor.a * Alpha);
			m_GizmoCenterColor = new Color(m_GizmoCenterColor.r, m_GizmoCenterColor.g, m_GizmoCenterColor.b, m_GizmoCenterColor.a * Alpha);
			m_GizmoMeshWireColor = new Color(m_GizmoMeshWireColor.r, m_GizmoMeshWireColor.g, m_GizmoMeshWireColor.b, m_GizmoMeshWireColor.a * Alpha);
			m_GizmoTriggerColor = new Color(m_GizmoTriggerColor.r, m_GizmoTriggerColor.g, m_GizmoTriggerColor.b, m_GizmoTriggerColor.a * Alpha);
			m_CachedColliderTransforms.Clear();

			m_CachedEdgeColliders2D?.Clear();
			m_CachedBoxColliders2D?.Clear();
			m_CachedCircleColliders2D?.Clear();
			m_CachedBoxColliders?.Clear();
			m_CachedSphereColliders?.Clear();
			m_CachedMeshColliders?.Clear();

			Collider2D[] colliders2d = IncludeChildColliders
				? gameObject.GetComponentsInChildren<Collider2D>()
				: gameObject.GetComponents<Collider2D>();
			Collider[] colliders = IncludeChildColliders
				? gameObject.GetComponentsInChildren<Collider>(true)
				: gameObject.GetComponents<Collider>();

			for (int i = 0; i < colliders2d.Length; i++)
			{
				Collider2D colliderChild = colliders2d[i];

				BoxCollider2D boxCollider2D = colliderChild as BoxCollider2D;
				if (boxCollider2D != null)
				{
					if (m_CachedBoxColliders2D == null)
					{
						m_CachedBoxColliders2D = new List<BoxCollider2D>();
					}
					m_CachedBoxColliders2D.Add(boxCollider2D);
					m_CachedColliderTransforms.Add(boxCollider2D.transform);
					continue;
				}

				EdgeCollider2D edge = colliderChild as EdgeCollider2D;
				if (edge != null)
				{
					if (m_CachedEdgeColliders2D == null)
					{
						m_CachedEdgeColliders2D = new List<EdgeCollider2D>();
					}
					m_CachedEdgeColliders2D.Add(edge);
					m_CachedColliderTransforms.Add(edge.transform);
					continue;
				}

				CircleCollider2D circle2d = colliderChild as CircleCollider2D;
				if (circle2d != null)
				{
					if (m_CachedCircleColliders2D == null)
					{
						m_CachedCircleColliders2D = new List<CircleCollider2D>();
					}
					m_CachedCircleColliders2D.Add(circle2d);
					m_CachedColliderTransforms.Add(circle2d.transform);
				}
			}

			for (int i = 0; i < colliders.Length; i++)
			{
				Collider currentCollider = colliders[i];

				BoxCollider box = currentCollider as BoxCollider;
				if (box != null)
				{
					if (m_CachedBoxColliders == null)
					{
						m_CachedBoxColliders = new List<BoxCollider>();
					}
					m_CachedBoxColliders.Add(box);
					m_CachedColliderTransforms.Add(box.transform);
					continue;
				}

				SphereCollider sphere = currentCollider as SphereCollider;
				if (sphere != null)
				{
					if (m_CachedSphereColliders == null)
					{
						m_CachedSphereColliders = new List<SphereCollider>();
					}
					m_CachedSphereColliders.Add(sphere);
					m_CachedColliderTransforms.Add(sphere.transform);
				}

				MeshCollider mesh = currentCollider as MeshCollider;
				if (mesh != null)
				{
					if (m_CachedMeshColliders == null)
					{
						m_CachedMeshColliders = new List<MeshCollider>();
					}
					m_CachedMeshColliders.Add(mesh);
					m_CachedColliderTransforms.Add(mesh.transform);
				}
			}
		}

		#endregion

		#region Drawers

		private void DrawEdgeCollider2D(EdgeCollider2D coll)
		{
			Transform target = coll.transform;
			Vector3 lossyScale = target.lossyScale;
			Vector3 position = target.position;

			Gizmos.color = m_GizmoWireColor;
			Vector3 previous = Vector2.zero;
			bool first = true;
			for (int i = 0; i < coll.points.Length; i++)
			{
				Vector2 collPoint = coll.points[i];
				Vector3 pos = new Vector3(collPoint.x * lossyScale.x, collPoint.y * lossyScale.y, 0);
				Vector3 rotated = target.rotation * pos;

				if (first)
				{
					first = false;
				}
				else
				{
					Gizmos.color = m_GizmoWireColor;
					Gizmos.DrawLine(position + previous, position + rotated);
				}
				previous = rotated;

				DrawColliderGizmo(target.position + rotated, .05f,coll.isTrigger);
			}
		}

		private void DrawBoxCollider2D(BoxCollider2D coll)
		{
			Transform target = coll.transform;
			Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
			DrawColliderGizmo(coll.offset, coll.size,coll.isTrigger);
			Gizmos.matrix = Matrix4x4.identity;
		}

		private void DrawBoxCollider(BoxCollider coll)
		{
			Transform target = coll.transform;
			Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
			DrawColliderGizmo(coll.center, coll.size,coll.isTrigger);
			Gizmos.matrix = Matrix4x4.identity;
		}

		private void DrawCircleCollider2D(CircleCollider2D coll)
		{
			Transform target = coll.transform;
			Vector2 offset = coll.offset;
			Vector3 scale = target.lossyScale;
			DrawColliderGizmo(target.position + new Vector3(offset.x, offset.y, 0.0f), coll.radius * Mathf.Max(scale.x, scale.y),coll.isTrigger);
		}

		private void DrawSphereCollider(SphereCollider coll)
		{
			Transform target = coll.transform;
			Vector3 scale = target.lossyScale;
			Vector3 center = coll.center;
			float max = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)); // to not use Mathf.Max version with params[]
			DrawColliderGizmo(target.position + new Vector3(center.x, center.y, 0.0f), coll.radius * max,coll.isTrigger);
		}

		private void DrawMeshCollider(MeshCollider MeshCollider)
		{
			if (MeshCollider == null || MeshCollider.sharedMesh == null)
			{
				return;
			}
			Transform target = MeshCollider.transform;
			Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
			Gizmos.color = m_GizmoMeshWireColor;
			Gizmos.matrix = Matrix4x4.identity;
			DrawColliderGizmo(MeshCollider.sharedMesh, MeshCollider.transform.position, MeshCollider.transform.rotation, MeshCollider.transform.lossyScale, MeshCollider.isTrigger);
		}

		private void BeginDrawingColliders()
		{
			if (DrawCenter)
			{
				Gizmos.color = m_GizmoCenterColor;
				foreach (Transform withCollider in m_CachedColliderTransforms)
				{
					Gizmos.DrawSphere(withCollider.position, CenterMarkerRadius);
				}
			}
			Gizmos.color = new Color(m_GizmoWireColor.r, m_GizmoWireColor.g, m_GizmoWireColor.b, m_GizmoWireColor.a * Alpha);
			Gizmos.color = new Color(m_GizmoFillColor.r, m_GizmoFillColor.g, m_GizmoFillColor.b, m_GizmoFillColor.a * Alpha);
			if (!DrawWire && !DrawFill)
			{
				return;
			}

			if (m_CachedEdgeColliders2D != null)
			{
				foreach (EdgeCollider2D edge in m_CachedEdgeColliders2D)
				{
					if (edge == null)
					{
						continue;
					}
					DrawEdgeCollider2D(edge);
				}
			}

			if (m_CachedBoxColliders2D != null)
			{
				for (int index = 0; index < m_CachedBoxColliders2D.Count; index++)
				{
					BoxCollider2D box = m_CachedBoxColliders2D[index];
					if (box == null)
					{
						continue;
					}
					DrawBoxCollider2D(box);
				}
			}

			if (m_CachedCircleColliders2D != null)
			{
				for (int index = 0; index < m_CachedCircleColliders2D.Count; index++)
				{
					CircleCollider2D circle = m_CachedCircleColliders2D[index];
					if (circle == null)
					{
						continue;
					}
					DrawCircleCollider2D(circle);
				}
			}

			if (m_CachedBoxColliders != null)
			{
				for (int index = 0; index < m_CachedBoxColliders.Count; index++)
				{
					BoxCollider box = m_CachedBoxColliders[index];
					if (box == null)
					{
						continue;
					}
					DrawBoxCollider(box);
				}
			}

			if (m_CachedSphereColliders != null)
			{
				for (int index = 0; index < m_CachedSphereColliders.Count; index++)
				{
					SphereCollider sphere = m_CachedSphereColliders[index];
					if (sphere == null)
					{
						continue;
					}
					DrawSphereCollider(sphere);
				}
			}

			if (m_CachedMeshColliders != null)
			{
				for (int index = 0; index < m_CachedMeshColliders.Count; index++)
				{
					MeshCollider mesh = m_CachedMeshColliders[index];
					if (mesh == null)
					{
						continue;
					}
					DrawMeshCollider(mesh);
				}
			}
		}

		private void DrawColliderGizmo(Vector3 position, Vector3 size, bool IsTriggerCollider = false)
		{
			if (DrawWire)
			{
				Gizmos.color = UseSeparateColorForTrigger && IsTriggerCollider ? m_GizmoTriggerColor : m_GizmoWireColor;
				Gizmos.DrawWireCube(position, size);
			}
			if (DrawFill)
			{
				Gizmos.color = UseSeparateColorForTrigger && IsTriggerCollider ? m_GizmoTriggerColor : m_GizmoFillColor;
				Gizmos.DrawCube(position, size);
			}
		}

		private void DrawColliderGizmo(Vector3 position, float radius, bool IsTriggerCollider = false)
		{
			if (DrawWire)
			{
				Gizmos.color = UseSeparateColorForTrigger && IsTriggerCollider ? m_GizmoTriggerColor : m_GizmoWireColor;
				Gizmos.DrawWireSphere(position, radius);
			}

			if (DrawFill)
			{
				Gizmos.color = UseSeparateColorForTrigger && IsTriggerCollider ? m_GizmoTriggerColor : m_GizmoFillColor;
				Gizmos.DrawSphere(position, radius);
			}
		}

		private void DrawColliderGizmo(Mesh Mesh, Vector3 position, Quaternion rotation, Vector3 scale, bool IsTriggerCollider = false)
		{
			if (DrawWire)
			{
				Gizmos.color = UseSeparateColorForTrigger && IsTriggerCollider ? m_GizmoTriggerColor : m_GizmoWireColor;
				Gizmos.DrawWireMesh(Mesh, position, rotation, scale);
			}

			if (DrawFill)
			{
				Gizmos.color = UseSeparateColorForTrigger && IsTriggerCollider ? m_GizmoTriggerColor : m_GizmoFillColor;
				Gizmos.DrawMesh(Mesh, position, rotation, scale);
			}
		}

		#endregion

	}
}
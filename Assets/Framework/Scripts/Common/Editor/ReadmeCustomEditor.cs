#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Framework.Scripts.Common.Editor
{
	[CustomEditor(typeof(Readme))]
	public class ReadmeEditorInspector : UnityEditor.Editor
	{
		private const float K_INDENT_SPACE = 16f;
		private Readme readme;

		private void OnEnable()
		{
			if (readme == null)
			{
				readme = (Readme)target;
			}
		}

		protected override void OnHeaderGUI()
		{
			Init();
			float iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth/3f - 20f, readme.iconMaxWidth);
			GUILayout.BeginHorizontal("In BigTitle");
			{
				GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
				GUILayout.Label(readme.title, TitleStyle);
			}
			GUILayout.EndHorizontal();
		}
	
		public override void OnInspectorGUI()
		{
			if (readme == null)
			{
				return;
			}
			Init();
			foreach (Readme.Section section in readme.sections)
			{
				if (!string.IsNullOrEmpty(section.heading))
				{
					GUILayout.Label(section.heading, HeadingStyle);
				}
				if (!string.IsNullOrEmpty(section.text))
				{
					GUILayout.Label(section.text, BodyStyle);
				}
				if (!string.IsNullOrEmpty(section.linkText))
				{
					GUILayout.Space(K_INDENT_SPACE / 2);
					if (LinkLabel(new GUIContent(section.linkText)))
					{
						Application.OpenURL(section.url);
					}
				}
				if (section.Image != null)
				{
					GUILayout.Space(K_INDENT_SPACE / 2);
					GUILayout.Label(section.Image, GUILayout.Width(section.ImageWidth), GUILayout.Height(section.ImageHeight));
				}
				GUILayout.Space(K_INDENT_SPACE);
			}
		}

		private bool m_Initialized;

		private GUIStyle LinkStyle => m_LinkStyle;

		[SerializeField]
		private GUIStyle m_LinkStyle;

		private GUIStyle TitleStyle => m_TitleStyle;

		[SerializeField]
		private GUIStyle m_TitleStyle;

		private GUIStyle HeadingStyle => m_HeadingStyle;

		[SerializeField]
		private GUIStyle m_HeadingStyle;

		private GUIStyle BodyStyle => m_BodyStyle;

		[SerializeField]
		private GUIStyle m_BodyStyle;

		private void Init()
		{
			if (m_Initialized)
			{
				return;
			}
			m_BodyStyle = new GUIStyle(EditorStyles.label);
			m_BodyStyle.wordWrap = true;
			m_BodyStyle.fontSize = 14;
		
			m_TitleStyle = new GUIStyle(m_BodyStyle);
			m_TitleStyle.fontSize = 26;

			m_HeadingStyle = new GUIStyle(m_BodyStyle);
			m_HeadingStyle.fontSize = 18;
			m_HeadingStyle.fontStyle = FontStyle.Bold;
		
			m_LinkStyle = new GUIStyle(m_BodyStyle);
			m_LinkStyle.normal.textColor = new Color (0x00/255f, 0x78/255f, 0xDA/255f, 1f);
			m_LinkStyle.stretchWidth = false;
		
			m_Initialized = true;
		}

		private bool LinkLabel (GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = GUILayoutUtility.GetRect(label, LinkStyle, options);

			Handles.BeginGUI ();
			Handles.color = LinkStyle.normal.textColor;
			Handles.DrawLine (new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
			Handles.color = Color.white;
			Handles.EndGUI ();

			EditorGUIUtility.AddCursorRect (position, MouseCursor.Link);

			return GUI.Button (position, label, LinkStyle);
		}
	}
}
#endif

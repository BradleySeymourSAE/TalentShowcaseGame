using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Common;
using UnityEditor;
using UnityEngine;

namespace Framework.Scripts.Common.TriggerToolkit
{
    public class TriggerToolkit : MonoBehaviour
    {
        [SerializeField] protected List<TriggerCondition> Conditions = new(); 
        [SerializeField] protected Collider2D m_Collider;
        [SerializeField] protected List<Collider2D> m_Colliders = new();
        [SerializeField] protected bool UseVerboseLogging;
        public event Action<Collider2D, TriggerCondition.ETriggerEventType> OnTriggerEvent = delegate { };

        [SerializeField, Tooltip("If true, will use OnCollision events as well as OnTrigger events")]
        private bool UseCollisionEvents = false;

        [SerializeField, Tooltip("Whether or not to use a layer filter when checking incoming events")]
        private bool UseLayerFilter;

        [SerializeField,Tooltip("Layer's to check against when UseLayerFilter is true")]
        private LayerMask IncludedLayers;

        /// <summary>
        ///     Get's the collider component that's attached to this game object
        /// </summary>
        public Collider2D Collider
        {
            get
            {
                return m_Collider;
            }
        }
        /// <summary>
        ///     Get's all colliders that are attached to this game object that aren't the trigger collider
        /// </summary>
        public List<Collider2D> AllColliders
        {
            get
            {
                return m_Colliders;
            }
        }

        public List<TriggerCondition> GetTriggerConditions()
        {
            return Conditions;
        }

        protected virtual void Awake()
        {
            int layer = LayerMask.NameToLayer("Sensors");
            if (gameObject.HasLayerMask(layer) == false)
            {
                gameObject.layer = layer; 
            }
            AssignColliderComponents();

        }

        protected void AssignColliderComponents()
        {
            m_Collider = GetComponent<Collider2D>();
            // triggers on concave colliders don't work,
            if (m_Collider == null)
            {
                console.error(this,"No collider component found on", gameObject.name);
                return;
            }
            m_Collider.isTrigger = true;
            if (m_Colliders == null || m_Colliders.Count == 0) 
            {
                Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
                m_Colliders = colliders.Where(c => c != m_Collider).ToList();
            }
        }

        protected virtual void Start()
        {
            if (Conditions != null && Conditions.Count > 0) 
            {
                foreach (TriggerCondition condition in Conditions)
                {
                    condition.OnTriggerAction += OnTriggerEvent;
                }
            }
        }

        protected virtual void OnActionTriggered(Collider2D OtherCollider, TriggerCondition.ETriggerEventType TriggerCondition)
        {

        }

        protected virtual void OnDestroy()
        {
            if (Conditions != null && Conditions.Count > 0) 
            {
                foreach (TriggerCondition condition in Conditions)
                {
                    condition.OnTriggerAction -= OnTriggerEvent;
                }
            }
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (UseLayerFilter && other.gameObject.HasLayerMask(IncludedLayers) == false)
            {
                if (UseVerboseLogging)
                {
                    console.log(this, "Trigger entered but layer was not included in filter", other.gameObject.name);
                }
                return;
            }

            for (int index = 0; index < Conditions.Count; ++index)
            {
                if (UseVerboseLogging)
                {
                    console.log(this, "Condition", Conditions[index], "was triggered (ON ENTER) - Invoking OnTriggerEnter and OnActionTriggered()");
                }
                Conditions[index].HandleOnEnter(other, TriggerCondition.ETriggerEventType.ENTER);
                OnActionTriggered(other, TriggerCondition.ETriggerEventType.ENTER);
            }
        }

        protected void OnTriggerStay2D(Collider2D other)
        {
            if (UseLayerFilter &&  other.gameObject.HasLayerMask(IncludedLayers) == false)
            {
                if (UseVerboseLogging)
                {
                    console.log(this, "Trigger stay invoked but layer was not included in filter", other.gameObject.name);
                }
                return;
            }
            for (int index = 0; index < Conditions.Count; ++index)
            {
                if (UseVerboseLogging)
                {
                    console.log(this, "Condition", Conditions[index], "was triggered (ON STAY) - Invoking OnTriggerStay and OnActionTriggered()");
                }
                Conditions[index].HandleOnStay(other, TriggerCondition.ETriggerEventType.STAY);
                OnActionTriggered(other, TriggerCondition.ETriggerEventType.STAY);
            }
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (UseLayerFilter &&  other.gameObject.HasLayerMask(IncludedLayers) == false)
            {
                if (UseVerboseLogging)
                {
                    console.log(this, "Trigger exited but layer was not included in filter", other.gameObject.name);
                }
                return;
            }
            for (int index = 0; index < Conditions.Count; ++index)
            {
                if (UseVerboseLogging)
                {
                    console.log(this, "Condition", Conditions[index], "was triggered. Invoking OnTriggerExit and OnActionTriggered()");
                }
                Conditions[index].HandleOnExit(other, TriggerCondition.ETriggerEventType.EXIT);
                OnActionTriggered(other, TriggerCondition.ETriggerEventType.EXIT);
            }
        }

        protected void OnCollisionEnter2D(Collision2D Other)
        {
            if (UseLayerFilter && Other.gameObject.HasLayerMask(IncludedLayers) == false)
            {
                return;
            }
            if (UseCollisionEvents)
            {
                for (int index = 0; index < Conditions.Count; ++index)
                {
                    Conditions[index].HandleOnEnter(Other.collider, TriggerCondition.ETriggerEventType.ENTER);
                    OnActionTriggered(Other.collider, TriggerCondition.ETriggerEventType.ENTER);
                }
            }
        }

        protected void OnCollisionStay2D(Collision2D Other)
        {
            if (UseLayerFilter && Other.gameObject.HasLayerMask(IncludedLayers) == false)
            {
                return;
            }
            if (UseCollisionEvents)
            {
                for (int index = 0; index < Conditions.Count; ++index)
                {
                    Conditions[index].HandleOnStay(Other.collider, TriggerCondition.ETriggerEventType.ENTER);
                    OnActionTriggered(Other.collider, TriggerCondition.ETriggerEventType.ENTER);
                }
            }
        }

        protected void OnCollisionExit2D(Collision2D Other)
        {
            if (UseLayerFilter && Other.gameObject.HasLayerMask(IncludedLayers) == false)
            {
                return;
            }
            if (UseCollisionEvents)
            {
                for (int index = 0; index < Conditions.Count; ++index)
                {
                    Conditions[index].HandleOnExit(Other.collider, TriggerCondition.ETriggerEventType.ENTER);
                    OnActionTriggered(Other.collider, TriggerCondition.ETriggerEventType.ENTER);
                }
            }
        }

        #if UNITY_EDITOR
        protected internal void CheckTriggerConditions()
        {
            if (Conditions != null && Conditions.Count > 0) 
            {
                Conditions.ForEach(condition => condition.DebugLogTriggerConditionTags());
            }
            console.log(this, "Included Layers:", IncludedLayers);
            console.log(this, "Use Collision Events:", UseCollisionEvents);
            console.log(this, "Use Layer Filter: ", UseLayerFilter);

            AssignColliderComponents();
        }
        #endif
    }

    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(TriggerToolkit), true)]
    public class TriggerToolkitEditorInspector : UnityEditor.Editor
    {
        private SerializedProperty sp_Conditions;
        private SerializedProperty sp_Collider;
        private SerializedProperty sp_Colliders;
        private SerializedProperty sp_UseVerboseLogging;

        private SerializedProperty sp_UseCollisionEvents;
        private SerializedProperty sp_UseLayerFilter;
        private SerializedProperty sp_IncludedLayers;

        private TriggerToolkit toolkit;
        private LayerMask m_LayerMask;
        private bool m_LayersCanInteract;


        protected virtual void OnEnable()
        {
            if (toolkit == null)
            {
                toolkit = (TriggerToolkit)target;
            }
        }

        public override void OnInspectorGUI()
        {
            if (toolkit == null)
            {
                return;
            }
            serializedObject.Update();

            sp_Conditions = serializedObject.FindProperty("Conditions");
            sp_Collider = serializedObject.FindProperty("m_Collider");
            sp_Colliders = serializedObject.FindProperty("m_Colliders");
            sp_UseVerboseLogging = serializedObject.FindProperty("UseVerboseLogging");
            sp_UseCollisionEvents = serializedObject.FindProperty("UseCollisionEvents");
            sp_UseLayerFilter = serializedObject.FindProperty("UseLayerFilter");
            sp_IncludedLayers = serializedObject.FindProperty("IncludedLayers");

            // properties
            EditorGUILayout.PropertyField(sp_Collider, new GUIContent("Trigger Collider"));
            EditorGUILayout.PropertyField(sp_Colliders, new GUIContent("All Attached Colliders (Excluding Trigger)"), true);
            EditorGUILayout.PropertyField(sp_UseLayerFilter, new GUIContent("Use Layer Mask Filter"));
            EditorGUILayout.PropertyField(sp_UseCollisionEvents, new GUIContent("Use Collision Events"));
            EditorGUILayout.PropertyField(sp_UseVerboseLogging, new GUIContent("Use Verbose Logging"));
            if (sp_UseLayerFilter.boolValue)
            {
                EditorGUILayout.PropertyField(sp_IncludedLayers, true);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(sp_Conditions,new GUIContent("Event Conditions"), true);

            // check trigger conditions
            EditorGUILayout.Separator();
            if (GUILayout.Button("Check Trigger Conditions"))
            {
                toolkit?.CheckTriggerConditions();
            }

            string mask = LayerMask.LayerToName(toolkit.gameObject.layer);
            int sensorsMask = LayerMask.NameToLayer("Sensors");
            string requiredMask = LayerMask.LayerToName(sensorsMask);

            EditorGUILayout.HelpBox($"Make sure that the layer of this object is set to Sensors! Otherwise, the player will not be able to interact with the trigger...", MessageType.Info);
            EditorGUILayout.LabelField("Current Layer Mask", mask, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Required Layer Mask", requiredMask, EditorStyles.boldLabel);


            serializedObject.ApplyModifiedProperties();
            Repaint();
        }
    }
    #endif

}
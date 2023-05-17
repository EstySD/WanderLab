using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CMR
{
    [CustomEditor(typeof(ConvexDecompositionSettings))]
    [CanEditMultipleObjects]
    public class ConvexDecompositionEditor : Editor
    {
        private static class Styles
        {
            public static GUIContent presetIcon = EditorGUIUtility.IconContent("Preset.Context");
            public static GUIStyle iconButton = new GUIStyle("IconButton");
        }

        public GameObject selectedObject;
        public Mesh inputMesh;
        public PhysicMaterial physicMaterial;

        bool m_StylesInitialized;
        GUIContent m_EnableOpenCLLabel;
        GUIStyle m_FoldoutStyle;
        GUIStyle m_HorizontalLine;
        Color m_HorizontalLineColor;

        ConvexDecompositionSettings m_Settings;
        bool m_HasOpenCL;
        List<string> m_OpenCLPlatforms;
        List<string> m_OpenCLDevices;

        bool m_HullFoldout = true;
        bool m_AdvancedFoldout;
        bool m_OpenCLFoldout = true;
        bool m_SettingsFoldout;

        SerializedProperty m_CreateAsset;
        SerializedProperty m_CreateColliders;
        SerializedProperty m_CreateMeshRenderers;
        SerializedProperty m_Resolution;
        SerializedProperty m_MaxHulls;
        SerializedProperty m_MaxConcavity;
        SerializedProperty m_PlaneDownsampling;
        SerializedProperty m_HullDownsampling;
        SerializedProperty m_SymmetryPlaneBias;
        SerializedProperty m_RevolutionAxesBias;
        SerializedProperty m_MeshNormalization;
        SerializedProperty m_TetrahedronMode;
        SerializedProperty m_MaxVertices;
        SerializedProperty m_MinVolume;
        SerializedProperty m_ApproximateHulls;
        SerializedProperty m_ProjectVertices;
        SerializedProperty m_EnableOpenCL;
        SerializedProperty m_OpenCLPlatformID;
        SerializedProperty m_OpenCLDeviceID;

        void OnEnable()
        {
            m_Settings = (ConvexDecompositionSettings)target;

            if (selectedObject == null)
            {
                selectedObject = Selection.activeGameObject;
            }

            if (selectedObject != null && physicMaterial == null)
            {
                var collider = selectedObject.GetComponent<Collider>();
                if (collider != null)
                {
                    physicMaterial = collider.sharedMaterial;
                }
            }

            m_OpenCLPlatforms = VHCDAPI.GetPlatforms();
            m_OpenCLDevices = m_OpenCLPlatforms.Count > 0 ? VHCDAPI.GetDevices(0) : new List<string>();

            m_CreateAsset = serializedObject.FindProperty("m_CreateAsset");
            m_CreateColliders = serializedObject.FindProperty("m_CreateColliders");
            m_CreateMeshRenderers = serializedObject.FindProperty("m_CreateMeshRenderers");
            m_Resolution = serializedObject.FindProperty("m_Resolution");
            m_MaxHulls = serializedObject.FindProperty("m_MaxHulls");
            m_MaxConcavity = serializedObject.FindProperty("m_MaxConcavity");
            m_PlaneDownsampling = serializedObject.FindProperty("m_PlaneDownsampling");
            m_HullDownsampling = serializedObject.FindProperty("m_HullDownsampling");
            m_SymmetryPlaneBias = serializedObject.FindProperty("m_SymmetryPlaneBias");
            m_RevolutionAxesBias = serializedObject.FindProperty("m_RevolutionAxesBias");
            m_MeshNormalization = serializedObject.FindProperty("m_MeshNormalization");
            m_TetrahedronMode = serializedObject.FindProperty("m_TetrahedronMode");
            m_MaxVertices = serializedObject.FindProperty("m_MaxVertices");
            m_MinVolume = serializedObject.FindProperty("m_MinVolume");
            m_ApproximateHulls = serializedObject.FindProperty("m_ApproximateHulls");
            m_ProjectVertices = serializedObject.FindProperty("m_ProjectVertices");
            m_EnableOpenCL = serializedObject.FindProperty("m_EnableOpenCL");
            m_OpenCLPlatformID = serializedObject.FindProperty("m_OpenCLPlatformID");
            m_OpenCLDeviceID = serializedObject.FindProperty("m_OpenCLDeviceID");

            // Check if OpenCL is supported
            m_HasOpenCL = m_OpenCLPlatforms.Count > 0 && m_OpenCLDevices.Count > 0;
            if (m_EnableOpenCL.boolValue && !m_HasOpenCL)
            {
                m_EnableOpenCL.boolValue = false;
            }

            m_OpenCLPlatformID.intValue = Math.Max(0, Math.Min(m_OpenCLPlatformID.intValue, m_OpenCLPlatforms.Count - 1));
            m_OpenCLDeviceID.intValue = Math.Max(0, Math.Min(m_OpenCLDeviceID.intValue, m_OpenCLDevices.Count - 1));
        }

        public override void OnInspectorGUI()
        {
            if (!m_StylesInitialized)
            {
                m_StylesInitialized = true;

                m_EnableOpenCLLabel = new GUIContent("Enabled");

                m_FoldoutStyle = new GUIStyle(EditorStyles.foldout);
                m_FoldoutStyle.fontStyle = FontStyle.Bold;

                m_HorizontalLine = new GUIStyle();
                m_HorizontalLine.normal.background = EditorGUIUtility.whiteTexture;
                m_HorizontalLine.margin = new RectOffset(0, 0, 4, 4);
                m_HorizontalLine.fixedHeight = 1;

                m_HorizontalLineColor = new Color(0.4f, 0.4f, 0.4f);
            }

            serializedObject.Update();

            EditorGUILayout.Separator();
            selectedObject = (GameObject)EditorGUILayout.ObjectField("Input GameObject",
                selectedObject, typeof(GameObject), true);

            inputMesh = null;
            if (selectedObject != null)
            {
                MeshFilter meshFilter = selectedObject.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    inputMesh = meshFilter.sharedMesh;
                }
            }

            if (selectedObject == null)
            {
                EditorGUILayout.HelpBox("Select an input GameObject", MessageType.Info);
            }
            else if (inputMesh == null)
            {
                EditorGUILayout.HelpBox("Selected GameObject does not have a mesh", MessageType.Warning);
            }

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_CreateColliders);

            if (m_CreateColliders.boolValue)
            {
                physicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Physic Material",
                    physicMaterial, typeof(PhysicMaterial), true);
            }

            EditorGUILayout.PropertyField(m_CreateMeshRenderers);
            EditorGUILayout.PropertyField(m_CreateAsset);

            EditorGUILayout.Separator();
            HorizontalRule();
            m_HullFoldout = EditorGUILayout.Foldout(m_HullFoldout, "Convex Hulls", m_FoldoutStyle);
            if (m_HullFoldout)
            {
                EditorGUILayout.PropertyField(m_MaxHulls);
                EditorGUILayout.PropertyField(m_MaxVertices);
            }

            HorizontalRule();
            m_AdvancedFoldout = EditorGUILayout.Foldout(m_AdvancedFoldout, "Advanced", m_FoldoutStyle);
            if (m_AdvancedFoldout)
            {
                EditorGUILayout.PropertyField(m_Resolution);
                EditorGUILayout.PropertyField(m_MaxConcavity);
                EditorGUILayout.PropertyField(m_PlaneDownsampling);
                EditorGUILayout.PropertyField(m_HullDownsampling);
                EditorGUILayout.PropertyField(m_SymmetryPlaneBias);
                EditorGUILayout.PropertyField(m_RevolutionAxesBias);
                EditorGUILayout.PropertyField(m_MeshNormalization);
                EditorGUILayout.PropertyField(m_TetrahedronMode);
                EditorGUILayout.PropertyField(m_MinVolume);
                EditorGUILayout.PropertyField(m_ApproximateHulls);
                EditorGUILayout.PropertyField(m_ProjectVertices);
            }

            if (m_HasOpenCL)
            {
                HorizontalRule();
                m_OpenCLFoldout = EditorGUILayout.Foldout(m_OpenCLFoldout, "OpenCL", m_FoldoutStyle);
                if (m_OpenCLFoldout)
                {
                    EditorGUILayout.PropertyField(m_EnableOpenCL, m_EnableOpenCLLabel);

                    int platformID = EditorGUILayout.Popup("Platform",
                        m_OpenCLPlatformID.intValue, m_OpenCLPlatforms.ToArray());
                    if (platformID != m_OpenCLPlatformID.intValue)
                    {
                        m_OpenCLDevices = m_OpenCLPlatforms.Count > 0 ? VHCDAPI.GetDevices(0) : new List<string>();
                        m_OpenCLDeviceID.intValue = 0;
                    }

                    m_OpenCLDeviceID.intValue = EditorGUILayout.Popup("Device",
                            m_OpenCLDeviceID.intValue, m_OpenCLDevices.ToArray());
                }
            }

            serializedObject.ApplyModifiedProperties();

            HorizontalRule();
            m_SettingsFoldout = EditorGUILayout.Foldout(m_SettingsFoldout, "Settings", m_FoldoutStyle);
            if (m_SettingsFoldout)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Clear Settings");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Reset"))
                {
                    var window = EditorWindow.GetWindow<ConvexDecompositionEditorWindow>();
                    window.ResetPreferences(m_Settings);
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Save/Load Settings");
                GUILayout.FlexibleSpace();
                var buttonPosition = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, Styles.iconButton);
                if (EditorGUI.DropdownButton(buttonPosition, Styles.presetIcon, FocusType.Passive, Styles.iconButton))
                {
                    var window = EditorWindow.GetWindow<ConvexDecompositionEditorWindow>();
                    window.ShowPresets();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void HorizontalRule()
        {
            Color origColor = GUI.color;
            GUI.color = m_HorizontalLineColor;
            GUILayout.Box(GUIContent.none, m_HorizontalLine);
            GUI.color = origColor;
        }
    }
}

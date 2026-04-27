using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EB.DOTS.HSM.Editor
{
    [CustomEditor(typeof(HsmBoot))]
    public class HsmBootEditor : UnityEditor.Editor
    {
        private List<Type> _stateTypes = new ();
        private string[] _stateTypeNames = Array.Empty<string>();
        private SerializedProperty _selectedStateTypeNameProp;

        private void OnEnable()
        {
            _selectedStateTypeNameProp = serializedObject.FindProperty("selectedStateTypeName");

            _stateTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => {
                    try { return a.GetTypes(); } 
                    catch { return Type.EmptyTypes; }
                })
                .Where(t => typeof(ISubState<HsmRoot>).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                .OrderBy(t => t.Name)
                .ToList();

            _stateTypeNames = _stateTypes.Select(t => t.FullName).ToArray();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string currentTypeName = _selectedStateTypeNameProp.stringValue;
            int selectedIndex = -1;

            if (!string.IsNullOrEmpty(currentTypeName))
            {
                selectedIndex = _stateTypes.FindIndex(t => t.AssemblyQualifiedName == currentTypeName);
            }

            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUILayout.Popup("Initial State", selectedIndex, _stateTypeNames);
            if (EditorGUI.EndChangeCheck())
            {
                if (selectedIndex >= 0 && selectedIndex < _stateTypes.Count)
                {
                    _selectedStateTypeNameProp.stringValue = _stateTypes[selectedIndex].AssemblyQualifiedName;
                }
                else
                {
                    _selectedStateTypeNameProp.stringValue = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(_selectedStateTypeNameProp.stringValue))
            {
                EditorGUILayout.HelpBox("Please select an initial HSM state.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

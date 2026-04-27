using System;
using UnityEngine;

namespace EB.DOTS.HSM
{
    public class HsmBoot : MonoBehaviour
    {
        [SerializeField, HideInInspector] 
        private string selectedStateTypeName;

        private void Awake()
        {
            if (HsmTools.IsHsmRunning())
            {
                return; // ignore
            }
            
            if (string.IsNullOrEmpty(selectedStateTypeName))
            {
                Debug.LogError("No HSM state selected in HsmStateLoader.");
                return;
            }

            Type type = Type.GetType(selectedStateTypeName);
            if (type == null)
            {
                Debug.LogError($"Could not find HSM state type: {selectedStateTypeName}");
                return;
            }

            try
            {
                if (Activator.CreateInstance(type) is ISubState<HsmRoot> state)
                {
                    HsmTools.InitHsm(state);
                }
                else
                {
                    Debug.LogError($"Type {selectedStateTypeName} is not a valid ISubState<HsmRoot>.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to instantiate HSM state type: {selectedStateTypeName}. Make sure it has a parameterless constructor. Error: {ex.Message}");
            }
        }

#if UNITY_EDITOR
        public string SelectedStateTypeName
        {
            get => selectedStateTypeName;
            set => selectedStateTypeName = value;
        }
#endif
    }
}

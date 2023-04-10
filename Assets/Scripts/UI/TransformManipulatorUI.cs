using RuntimeHandle;
using UnityEngine;

namespace Campaigner.UI
{
    public class TransformManipulatorUI : MonoBehaviour
    {
        
        [SerializeField] private GameObject transformManipulatorUI;


        private void Start()
        {
            TransformManipulatorManager.Instance.OnTransformHandleEnabled += EnableUI;
            TransformManipulatorManager.Instance.OnTransformHandleDisabled += DisableUI;
        }

        private void EnableUI()
        {
            transformManipulatorUI.SetActive(true);   
        }
        
        private void DisableUI()
        {
            transformManipulatorUI.SetActive(false);
        }

        public void MoveButtonClick()
        {
            TransformManipulatorManager.Instance.SetHandleType(HandleType.POSITION);
        }
        
        public void RotateButtonClick()
        {
            TransformManipulatorManager.Instance.SetHandleType(HandleType.ROTATION);
        }
        
        public void ScaleButtonClick()
        {
            TransformManipulatorManager.Instance.SetHandleType(HandleType.SCALE);
        }

        private void OnDestroy()
        {
            TransformManipulatorManager.Instance.OnTransformHandleEnabled -= EnableUI;
            TransformManipulatorManager.Instance.OnTransformHandleDisabled -= DisableUI;
        }
    }
}



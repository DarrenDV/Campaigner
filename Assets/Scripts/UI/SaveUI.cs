using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Campaigner.UI
{
    public class SaveUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private GameObject saveUIObject;

        [SerializeField] private GameObject parent;
        
        private void Start()
        {
            if (parent == null)
            {
                parent = BuildingManager.Instance.GetParent();
            }
            
            GameUIManager.Instance.OnGameMenuStateChanged += MenuStateSwitched;
        }

        private void MenuStateSwitched(GameMenuState gameMenuState)
        {
            if (gameMenuState != GameMenuState.SaveMenu)
            {
                saveUIObject.gameObject.SetActive(false);
                return;
            }
            
            saveUIObject.gameObject.SetActive(true);
        }


        public void SaveButtonClick()
        {
            if (SavingUtils.FileExists(inputField.text))
            {
                //TODO: Add a warning that the file already exists
                return;
            }
            
            SavingLoading.SavingLoading.Save(inputField.text, parent);
            GameUIManager.Instance.GameMenuState = GameMenuState.EscMenu;
        }

        private void OnDestroy()
        {
            GameUIManager.Instance.OnGameMenuStateChanged -= MenuStateSwitched;
        }
    }
}
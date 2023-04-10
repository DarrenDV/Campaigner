using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Campaigner.UI
{
    public class LoadUI : MonoBehaviour
    {
        [SerializeField] private GameObject loadingUI;
        [SerializeField] private TMP_Dropdown dropdown;
        
        [SerializeField] private const string FIRST_OPTION_TEXT = "Select a save file";

        private void Start()
        {
            GameUIManager.Instance.OnGameMenuStateChanged += MenuStateSwitched;
        }
        
        private void MenuStateSwitched(GameMenuState gameMenuState)
        {
            if (gameMenuState != GameMenuState.LoadMenu)
            {
                loadingUI.gameObject.SetActive(false);
                return;
            }
            
            UpdateDropDown();
            loadingUI.gameObject.SetActive(true);
        }

        public void LoadButtonClick()
        {
            if (dropdown.value == 0)
            {
                return;
            }

            WorldInfo.Instance.OnLoad();
            SavingLoading.SavingLoading.Load(dropdown.options[dropdown.value].text);
            GameUIManager.Instance.GameMenuState = GameMenuState.EscMenu;
        }

        private void OnDestroy()
        {
            GameUIManager.Instance.OnGameMenuStateChanged -= MenuStateSwitched;
        }

        private void UpdateDropDown()
        {
            dropdown.options.Clear();
            
            TMP_Dropdown.OptionData firstOption = new TMP_Dropdown.OptionData();
            firstOption.text = FIRST_OPTION_TEXT;
            dropdown.options.Add(firstOption);
        
            foreach(string saveFile in new SavingUtils().GetSaveFiles())
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
                optionData.text = saveFile;
                dropdown.options.Add(optionData);
            }
        }
    }
}
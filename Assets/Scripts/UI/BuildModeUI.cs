using System;
using UnityEngine;

namespace Campaigner.UI
{
    public class BuildModeUI : MonoBehaviour
    {
        [SerializeField] private GameObject buildModeUI;

        private void Start()
        {
            GameUIManager.Instance.OnGameMenuStateChanged += MenuStateSwitched;
        }
        
        private void MenuStateSwitched(GameMenuState gameMenuState)
        {
            if (gameMenuState != GameMenuState.BaseView)
            {
                buildModeUI.SetActive(false);
                return;
            }
            
            buildModeUI.SetActive(true);
        }
        
        public void BuildModeButtonClicked(int menuState)
        {
            GameUIManager.Instance.GameMenuState = (GameMenuState)menuState;
        }

        private void OnDestroy()
        {
            GameUIManager.Instance.OnGameMenuStateChanged -= MenuStateSwitched;
        }
    }
}
using System;
using UnityEngine;

namespace Campaigner.UI
{
    public class PlaceObjectsUI : MonoBehaviour
    {
        [SerializeField] private GameObject placeObjectsUI;

        private void Start()
        {
            GameUIManager.Instance.OnGameMenuStateChanged += MenuStateSwitched;
        }
            
        private void MenuStateSwitched(GameMenuState gameMenuState)
        {
            if (gameMenuState != GameMenuState.PlaceObjectUI)
            {
                placeObjectsUI.gameObject.SetActive(false);
                return;
            }
            
            placeObjectsUI.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            GameUIManager.Instance.OnGameMenuStateChanged -= MenuStateSwitched;
        }
    }
}
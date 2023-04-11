using Campaigner.UI;
using UnityEngine;
using UnityEngine.UI;

public class EscMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject escMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;
    
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    
    private void Start()
    {
        if (escMenu == null)
        {
            escMenu = GameObject.Find("EscMenu");
        }
        
        GameUIManager.Instance.OnGameMenuStateChanged += MenuStateSwitched;
        
    }
    
    private void MenuStateSwitched(GameMenuState gameMenuState)
    {
        if (gameMenuState != GameMenuState.EscMenu)
        {
            escMenu.SetActive(false);
            return;
        }
        
        escMenu.SetActive(true);
    }

    public void EscButtonClicked(int menuState)
    {
        GameUIManager.Instance.GameMenuState = (GameMenuState)menuState;
    }

    public void BackToMainMenu()
    {
        if (SteamLobby.Instance != null)
        {
            SteamLobby.Instance.LeaveLobby();
        }
        
        GameUIManager.Instance.BackToMainMenu();
    }

    private void OnDestroy()
    {
        GameUIManager.Instance.OnGameMenuStateChanged -= MenuStateSwitched;
    }
    
}

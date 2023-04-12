using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        FolderStructure();
    }

    private void FolderStructure()
    {
        if (!Directory.Exists(Application.dataPath + "/SaveData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/SaveData");
        }
    }

    public void HostButtonClick()
    {
        SteamLobby.Instance.HostLobby();
    }
}

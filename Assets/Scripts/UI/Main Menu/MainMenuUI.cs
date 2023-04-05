using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MainMenuUI : MonoBehaviour
{
    public void OnSceneButtonClicked(string sceneName)
    {
        NetworkManager.singleton.ServerChangeScene(sceneName);
    }
}

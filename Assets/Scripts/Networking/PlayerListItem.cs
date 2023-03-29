using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    /*
     *  This class is used for the player list items in the lobby scene
     *  It will be used to display the player's name and avatar
     *
     *  I am not sure if I want to keep the avatar part incorporated, removing it would make the script a lot simpler
     * 
     */
    
    
    public string PlayerName;
    public int ConnectionID;

    public TextMeshProUGUI PlayerNameText;

    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
    }
}

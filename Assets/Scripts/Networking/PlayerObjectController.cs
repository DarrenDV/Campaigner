using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    /*
     *  This class is the "Local Player" object containing data for the local player
     *  The main goal of this class is to store data about the player.
     *  This data will be used for interactions in the game and determining what the player can do.
     *
     *  Currently this class only works in the Lobby scene and is destroyed afterwards
     *
     *  TODO: Make this class work in the game scene as well    
     *  TODO: Rework this class 
     */
    
    
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerID;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    
    [SyncVar] public PlayerType PlayerType;

    [SyncVar] public string name;
    
    
    private CustomNetworkManager networkManager;
    
    private CustomNetworkManager NetworkManager
    {
        get
        {
            if (networkManager != null) { return networkManager; }
            return networkManager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }
    
    
    
    public override void OnStartAuthority()
    {
        DontDestroyOnLoad(this);
        
        
        CmdUpdatePlayerName(SteamFriends.GetPersonaName());
        name = SteamFriends.GetPersonaName();
        gameObject.name = "LocalGamePlayer"; //TODO Change to SteamFriends.GetPersonaName()
        //LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.LocalPlayerController = this;
        LobbyController.Instance.UpdateUIElements();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        NetworkManager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        //LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        NetworkManager.GamePlayers.Remove(this);
        //LobbyController.Instance.UpdatePlayerList();
    }
    
    [Command]
    private void CmdUpdatePlayerName(string newName)
    {
        this.PlayerNameUpdate(PlayerName, newName);
    }

    public void PlayerNameUpdate(string old, string newValue)
    {
        if (isServer)
        {
            this.PlayerName = newValue;
        }

        if (isClient)
        {
            //LobbyController.Instance.UpdatePlayerList();
            
            //Extremely scuffed way to force a refresh of the player list
            NetworkManager.GamePlayers.Add(this);
            NetworkManager.GamePlayers.RemoveAt(NetworkManager.GamePlayers.Count - 1);
        }
    }
    
}

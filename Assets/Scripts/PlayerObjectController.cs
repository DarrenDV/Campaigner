using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerID;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    
    [SyncVar] public PlayerType PlayerType;
    
    
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
        CmdUpdatePlayerName(SteamFriends.GetPersonaName());
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

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
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Debug.Log("Hello");
        NetworkManager.Players.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Debug.Log("OPEow[qe");
        NetworkManager.Players.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
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
            LobbyController.Instance.UpdatePlayerList();
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using TMPro;

public class SteamLobby : MonoBehaviour
{
    /*
     *  This script is used to create a lobby and join a lobby using steam
     *  This is far from the final version of the script and will definitely be looked at when the networking rework happens
     *
     * 
     */
    
    
    public static SteamLobby Instance;
    
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    public ulong lobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager networkManager;

    private void Start()
    {
        if(!SteamManager.Initialized)
        {
            return;
        }
        
        if(Instance == null)
        {
            Instance = this;
        }
        
        networkManager = GetComponent<CustomNetworkManager>();
        
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }
    
    
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }
        
        Debug.Log("Lobby created!");
        
        networkManager.StartHost();
        
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        
    }
    
    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Join request!");
        
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        lobbyID = callback.m_ulSteamIDLobby;

        if(NetworkServer.active)
        {
            return;
        }
        
        networkManager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        
        networkManager.StartClient();
    }
    
    public void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(lobbyID));
    }
    
}

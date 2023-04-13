using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
    protected Callback<SteamNetConnectionStatusChangedCallback_t> connectionStatusChanged;

    public ulong lobbyID;
    private const string HostAddressKey = "HostAddress";
    
    [SerializeField] private bool debugMode = true;

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
        
        
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        connectionStatusChanged = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(ConnectionStatusChanged);
        
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        if (debugMode)
        {
            Debug.Log("Lobby created!");
        }
        
        CustomNetworkManager.Instance.StartHost();
        
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");

        if (debugMode)
        {
            Debug.Log("Lobby created with id: " + callback.m_ulSteamIDLobby);
        }
        
    }
    
    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, CustomNetworkManager.Instance.maxConnections);
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        if (debugMode)
        {
            Debug.Log("Join request!");
        }
        
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    
    private void ConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t callback)
    {
        if (NetworkServer.active)
        {
            return;
        }


        if (debugMode)
        {
            Debug.Log("Connection status changed! It's now: " + callback.m_info.m_eState);
        }
        
        if(callback.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer || callback.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally || callback.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_None)
        {
            if (debugMode)
            {
                Debug.Log("Connection closed!");
            }
            
            LeaveLobby();
            
            if(NetworkServer.active)
            {
                CustomNetworkManager.Instance.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                CustomNetworkManager.Instance.StopClient();
            }

            if (SceneManager.GetActiveScene().ToString() != "MenuScene")
            {
                SceneManager.LoadScene("MenuScene");
            }
            
            CustomNetworkManager.Instance.Reset();
        }
        
        if(callback.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
        {
            if (debugMode)
            {
                Debug.Log("Connected!");
            }
        }
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (debugMode)
        {
            Debug.Log("Lobby entered!");
            Debug.Log("OnLobbyEntered: " + callback.m_ulSteamIDLobby);
        }
        
        lobbyID = callback.m_ulSteamIDLobby;

        if(NetworkServer.active)
        {
            return;
        }
        
        CustomNetworkManager.Instance.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        CustomNetworkManager.Instance.StartClient();
    }
    
    public void LeaveLobby()
    {

        if (debugMode)
        {
            Debug.Log("Leaving lobby!");
        }
        
        SteamMatchmaking.LeaveLobby(new CSteamID(lobbyID));
        
        
        if (SteamMatchmaking.GetLobbyOwner(new CSteamID(lobbyID)) == SteamUser.GetSteamID())
        {
            Debug.Log("Deleting lobby data!");
            SteamMatchmaking.DeleteLobbyData(new CSteamID(lobbyID), HostAddressKey);
        }

        
        
        //if(NetworkServer.active)
        //{
            //networkManager.StopHost();
            //CustomNetworkManager.Instance.StopHost();
        //}
        //else if(NetworkClient.isConnected)
        //{
            //networkManager.StopClient();
            //CustomNetworkManager.Instance.StopClient();
        //}
        
        
        //Destroy(this.gameObject);
        
        //CustomNetworkManager.Instance.Reset();
        
        
        
    }
    
}

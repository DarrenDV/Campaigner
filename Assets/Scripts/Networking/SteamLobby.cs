using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;


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

    /// <summary>
    /// Called when a lobby is created, starts the host and sets the lobby data, will only run for the host
    /// </summary>
    /// <param name="callback"></param>
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
    
    /// <summary>
    /// Host a lobby, tells steam to create a lobby
    /// </summary>
    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, CustomNetworkManager.Instance.maxConnections);
    }

    /// <summary>
    /// Called when a player presses the join lobby button or accepts an invite from the host
    /// </summary>
    /// <param name="callback"></param>
    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        if (debugMode)
        {
            Debug.Log("Join request!");
        }
        
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    
    /// <summary>
    /// Callback called every time a connection status changes in steam
    /// </summary>
    /// <param name="callback"></param>
    private void ConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t callback)
    {
        if (NetworkServer.active) //Only run this as a client
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
            
            if (NetworkClient.isConnected)
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

    /// <summary>
    /// Function called when steam accepted the lobby join request, starts the client and sets the network address
    /// </summary>
    /// <param name="callback"></param>
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
    
    /// <summary>
    /// Function called to leave a lobby in steam
    /// </summary>
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
    }
}
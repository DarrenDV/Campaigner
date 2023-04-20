# Networking

## Notable Scripts
- [NetworkManager](../../Assets/Scripts/Networking/CustomNetworkManager.cs)
- [LobbyController](../../Assets/Scripts/Networking/LobbyController.cs)
- [PlayerObjectController](../../Assets/Scripts/Networking/PlayerObjectController.cs)
- [SteamLobby](../../Assets/Scripts/Networking/SteamLobby.cs)

The networking system is a system made using Mirror and Steamworks. 
I use the steamworks.net package and make use of the FuzzySteamworks Transport layer.

<br>

Whenever a player either hosts a Lobby or wants to join a lobby, a callback from steam is sent that is handled in SteamLobby.
The callbacks used are LobbyCreated, LobbyJoinRequest, LobbyEntered and ConnectionStatusChanged.

<br>

Whilst most of the code for this system is quite standard, there was some trickery for making sure leaving a lobby went good. The problem was that whenever the host left a game, their client was essentially stuck in their lobby, not being able to leave it and making them join that same lobby whenever they wanted to host one.
<br>
The function ConnectionStatusChanged() in SteamLobby was used to solve this issue.
```csharp
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
```
This function checks what the received callback from the host is in order to leave when the host loses connection. This way the client is "reset" and they can safely join or host another lobby.
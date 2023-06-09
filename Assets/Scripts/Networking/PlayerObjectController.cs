using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

public class PlayerObjectController : NetworkBehaviour
{
    /*
     *  This class is the "Local Player" object containing data for the local player
     *  The main goal of this class is to store data about the player.
     *  This data will be used for interactions in the game and determining what the player can do.
     *
     *  Currently this class only works in the Lobby scene and is destroyed afterwards
     *
     *  TODO: Rework this class
     *  TODO: Decide if it's actually in need of a rework
     */
    
    
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerID;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    
    [SyncVar] public PlayerType PlayerType;

    public override void OnStartAuthority()
    {
        CmdUpdatePlayerName(SteamFriends.GetPersonaName());
        name = SteamFriends.GetPersonaName();
        gameObject.name = "LocalGamePlayer"; //TODO Change to SteamFriends.GetPersonaName() -- Actually maybe not 


        if (LobbyController.Instance != null)
        {
            LobbyController.Instance.LocalPlayerController = this;
        
            LobbyController.Instance.UpdateUIElements();
            LobbyController.Instance.UpdateLobbyName();
        }
        
        if(PlayerSteamID == SteamUser.GetSteamID().m_SteamID)
        {
            CustomNetworkManager.Instance.localPlayer = this;
        }
    }

    public override void OnStartClient()
    {
        //This is a scuffed way to make sure there are no null values in the list, this happens when the scene changes
        if (CustomNetworkManager.Instance.GamePlayers.Count > 0)
        {
            if (CustomNetworkManager.Instance.GamePlayers[0] == null)
            {
                CustomNetworkManager.Instance.GamePlayers.Clear();
            }
        }
        
        CustomNetworkManager.Instance.GamePlayers.Add(this);

        if (LobbyController.Instance != null)
        {
            LobbyController.Instance.UpdateLobbyName();
        }
    }

    public override void OnStopClient()
    {
        CustomNetworkManager.Instance.GamePlayers.Remove(this);
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
            //Extremely scuffed way to force a refresh of the player list
            CustomNetworkManager.Instance.GamePlayers.Add(this);
            CustomNetworkManager.Instance.GamePlayers.RemoveAt(CustomNetworkManager.Instance.GamePlayers.Count - 1);
        }
    }

    public void Quit()
    {
        //Set the offline scene to null
        CustomNetworkManager.Instance.offlineScene = "";

        //Make the active scene the offline scene
        SceneManager.LoadScene("MenuScene");

        //Leave Steam Lobby
        SteamLobby.Instance.LeaveLobby();

        if (isOwned)
        {
            if (isServer)
            {
                CustomNetworkManager.Instance.StopHost();
            }
            else
            {
                CustomNetworkManager.Instance.StopClient();
            }
        }
    }
    

}

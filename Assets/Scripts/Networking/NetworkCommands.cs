using Mirror;
using UnityEngine;

public class NetworkCommands : NetworkBehaviour
{
    /*
     * This class is used to send commands to the server
     * These commands are meant to be seperate from the classes that would be associated with the command
     *      e.g. The command to place an object should be in this class, not in the BuildingManager class
     * This is because I want these classes to be able to be used in singleplayer mode, and making them networked would make that impossible
     */
    
    public static NetworkCommands Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Places an object on the server at the given position, rotation and scale
    /// </summary>
    /// <param name="objectName">The name of the prefab to be placed</param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    [Command(requiresAuthority = false)]
    public void CmdPlaceItem(string objectName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        BuildingManager.Instance.PlaceObject(objectName, position, rotation, scale);
    }

    
    [ClientRpc]
    public void RpcItemSpawned(GameObject go)
    {
        if (isServer) return;
        go.GetComponent<PlacedObject>().SpawnSnappingPoints();
    }
    
}

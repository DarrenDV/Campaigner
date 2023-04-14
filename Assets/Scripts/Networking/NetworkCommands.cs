using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkCommands : NetworkBehaviour
{
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
    
    [Command(requiresAuthority = false)]
    public void CmdPlaceItem(string objectName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Debug.Log("Placing item");
        BuildingManager.Instance.PlaceObject(objectName, position, rotation, scale);
    }
}

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlacedObject : NetworkBehaviour
{
    /*
     *  This class is / will be used to store data about placed objects in the scene.
     *  I might decide to save the actual data on a serializable class instead of this one and use this one as a connetion to that class.
     *
     *  Content that I (might) want to store:
     * - Name of the object (not the gameobject name, but the name the DM gave it) (the gameobject name will be used to find the prefab
     * - Notes about the object, like what it does, what it is, etc.
     * - Maybe a function to hide the object from the player, this way the DM can hide objects that the player should not see yet
     */
    
    
    [SyncVar] public string ObjectName;
    public string notes;
    
    public GenerateSnappingPoints snappingPointsGenerator;

    private void Start()
    {
        snappingPointsGenerator = gameObject.AddComponent<GenerateSnappingPoints>();
        BuildingManager.Instance.spawnedObjects.Add(gameObject);

        if (!BuildingManager.Instance.ghostPlacer.GetGhostObject() == this.gameObject)
        {
            transform.SetParent(BuildingManager.Instance._parent.transform);
            gameObject.tag = "Selectable";
        }
        
    }
    
    public void ObjectPlaced()
    {
        snappingPointsGenerator = GetComponent<GenerateSnappingPoints>();
    }

    public List<GameObject> GetSnapPoints()
    {
        return snappingPointsGenerator.snapPoints;
    }
}

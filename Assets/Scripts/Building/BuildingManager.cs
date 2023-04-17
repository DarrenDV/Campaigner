using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    [SerializeField] private List<GameObject> placeableObjects; //Is only serialized for debugging purposes
    [SerializeField] private Material ghostMaterial;
    
    public readonly Dictionary<string, GameObject> placeableObjectsDict = new Dictionary<string, GameObject>();

    private const string PREFABS_PATH = "Prefabs";

    public GameObject _parent;
    public GhostPlacerAndSnapper ghostPlacer;

    public List<GameObject> spawnedObjects;

    /// <summary>
    /// Makes every object have visible snap points
    /// </summary>
    public bool DebugSnapPoints = false;

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
        
        if(_parent == null)
            _parent = new GameObject("Parent");
    }

    private void Start()
    {
        SetPlaceableObjects();
        ghostPlacer = GetComponentInChildren<GhostPlacerAndSnapper>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {

        if (Input.GetMouseButtonDown(0))
        {
            PlacingInput();
        }
    }

    /// <summary>
    /// Functions called when the player presses the left mouse button regarding placing objects
    /// </summary>
    private void PlacingInput()
    {
        if (!ghostPlacer.enabled || ghostPlacer.GetGhostObject() == null || UIUtils.Instance.IsPointerOverUIElement())
        {
            return;
        }
        
        GameObject go = ghostPlacer.GetGhostObject();

        if (!NetworkServer.active && !NetworkClient.isConnected) //If we are not connected to a server, we can just place the object
        {
            PlaceObject(go.name, go.transform);
        }
        else
        {
            if (NetworkServer.active) //If we are the server, we can place the object
            {
                PlaceObject(go.name, go.transform);
            }
            else if(NetworkClient.isConnected) //If we are a client, we need to send a command to the server to place the object
            {
                NetworkCommands.Instance.CmdPlaceItem(go.name, go.transform.position, go.transform.rotation, go.transform.localScale);
            }
        }

        ghostPlacer.ClearGhostObject();
        //ghostPlacer.enabled = false;
        
        SpawnGhostObject(go.name);
    }

    /// <summary>
    /// Collects all the prefabs in the Resources/Prefabs folder and adds them to the placeableObjects list
    /// </summary>
    private void SetPlaceableObjects()
    {
        placeableObjects = Resources.LoadAll<GameObject>(PREFABS_PATH).ToList();

        foreach (GameObject prefab in placeableObjects)
        {
            placeableObjectsDict.Add(prefab.name, prefab);
        }
    }

    /// <summary>
    /// Function called to place an object in the world
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="_transform"></param>
    public void PlaceObject(string objectName, Transform _transform)
    {
        PlaceObject(objectName, _transform.position, _transform.rotation, _transform.localScale);
    }

    /// <summary>
    /// Place an object in the world with a specific position, rotation and scale
    /// </summary>
    /// <param name="objectName">The name of the prefab</param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    public void PlaceObject(string objectName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject go = Instantiate(placeableObjectsDict[objectName]);
        go.name = objectName;

        if (NetworkServer.active)
        {   
            NetworkServer.Spawn(go);
        }
        
        go.transform.position = position;
        
        
        go.transform.rotation = rotation;
        go.transform.localScale = scale;
        
        go.GetComponent<PlacedObject>().ObjectPlaced(objectName);
        
        UpdateWorldBounds(position);
    }

    /// <summary>
    /// Function called to spawn a ghost object that'll be used to place the object in the world
    /// </summary>
    /// <param name="objectName"></param>
    public void SpawnGhostObject(string objectName)
    {
        if (ghostPlacer.enabled && ghostPlacer.GetGhostObject() != null)
        {
            ghostPlacer.ClearGhostObject();
        }
            
        
        GameObject go = Instantiate(placeableObjectsDict[objectName]);
        go.name = objectName;
        go.GetComponent<PlacedObject>().ObjectPlaced(objectName, true);
        go.GetComponent<Renderer>().material = ghostMaterial;
        

        ghostPlacer.enabled = true;
        ghostPlacer.SetGhostObject(go);
    }

    /// <summary>
    /// Updates the world bounds to fit the new object
    /// </summary>
    /// <param name="position"></param>
    private void UpdateWorldBounds(Vector3 position)
    {
        Vector3 minBounds = WorldInfo.Instance.GetMinBounds();
        
        if (position.x < minBounds.x)
        {
            minBounds.x = position.x;
        }
        if(position.y < minBounds.y)
        {
            minBounds.y = position.y;
        }
        if(position.z < minBounds.z)
        {
            minBounds.z = position.z;
        }
        
        WorldInfo.Instance.SetMinBounds(minBounds);
        
        Vector3 maxBounds = WorldInfo.Instance.GetMaxBounds();
        
        if (position.x > maxBounds.x)
        {
            maxBounds.x = position.x;
        }
        if(position.y > maxBounds.y)
        {
            maxBounds.y = position.y;
        }
        if(position.z > maxBounds.z)
        {
            maxBounds.z = position.z;
        }
        
        WorldInfo.Instance.SetMaxBounds(maxBounds);
        
    }
    
    /// <summary>
    /// Destroys all built objects in the scene
    /// </summary>
    public void ClearScene()
    {
        foreach (Transform child in _parent.transform)
        {
            if (NetworkServer.active)
            {
                NetworkServer.Destroy(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    /// <summary>
    /// Returns the parent object that houses all placed objects
    /// </summary>
    /// <returns></returns>
    public GameObject GetParent()
    {
        return _parent;
    }
    
}
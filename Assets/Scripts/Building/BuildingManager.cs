using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildingManager : NetworkBehaviour
{
    public static BuildingManager Instance { get; private set; }

    [SerializeField] private List<GameObject> placeableObjects; //Is only serialized for debugging purposes
    [SerializeField] private Material ghostMaterial;
    
    public readonly Dictionary<string, GameObject> placeableObjectsDict = new Dictionary<string, GameObject>();

    private const string PREFABS_PATH = "Prefabs";

    public GameObject _parent;
    private GhostPlacerAndSnapper _ghostPlacer;

    public List<GameObject> spawnedObjects;

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
        _ghostPlacer = GetComponentInChildren<GhostPlacerAndSnapper>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (_ghostPlacer.enabled && _ghostPlacer.GetGhostObject() != null && !UIUtils.Instance.IsPointerOverUIElement())
            {
                GameObject go = _ghostPlacer.GetGhostObject();

                if (NetworkServer.active)
                {
                    Debug.Log("Poep");
                    PlaceObject(go.name, go.transform);
                }
                else if(NetworkClient.isConnected)
                {
                    Debug.Log("Kak");
                    CmdPlaceItem(go.name, go.transform.position, go.transform.rotation, go.transform.localScale);
                }
                
                _ghostPlacer.ClearGhostObject();
                _ghostPlacer.enabled = false;
            }
        }
    }
    
    private void SetPlaceableObjects()
    {
        placeableObjects = Resources.LoadAll<GameObject>(PREFABS_PATH).ToList();

        foreach (GameObject prefab in placeableObjects)
        {
            placeableObjectsDict.Add(prefab.name, prefab);
        }
    }
    
    [Command(requiresAuthority = false)]
    private void CmdPlaceItem(string objectName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Debug.Log("Placing item");
        PlaceObject(objectName, position, rotation, scale);
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

        // go.AddComponent<GenerateSnappingPoints>();
        // go.GetComponent<PlacedObject>().ObjectPlaced();
        
        //go.tag = "Selectable";
        
        //go.transform.SetParent(_parent.transform);
        
        UpdateWorldBounds(position);
    }

    /// <summary>
    /// Function called to spawn a ghost object that'll be used to place the object in the world
    /// </summary>
    /// <param name="objectName"></param>
    public void SpawnGhostObject(string objectName)
    {
        if (_ghostPlacer.enabled && _ghostPlacer.GetGhostObject() != null)
        {
            _ghostPlacer.ClearGhostObject();
        }
            
        
        GameObject go = Instantiate(placeableObjectsDict[objectName]);
        go.name = objectName;
        go.AddComponent<GhostObject>();
        go.GetComponent<Renderer>().material = ghostMaterial;
        

        _ghostPlacer.enabled = true;
        _ghostPlacer.SetGhostObject(go);
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    [SerializeField] private List<GameObject> placeableObjects; //Is only serialized for debugging purposes
    [SerializeField] private Material ghostMaterial;
    
    public readonly Dictionary<string, GameObject> placeableObjectsDict = new Dictionary<string, GameObject>();

    private const string PREFABS_PATH = "Prefabs";

    private GameObject _parent;
    private GhostPlacerAndSnapper _ghostPlacer;

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
                PlaceObject(go.name, go.transform);
                
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

        go.AddComponent<GenerateSnappingPoints>();
        go.GetComponent<PlacedObject>().ObjectPlaced();
        
        go.tag = "Selectable";
        
        go.transform.SetParent(_parent.transform);
        
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
    
    public void ClearScene()
    {
        foreach (Transform child in _parent.transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    public GameObject GetParent()
    {
        return _parent;
    }
    
}
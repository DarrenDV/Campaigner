using UnityEngine;
using TMPro;

public class SaveAndLoadStuff : MonoBehaviour
{
    /*
     * Class is messy and has a dumb name.
     * Will be fixed later
     */
    
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private GameObject parent;

    [SerializeField] private GameObject SaveUI;
    [SerializeField] private TMP_InputField inputField;
    
    
    [SerializeField] private GameObject LoadUI;
    [SerializeField] private TMP_Dropdown dropdown;

    private bool openMenu;
 
    private string version = "0.0.1";   
    
    private void Start()
    {
        if (saveManager == null)
        {
            saveManager = GetComponent<SaveManager>();
        }

        if (parent == null)
        {
            parent = BuildingManager.Instance.GetParent();
        }

        version = Application.version;
    }

    private void Update()
    {
        if (!openMenu)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                SaveUI.SetActive(true);
                openMenu = true;
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                LoadUI.SetActive(true);
                openMenu = true;
            }
        }
    }

    public void SaveButtonClick()
    {
        Save(inputField.text);
        inputField.text = "";
        SaveUI.SetActive(false);
        openMenu = false;
    }


    private void Save(string mapName)
    {
        MapSceneData mapSceneData = new MapSceneData();
        mapSceneData.mapName = mapName;
        mapSceneData.version = version;

        foreach (GameObject child in SavingUtils.GetAllChildren(parent))
        {
            GameObjectData gameObjectData = new GameObjectData();
            gameObjectData.name = child.name;
            gameObjectData.position = child.transform.position;
            gameObjectData.rotation = child.transform.rotation;
            gameObjectData.scale = child.transform.localScale;
            mapSceneData.gameObjects.Add(gameObjectData);
        }
        
        saveManager.Save(mapSceneData);
    }
    
    public void LoadButtonClicked()
    {
        if(dropdown.value == 0)
        {
            Debug.Log("Please select a save file");
            return;
        }
        
        Load(dropdown.options[dropdown.value].text);
        LoadUI.SetActive(false);
        openMenu = false;
    }

    private void Load(string fileName) //TODO: DELETE PREVIOUS SPAWNED OBJECTS
    {
        MapSceneData mapSceneData = saveManager.Load(fileName);
        
        if (mapSceneData == null)
        {
            return;
        }
        if(mapSceneData.gameObjects.Count == 0)
        {
            return;
        }
        
        BuildingManager.Instance.ClearScene();
        
        foreach (GameObjectData gameObjectData in mapSceneData.gameObjects)
        {
            
            BuildingManager.Instance.PlaceObject(gameObjectData.name, gameObjectData.position, gameObjectData.rotation, gameObjectData.scale);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
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
    [SerializeField] private Builder Builder;

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
            parent = GameObject.Find("Parent");
        }
        
        if (Builder == null)
        {
            Builder = GameObject.Find("Builder").GetComponent<Builder>();
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

    private void Load(string fileName)
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
        
        foreach (GameObjectData gameObjectData in mapSceneData.gameObjects)
        {
            GameObject go = Instantiate(Builder.placeableObjectsDict[gameObjectData.name], gameObjectData.position, gameObjectData.rotation);
            go.name = gameObjectData.name;
            NetworkServer.Spawn(go);
            go.transform.localScale = gameObjectData.scale;
            go.transform.SetParent(parent.transform);
            go.tag = "Selectable";

        }
    }
}

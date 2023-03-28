using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SaveAndLoadStuff : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private GameObject parent;
    [SerializeField] private Builder Builder;
 
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
        if (Input.GetKeyDown(KeyCode.O))
        {
            Save();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            Load();
        }

    }

    private void Save()
    {
        MapSceneData mapSceneData = new MapSceneData();
        mapSceneData.mapName = "Map1";
        mapSceneData.version = version;

        foreach (GameObject child in GetAllChildren(parent))
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

    private void Load()
    {
        MapSceneData mapSceneData = saveManager.Load();
        
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
    
    private List<GameObject> GetAllChildren(GameObject parent)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            children.Add(child.gameObject);
        }

        return children;
    }
}

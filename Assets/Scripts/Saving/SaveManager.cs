using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string mapName;
    
    public void Save(MapSceneData mapSceneData)
    {
        string json = JsonUtility.ToJson(mapSceneData);
        Debug.Log(json);
        
        //Save json to file
        string path = Application.dataPath + "/SaveData/" + mapSceneData.mapName + ".json";
        File.WriteAllText(path, json);
    }
    
    public MapSceneData Load()
    {
        string path = Application.dataPath + "/SaveData/" + mapName + ".json";
        string json = "";

        if (!File.Exists(path))
        {
            return null;
        }
        
        json = File.ReadAllText(path);
        MapSceneData mapSceneData = JsonUtility.FromJson<MapSceneData>(json);
        return mapSceneData;
        
    }
}

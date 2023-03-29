using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveManager : MonoBehaviour
{
    /*
     * This should not be present in a Manager class
     * Will likely be refactored
     */
    
    public void Save(MapSceneData mapSceneData)
    {
        string json = JsonUtility.ToJson(mapSceneData);
        Debug.Log(json);
        
        //Save json to file
        string path = Application.dataPath + "/SaveData/" + mapSceneData.mapName + ".json";
        File.WriteAllText(path, json);
    }
    
    public MapSceneData Load(string fileName)
    {
        string path = Application.dataPath + "/SaveData/" + fileName + ".json";
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

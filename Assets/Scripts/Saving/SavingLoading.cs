﻿using System.IO;
using UnityEngine;

namespace Campaigner.SavingLoading
{
    public class SavingLoading 
    {
        /// <summary>
        /// Save the current scene
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="parent"></param>
        public static void Save(string saveName, GameObject parent)
        {
            MapSceneData mapSceneData = new MapSceneData();
            mapSceneData.mapName = saveName;
            mapSceneData.version = Application.version;

            foreach (GameObject child in SavingUtils.GetAllChildren(parent))
            {
                GameObjectData gameObjectData = new GameObjectData();
                gameObjectData.name = child.name;
                gameObjectData.position = child.transform.position;
                gameObjectData.rotation = child.transform.rotation;
                gameObjectData.scale = child.transform.localScale;
                mapSceneData.gameObjects.Add(gameObjectData);
            }
        
            SaveMapSceneData(mapSceneData);
        }
        
        /// <summary>
        /// Saves mapSceneData to a json file
        /// </summary>
        /// <param name="mapSceneData"></param>
        private static void SaveMapSceneData(MapSceneData mapSceneData)
        {
            string json = JsonUtility.ToJson(mapSceneData);
            Debug.Log(json);
        
            //Save json to file
            string path = Application.dataPath + "/SaveData/" + mapSceneData.mapName + ".json";
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Opens the save file and loads the scene
        /// </summary>
        /// <param name="fileName"></param>
        public static void Load(string fileName)
        {
            MapSceneData mapSceneData = LoadMapSceneData(fileName);
        
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
    
        /// <summary>
        /// Gets the json file and converts it to a MapSceneData object
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static MapSceneData LoadMapSceneData(string fileName)
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
    
    
    
}
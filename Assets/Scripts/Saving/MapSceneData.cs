using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapSceneData
{
    public string mapName;
    public string version; //Version of the game, for now has now impact but can be used for finding bugs
    public List<GameObjectData> gameObjects = new List<GameObjectData>();
}

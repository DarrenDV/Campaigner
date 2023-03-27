using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapSceneData
{
    public string mapName;
    public string version;
    public List<GameObjectData> gameObjects = new List<GameObjectData>();
}

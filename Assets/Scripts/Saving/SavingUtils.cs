using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavingUtils
{
    
    /// <summary>
    /// Get all the save files in the SaveData folder
    /// </summary>
    /// <returns>  List of save files </returns>
    public List<string> GetSaveFiles()
    {
        List<string> saveFiles = new List<string>();
        string path = Application.dataPath + "/SaveData/"; //TODO: Change this to a constant or something else not hardcoded
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);

            if (!fileName.Contains(".json"))    //TODO: Change this to a constant or something else not hardcoded
            {
                saveFiles.Add(fileName);
            }
        }
        return saveFiles;
    }
    
    /// <summary>
    /// Does the file exist in the SaveData folder
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool FileExists(string fileName)
    {
        string path = Application.dataPath + "/SaveData/" + fileName + ".json";
        return File.Exists(path);
    }
    
    /// <summary>
    /// Gets all the children of a parent gameobject
    /// </summary>
    /// <param name="parent"> The parent GameObject children containing the children </param>
    /// <returns> A list of gameobject that where children of the parent </returns>
    public static List<GameObject> GetAllChildren(GameObject parent)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            children.Add(child.gameObject);
        }

        return children;
    }
}

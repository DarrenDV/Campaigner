using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavingUtils
{
    public List<string> GetSaveFiles()
    {
        List<string> saveFiles = new List<string>();
        string path = Application.dataPath + "/SaveData/";
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);

            if (!fileName.Contains(".json"))
            {
                saveFiles.Add(fileName);
            }
        }
        return saveFiles;
    }
    
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

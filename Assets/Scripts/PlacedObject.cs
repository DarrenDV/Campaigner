using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public string notes;

    // Start is called before the first frame update
    void Start()
    {
        if(name.Contains("(Clone)"))
            name = name.Replace("(Clone)", "");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

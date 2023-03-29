using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    /*
     *  This class is / will be used to store data about placed objects in the scene.
     *  I might decide to save the actual data on a serializable class instead of this one and use this one as a connetion to that class.
     *
     *  Content that I (might) want to store:
     * - Name of the object (not the gameobject name, but the name the DM gave it) (the gameobject name will be used to find the prefab
     * - Notes about the object, like what it does, what it is, etc.
     * - Maybe a function to hide the object from the player, this way the DM can hide objects that the player should not see yet
     */
    
    
    public string ObjectName;
    public string notes;
    
}

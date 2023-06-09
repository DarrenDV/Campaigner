using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildableObject : MonoBehaviour
{
    public string objectName;
    public RawImage objectImage;
    public TextMeshProUGUI objectNameText;

    public void BuildObject()
    {
        BuildingManager.Instance.SpawnGhostObject(objectName);
    }

    public void SetObjectImage(GameObject prefab)
    {
        RuntimePreviewGenerator.BackgroundColor = Color.clear;
        objectImage.texture = RuntimePreviewGenerator.GenerateModelPreview(prefab.transform, 256, 256, false);
        
        objectNameText.text = objectName;
        
    }
    
    
}

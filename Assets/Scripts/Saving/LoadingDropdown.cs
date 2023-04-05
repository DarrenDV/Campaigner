using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingDropdown : MonoBehaviour
{
    
    /*
     *  Dropdown for selecting a save file to load
     *  Will later switch to some form of panel view thing because this is ugly
     */
    
    private void Start()
    {
        //Create a dropdown option 
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        dropdown.options.Clear();
        
        TMP_Dropdown.OptionData firstOption = new TMP_Dropdown.OptionData();
        firstOption.text = "Select a save file";
        dropdown.options.Add(firstOption);
        
        foreach(string saveFile in new SavingUtils().GetSaveFiles())
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
            optionData.text = saveFile;
            dropdown.options.Add(optionData);
        }
    }
}

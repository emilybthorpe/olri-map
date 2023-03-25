using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class fetchDirections : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject directionsObject ;
    public TMP_Dropdown resultsDropdown;
    public Button currentDirectionButton; 
    public TMP_Text currentDirectionText;
    public int directionIndex ; 
    public List<string> newList;
    void Start()
    {
        directionIndex = 0 ; 
        newList = new List<string>();
        getLatestDirectionList();
    }
    public void getLatestDirectionList(){
        newList = directionsObject.GetComponent<coordinateTranslate>().get_direction_list();
        
    }
    public void increment(){
        directionIndex ++;
        currentDirectionText.text = newList[directionIndex];
    }
    public void updateList(){
        
        resultsDropdown.ClearOptions();
        
        
        
        resultsDropdown.AddOptions(newList);
        // Show the drop-down menu
        resultsDropdown.enabled = false;
        resultsDropdown.enabled = true;

        resultsDropdown.Show();
        currentDirectionText.text = newList[directionIndex];
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class savetoPlayerPref : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject currentLocationEntry; 
    public GameObject destinationEntry;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void savePlayerPref(){
       
        PlayerPrefs.SetString("currentLocation", currentLocationEntry.GetComponent<TMP_Text>().text);
        PlayerPrefs.SetString("destination", destinationEntry.GetComponent<TMP_Text>().text);
        Debug.Log(PlayerPrefs.GetString("currentLocation"));
        Debug.Log(PlayerPrefs.GetString("destination"));
    }
}

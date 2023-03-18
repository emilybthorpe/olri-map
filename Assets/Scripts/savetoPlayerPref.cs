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
        Debug.Log(currentLocationEntry.GetComponent<TMP_Text>().text);
        Debug.Log(Int32.Parse(currentLocationEntry.GetComponent<TMP_Text>().text));
        Debug.Log("____________");
        PlayerPrefs.SetInt("currentLocation", 100);
        PlayerPrefs.SetInt("destination", 102);
        Debug.Log(PlayerPrefs.GetInt("destination"));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class dataHolder: MonoBehaviour {

    private static dataHolder _instance;

    public static dataHolder Instance { get { return _instance; } }
    

    public string destination; 
    public string currentLocation; 

    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
}
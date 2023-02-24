using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ManageHallways : MonoBehaviour
{
    public TextAsset jsonFile;

    public Hallways hallwaysFromJSON {get; set;}
 
    void Start()
    {
        hallwaysFromJSON = JsonUtility.FromJson<Hallways>(jsonFile.text);
 
        foreach (Hallway hallway in hallwaysFromJSON.hallways)
        {
            Debug.Log("Found room" + roomInfo.Number);
        }
    }
}

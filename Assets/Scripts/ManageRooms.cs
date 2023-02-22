using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ManageRooms : MonoBehaviour
{
    public TextAsset jsonFile;

    public Rooms roomsFromJSON {get; set;}
 
    void Start()
    {
        roomsFromJSON = JsonUtility.FromJson<Rooms>(jsonFile.text);
 
        foreach (RoomInfo roomInfo in roomsFromJSON.rooms)
        {
            Debug.Log("Found room" + roomInfo.Number);
        }
    }
}

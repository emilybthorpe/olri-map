using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageRooms
{
    public TextAsset jsonFile = Resources.Load<TextAsset>("rooms");

    public Rooms roomsFromJSON {get; set;}
 
    public ManageRooms()
    {
        roomsFromJSON = JsonUtility.FromJson<Rooms>(jsonFile.text);
        Debug.Log("rooms " + roomsFromJSON.ToString());
 
        foreach (RoomInfo roomInfo in roomsFromJSON.rooms)
        {
            Debug.Log("Found room" + roomInfo.Number);
        }
    }

    Rooms GetRooms() {
        return roomsFromJSON;
    }
}

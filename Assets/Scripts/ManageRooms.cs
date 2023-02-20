using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ManageRooms : MonoBehaviour
{
    public TextAsset jsonFile;
 
    void Start()
    {
        Rooms roomsInJSON = JsonUtility.FromJson<Rooms>(jsonFile.text);
 
        foreach (RoomInfo roomInfo in roomsInJSON.rooms)
        {
            Debug.Log("Found room" + roomInfo.Number);
        }
    }
}

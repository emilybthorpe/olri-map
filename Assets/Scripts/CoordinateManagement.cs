using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageCoordinates : MonoBehaviour
{
    
    public int[,] coordinateMap;
 
    TextAsset jsonFile;

    Rooms rooms;
    Hallways hallways;


    void Start()
    {
        jsonFile = Resources.Load<TextAsset>("rooms");
        coordinateMap = new int[500, 750];
        //ManageRooms manageRooms = new ManageRooms();
        ManageHallways manageHallways = new ManageHallways();
        
        //rooms = manageRooms.roomsFromJSON;
        hallways = manageHallways.hallwaysFromJSON;

        rooms = JsonUtility.FromJson<Rooms>(jsonFile.text);


        // Start my marking the entire map as outside, before marking the speicifcs of the building
        for (int x = 0; x < 500; x++) {
            for (int y = 0; y < 750; y++) {
                coordinateMap[x, y] = -1;
            }
        }

        establishRooms();
        
        // 
        // 0 = inside room (walkable)
        // 1 = wall (not walkable)
        // 2 = hallway (walkable)
        // -1 = inaccseasable (outside buidling, not walkable)
        // 
        // Logic: getting from point A to Point B using just walkable areas
    }

    /// <summary>
    /// Returns true if given point is inside a room
    /// </summary>
    bool checkIfPointInRoom(int x, int y) {
        return coordinateMap[x, y] == 1;
    }

    /// <summary>
    /// Returns a Room containing the inputed point (represented as x,y coordinates)
    /// If no such room exists, or the point is outside the building, returns null
    /// </summary>
    RoomInfo getRoomContainingPoint(int x, int y) {

        if (!checkIfPointInRoom(x, y)) {
            return null;
        }

        foreach (RoomInfo room in rooms.rooms)
        {
            if(room.coords.Contains(x) && room.coords.Contains(y)) {
                return room;
            }
        }

        return null;
    }

    void establishHallways() {
        foreach (Hallway hallway in hallways.hallways)
        {
            int[] coordinates = hallway.coords;
            for(int x = coordinates[0]; x < coordinates[2]; x++)
            {
                for(int y = coordinates[1]; y < coordinates[3]; y++) 
                {
                    coordinateMap[x, y] = 2;
                }
            }
        }
    }

    void establishRooms() {
        foreach (RoomInfo room in rooms.rooms) 
        {
            int[] coordinates = room.coords;
            if(room.shape.Equals("rect")) {
                //Mark outer walls
                for(int i = coordinates[0]; i <= coordinates[2]; i++) {
                    coordinateMap[i, coordinates[1]] = 1;
                    coordinateMap[i, coordinates[3]] = 1;
                }
                for (int i = coordinates[1]; i <= coordinates[2]; i++)
                {
                    coordinateMap[coordinates[0], i] = 1;
                    coordinateMap[coordinates[2], i] = 1;
                }
                //Mark inside room
                for(int x = coordinates[0] + 1; x < coordinates[2]; x++)
                {
                    for(int y = coordinates[1] + 1; y < coordinates[3]; y++) 
                    {
                        coordinateMap[x, y] = 0;
                    }
                }

            }
            //TODO: create code to mark polygons 
        }
    }
}

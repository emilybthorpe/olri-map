using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageCoordinates : MonoBehaviour
{
    
    public int[,] coordinateMap;
 
    Rooms rooms;


    void Start()
    {
        coordinateMap = new int[500][750];
        ManageRooms manageRooms = new ManageRooms();
        
        rooms = manageRooms.roomsFromJSON;


        // Start my marking the entire map as outside, before marking the speicifcs of the building
        for (int x = 0; x < 500; x++) {
            for (int y = 0; y < 750; y++) {
                coordinateMap[x][y];
            }
        }

        establishRooms(rooms);
        
        // """
        // 0 = inside room
        // 1 = wall
        // 2 = hallway
        // -1 = inaccseasable (outside buidling)
        // """;
    }

    /// <summary>
    /// Returns true if given point is inside a room
    /// </summary>
    bool checkIfPointInRoom(int x, int y) {
        return coordinateMap[x][y] == 1;
    }

    /// <summary>
    /// Returns a Room containing the inputed point (represented as x,y coordinates)
    /// If no such room exists, or the point is outside the building, returns null
    /// </summary>
    RoomInfo getRoomContainingPoint(int x, int y) {

        if (!checkIfPointInRoom()) {
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

    void establishHallways() {}




    void establishRooms() {
        foreach (RoomInfo room in rooms.rooms) 
        {
            int[] coordinates = room.coords;
            if(room.shape.Equals("rect")) {
                //Mark outer walls
                for(int i = coordinates[0]; i <= coordinates[2]; i++) {
                    roomMap[i][coordinates[1]] = 1;
                    roomMap[i][coordinates[3]] = 1;
                }
                for (int i = coordinates[1]; i <= coordinates[2]; i++)
                {
                    roomMap[coordinates[0]][i] = 1;
                    roomMap[coordinates[2]][i] = 1;
                }
                //Mark inside room
                for(int x = coordinates[0] + 1; x < coordinates[2]; x++)
                {
                    for(int y = coordinates[1] + 1; y < coordinates[3]; y++) 
                    {
                        roomMap[x][y] = 0;
                    }
                }

            }
            //TODO: create code to mark polygons 
        }
        return roomMap;
    }
}

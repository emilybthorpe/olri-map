using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageCoordinates : MonoBehaviour
{
    
    public int[][] coordinateMap;
 
    


    void Start()
    {
        coordinateMap = new int[500][750];
        Rooms rooms = ManageRooms.roomsFromJSON;

        establishRooms(rooms);
        
        // """
        // 0 = inside room
        // 1 = wall
        // 2 = hallway
        // -1 = inaccseasable (outside buidling)
        // """;
    }

    void establishRooms(Rooms rooms) {

        foreach (RoomInfo room in rooms.rooms) 
        {
            int[] coordinates = roomInfo.coords;
            if(roomInfo.shape.Equals("rect")) {
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

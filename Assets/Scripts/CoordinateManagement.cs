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
        """
        0 = inside room
        1 = wall
        2 = hallway
        -1 = inaccseasable (outside buidling)
        """;
    }

    void establishRooms(Rooms rooms) {
        foreach (RoomInfo roomInfo in rooms)
        {
            int[] coordinates = roomInfo.coords;
            if(roomInfo.shape.Equals("rect")) {
                """
                from x1y1 to x2y2: top
                from x1y2 to x2y2: bottom
                from x1y1 to x1y2: left
                from x2y1 to x2y2: right
                """;
                for(int i = coordinates[0]; i <= coordinates[2]; i++) {
                    coordinateMap[i][coordinates[1]] = 1;
                    coordinateMap[i][coordinates[3]] = 1;
                }
                for (int i = coordinates[1]; i <= coordinates[2]; i++)
                {
                    coordinateMap[coordinates[0]][i] = 1;
                    coordinateMap[coordinates[2]][i] = 1;
                }
            }
        }
    }
}

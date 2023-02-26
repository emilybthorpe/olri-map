using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageCoordinates : MonoBehaviour
{
    
    public int[,] coordinateMap;
 
    TextAsset roomsJSON;
    TextAsset hallwaysJSON;

    Rooms rooms;
    Hallways hallways;

    private string logPath;


    void Start()
    {
        setupLogOfMap();

        hallwaysJSON = Resources.Load<TextAsset>("hallways");
        roomsJSON = Resources.Load<TextAsset>("rooms");
        coordinateMap = new int[750, 750];

        hallways = JsonUtility.FromJson<Hallways>(hallwaysJSON.text);

        rooms = JsonUtility.FromJson<Rooms>(roomsJSON.text);

        // Start my marking the entire map as outside, before marking the speicifcs of the building
        markAllMapAsOutside();


        establishRooms();

        establishHallways();

        createLogOfMap();
        
        
    
        // 0 = inside room (walkable)
        // 1 = wall (not walkable)
        // 2 = hallway (walkable)
        // -1 = inaccseasable (outside buidling, not walkable)
        // 
        // Logic: getting from point A to Point B using just walkable areas
    }

    void setupLogOfMap() {
        //Path of the file
        logPath = Application.dataPath + "/Map.txt";
        //Create File if it doesn't exist
        if(File.Exists(logPath))
        {
            File.Delete(logPath);
        }

        File.WriteAllText(logPath, "Map \n\n");
        
    }

    void createLogOfMap() {
        //Content of the file
        string content = this.ToString();
        //Add some to text to it
        File.AppendAllText(logPath, "/n/n");
        File.AppendAllText(logPath, content);
    }

    void markAllMapAsOutside() {
        for (int x = 0; x < 750; x++) {
            for (int y = 0; y < 750; y++) {
                coordinateMap[x, y] = -1;
            }
        }
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
                    coordinateMap[y, x] = 2;
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

                //Starting from x1 to x2
                for(int i = coordinates[0]; i <= coordinates[2]; i++) {
                    //mark current x, y1
                    coordinateMap[coordinates[1], i] = 1;
                    //mark current x, y2
                    coordinateMap[coordinates[3], i] = 1;
                }
                //starting from y1 to x2
                for (int i = coordinates[1]; i <= coordinates[3]; i++)
                {
                    coordinateMap[i, coordinates[0]] = 1;
                    coordinateMap[i, coordinates[2]] = 1;
                }
                //Mark inside room
                for(int x = coordinates[0] + 1; x < coordinates[2]; x++)
                {
                    for(int y = coordinates[1] + 1; y < coordinates[3]; y++) 
                    {
                        coordinateMap[y, x] = 0;
                    }
                }

            }
            //TODO: create code to mark polygons 
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder(); 
        for (int i = 0; i < coordinateMap.GetLength(0); i++)
        {
            for (int j = 0; j < coordinateMap.GetLength(1); j++)
            {
                sb.Append(coordinateMap[i,j] + "\t");
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }
}

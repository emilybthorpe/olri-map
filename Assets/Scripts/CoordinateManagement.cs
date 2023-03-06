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
    
    public int[,] coordinateMap {get; set;}

 
    TextAsset roomsJSON;
    TextAsset hallwaysJSON;

    Rooms rooms;
    Hallways hallways;

    private string logPath;

    public bool finishedSettingUpMap = false;

    private Dictionary<int, RoomInfo> roomNumbers;


    void Start()
    {
        setupLogOfMap();

        hallwaysJSON = Resources.Load<TextAsset>("hallways");
        roomsJSON = Resources.Load<TextAsset>("rooms");
        coordinateMap = new int[750, 750];
        roomNumbers = new Dictionary<int, RoomInfo>();

        hallways = JsonUtility.FromJson<Hallways>(hallwaysJSON.text);

        rooms = JsonUtility.FromJson<Rooms>(roomsJSON.text);

        // Start my marking the entire map as outside, before marking the speicifcs of the building
        markAllMapAsOutside();


        establishHallways();
        establishRooms();

        

        createLogOfMap();
        
        finishedSettingUpMap = true;
        Debug.Log("finished setting up map");
    
        // 0 = inside room (walkable)
        // 1 = wall (not walkable)
        // 2 = hallway (walkable)
        // -1 = inaccseasable (outside buidling, not walkable)
        // 
        // Logic: getting from point A to Point B using just walkable areas
    }


    /// <summary>
    /// Prepares a text file to be populated with map information, deleting old verisions
    /// </summary>
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

    /// <summary>
    /// Generates text file containing map information
    /// </summary>
    void createLogOfMap() {
        //Content of the file
        string content = this.ToString();
        //Add some to text to it
        File.AppendAllText(logPath, "/n/n");
        File.AppendAllText(logPath, content);
    }

    /// <summary>
    /// Marks the entire map as outside and inaccseable  (value: -1)
    /// </summary>
    void markAllMapAsOutside() {
        for (int x = 0; x < coordinateMap.GetLength(0); x++) {
            for (int y = 0; y < coordinateMap.GetLength(1); y++) {
                coordinateMap[x, y] = -1;
            }
        }
    }


    /// <summary>
    /// Returns true if given point is inside any room
    /// </summary>
    bool checkIfPointInRoom(int x, int y) {
        return coordinateMap[x, y] == 1;
    }

    public RoomInfo GetRoomFromNumber(int number) {
        // verify room number is valid
        if(number >= roomNumbers.OrderBy(kvp => kvp.Value).First().Key && number <= roomNumbers.OrderBy(kvp => kvp.Value).Last().Key) {
            return roomNumbers[number];
        }

        return null;
    }


    /// <summary>
    /// Returns a Room containing the inputed point (represented as x,y coordinates)
    /// If no such room exists, or the point is outside the building, returns null
    /// </summary>
    public RoomInfo GetRoomContainingPoint(int x, int y) {

        if (!checkIfPointInRoom(x, y)) {
            return null;
        }

        foreach (RoomInfo room in rooms.rooms)
        {
            if(x > room.coords[0] && x < room.coords[2] && y > room.coords[1] && y < room.coords[3]) {
                return room;
            }
        }

        return null;
    }

    /// <summary>
    /// mark hallway locations in map
    /// </summary>
    void establishHallways() {
        foreach (Hallway hallway in hallways.hallways)
        {
            int[] coordinates = hallway.coords;
            markCoordinatesWithValue(coordinates, 2);
        }
    }

    private void markCoordinatesWithValue(int[] coordinates, int value, int modifier=0)
    {
        for (int x = coordinates[0] + modifier; x < coordinates[2]; x++)
        {
            for (int y = coordinates[1] + modifier; y < coordinates[3]; y++)
            {
                coordinateMap[y, x] = value;
            }
        }
    }

    /// <summary>
    /// Mark room's walls and inner sections in map
    /// </summary>
    void establishRooms() {
        foreach (RoomInfo room in rooms.rooms) 
        {
            int[] coordinates = room.coords;
            roomNumbers.Add(room.Number, room);

            // For now, if a room's shape is not a rectangle, leave it be
            // TODO: implement other room shapes 
            if(!room.shape.Equals("rect")) {
                continue;
            }

            // If this is not a valid room number, continue
            if(room.Number.ToString().Length != 3) {
                continue;
            }

            //Mark outer walls if 3 digit room
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
            
            // Mark inside room
            markCoordinatesWithValue(coordinates, 0 /*o, inside room*/, 1 /*start outside wall*/);
        }
    }

    /// <summary>
    /// returns the map represenation as a string
    /// </summary>
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

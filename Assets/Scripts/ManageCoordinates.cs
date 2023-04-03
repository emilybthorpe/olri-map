using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageCoordinates : MonoBehaviour
{
    
    public int[,] coordinateMap {get; set;}

 
    TextAsset roomsJSON;
    TextAsset hallwaysJSON;
    TextAsset stairsJSON;

    Rooms rooms;
    Hallways hallways;

    Stairs stairs;

    private string logPath;

    public bool finishedSettingUpMap = false;

    private Dictionary<string, RoomInfo> roomNumbers;

    private int[] interiorCoords = {235,4,60,4,65,285,130,338,633,337,632,280,665,
                                    275,666,120,633,118,635,106,426,104,359,109,
                                    361,130,235,132,235,59,259,60,256,4};

    void Start()
    {
        

        hallwaysJSON = Resources.Load<TextAsset>("hallways");
        stairsJSON = Resources.Load<TextAsset>("stairs");
        roomsJSON = Resources.Load<TextAsset>("rooms");
        coordinateMap = new int[750, 750];
        roomNumbers = new Dictionary<string, RoomInfo>();

        hallways = JsonUtility.FromJson<Hallways>(hallwaysJSON.text);

        rooms = JsonUtility.FromJson<Rooms>(roomsJSON.text);
        stairs = JsonUtility.FromJson<Stairs>(stairsJSON.text);

        SetupMap();
        
        finishedSettingUpMap = true;

        Debug.Log("finished setting up map");
    }


    /// <summary>
    /// Mark maps various components (outside, hallways, rooms, ect)
    /// </summary>
    private void SetupMap() {
        markAllMapAsOutside();
        MarkImpassibleInteriorArea();
        establishRooms();
        establishHallways();
        establishStairs();
        setupLogOfMap();
        createLogOfMap();
    }

    /// <summary>
    /// Marks interior as hallway if not otherwise marked
    /// Due to errors with image-map proccesing, sometimes spaces between rooms will not be marked 
    /// and instead be impassible. This prevents this issue and allows navigation to continue
    /// </summary>
    private void MarkImpassibleInteriorArea() {
        Point[] interiorPointArray = CoordinateMapHelper.ConvertIntCoordinateArrayToPointArray(interiorCoords);
        MarkPointsInPolygon(interiorPointArray);
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

        File.WriteAllText(logPath, "");  
    }

    /// <summary>
    /// Generates text file containing map information
    /// </summary>
    void createLogOfMap() {
        //Content of the file
        string content = this.ToString();
        //Add some to text to it
        File.AppendAllText(logPath, "Map\n\n");
        File.AppendAllText(logPath, "\n" + content);
    }

    /// <summary>
    /// Marks the entire map as outside and inaccseable  (value: -1)
    /// </summary>
    void markAllMapAsOutside() {
        for (int x = 0; x < coordinateMap.GetLength(0); x++) {
            for (int y = 0; y < coordinateMap.GetLength(1); y++) {
                coordinateMap[y, x] = 0;
            }
        }
    }

    /// <summary>
    /// mark stairs (staircases and elevators)
    /// </summary> 
    void establishStairs() {
        foreach (Stair stair in stairs.stairs)
        {
            int[] coordinates = stair.coords;
            markCoordinatesWithValue(coordinates, 2);
        }
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

    /// <summary>
    /// Mark polygonal room, first the outer wall, then the inside 
    /// </summary>
    private void MarkPolygonRoom(Point[] points)
    {
        //First, mark outer walls
        MarkOuterLinesOfPolygon(points);

        //Then, mark inside room
        MarkPointsInPolygon(points);

    }


    /// Utility Methods

    /// <summary>
    /// Returns true if given point is inside any room
    /// </summary>
    bool CheckIfPointInRoom(int x, int y) {
        return coordinateMap[y, x] == 0;
    }

    /// <Summary>
    /// Uses eucledian distance formal to find nearest stair from point. Very fast, but can be very inaccurate
    /// </summary>
    public Stair GetNearestStairEucledian(Point startPoint) {
        double nearestDistance = double.MaxValue;
        Stair closestStair = null; 
        foreach (Stair stair in stairs.stairs)
        {
            Point centerPoint = CoordinateMapHelper.GetCenterPointOfStair(stair);
            double distance = Math.Sqrt(Math.Pow((centerPoint.X - startPoint.X), 2) + Math.Pow((centerPoint.Y - startPoint.Y), 2));
            if (distance < nearestDistance ) {
                nearestDistance = distance;
                closestStair = stair;
            }
        }
        return closestStair;
    }

    /// <Summary>
    /// Finds and returns room class from a given room number
    /// If there is no such room, returns null
    /// </summary>
    public RoomInfo GetRoomFromNumber(string number) {
        // verify room number is valid
        try
        {
            return roomNumbers[number.Substring(0,3)];
        }
        catch (System.Exception)
        {
            
            return null;
        }
    }

    /// <Summary>
    /// Generates start end location from center of first room to center of second room
    /// </Summary>
    public StartEndLocation GetStartEndLocationFromRoomNumbers(string room1, string room2) 
    {    
        Point firstRoomPoint = CoordinateMapHelper.GetCenterPointOfRoom(GetRoomFromNumber(room1));
        Point secondRoomPoint = CoordinateMapHelper.GetCenterPointOfRoom(GetRoomFromNumber(room2));

        return new StartEndLocation(firstRoomPoint.X,firstRoomPoint.Y,secondRoomPoint.X,secondRoomPoint.Y);
    }
    
    /// <summary>
    /// Get rooms center point
    /// </summary>
    public Point GetCenterPointOfRoom(string number) {
        RoomInfo room = GetRoomFromNumber(number);
        int minX, maxX, minY, maxY;
        CoordinateMapHelper.GetMinMaxXY(room.coords, out minX, out maxX, out minY, out maxY);
        return new Point((maxX + minX)/ 2, (maxY + minY) / 2);
    }

    /// <summary>
    /// Gets staircase of floor
    /// </summary>
    public RoomInfo GetFloorStaircase(int floorNumber) {
        foreach (RoomInfo room in rooms.rooms)
        {
            if(CoordinateMapHelper.GetFloor(room) == floorNumber && room.Number.Length == 2) {
                return room;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns a Room containing the inputed point (represented as x,y coordinates)
    /// If no such room exists, or the point is outside the building, returns null
    /// </summary>
    public RoomInfo GetRoomContainingPoint(Point point) {

        if (!CheckIfPointInRoom(point.X, point.Y)) {
            
            return null;
        }

        foreach (RoomInfo room in rooms.rooms)
        {
            if(point.X > room.coords[0] && point.X < room.coords[2] && point.Y > room.coords[1] && point.Y < room.coords[3]) {
                return room;
            }
        }

        return null;
    }

    private void markCoordinatesWithValue(int[] coordinates, int value, int modifier=0)
    {
        int minX, maxX, minY, maxY;
        CoordinateMapHelper.GetMinMaxXY(coordinates, out minX, out maxX, out minY, out maxY);
        for (int x = minX + modifier; x < maxX - modifier; x++)
        {
            for (int y = minY + modifier; y < maxY - modifier; y++)
            {
                coordinateMap[y, x] = value;
            }
        }
    }

    /// <summary>
    /// Get floor of point
    /// </summary>
    public int getFloorOfPoint(Point point) {
        RoomInfo room = GetRoomContainingPoint(point);
        int floor = CoordinateMapHelper.GetFloor(room);
        return floor;
    }

    /// <summary>
    /// Marks outer walls of polygonal room (ie, room that is not a rectangle)
    /// </summary>
    private void MarkOuterLinesOfPolygon(Point[] points)
    {
        for (int i = 0; i < points.Length - 2; i++)
        {
            Point firstPoint = points[i];
            Point secondPoint = points[i + 1];


            // Mark a line (using the Bresenham's line algorithm https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm) between the two points
            for (int y = firstPoint.Y; y < secondPoint.Y; y++)
            {
                for (int x = firstPoint.X; x < secondPoint.X; x++)
                {
                    x = (y - firstPoint.Y) * (secondPoint.X - firstPoint.X) / (secondPoint.Y - firstPoint.Y) + firstPoint.X;
                    coordinateMap[y, x] = 1;
                }
            }
        }
    }

    //  private void markCoordinatesWithValue(int[] coordinates, int value, int modifier=0)
    // {
    //     // int minX, maxX, minY, maxY;
    //     // CoordinateMapHelper.GetMinMaxXY(coordinates, out minX, out maxX, out minY, out maxY);
    //     Point start = new Point(coordinates[0], coordinates[1]);
    //     Point end = new Point(coordinates[2], coordinates[3]);
    //     (Point topLeft, Point bottomRight) = CoordinateMapHelper.normalizePoints(start,end);
    //     for (int x = topLeft.X; x <= bottomRight.X; x++)
    //     {
    //         for (int y = topLeft.Y; y <= bottomRight.Y; y++)
    //         {
    //             coordinateMap[y, x] = value;
    //         }
    //     }
    // }

    /// <summary>
    /// Mark points in pologyon, adapted from example code here: https://www.codeproject.com/Questions/221022/Get-all-Points-within-a-Polygon-in-Csharp
    // </summary>
    private void MarkPointsInPolygon(Point[] points)
    {
        if (points.Length == 0)
            return;
        int highestx = int.MinValue;
        int highesty = int.MinValue;
        int lowestx = int.MaxValue;
        int lowesty = int.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].X > highestx)
                highestx = points[i].X;
            if (points[i].Y > highesty)
                highesty = points[i].Y;
            if (points[i].X < lowestx)
                lowestx = points[i].X;
            if (points[i].Y < lowesty)
                lowesty = points[i].Y;
        }
        for (int x = lowestx; x < highestx; x++)
        {
            for (int y = lowesty; y < highesty; y++)
            {
                if (CoordinateMapHelper.IsPointInPolygon(points, new Point(x,y)))
                {
                    coordinateMap[y, x] = 0;
                }
            }
        }
    }
    
    /// <summary>
    /// Takes a arary of integer points (ie: [x1, y1, x2, y2, ........ xN, yN])
    /// And converts to a List of <c>Point</c>s (ie: {Point(x1,y1),Point(x2,y2).....Point(xN,yN)})
    /// </summary>
    Point[] ConvertCoordinateIntegerArrayToPointArray(int[] pointsArray)
    {
        Point[] points = new Point[pointsArray.Length];
        for(int i = 0; i < pointsArray.Length - 1; i +=2) {
            points[i] = (new Point(pointsArray[i], pointsArray[i+1]));
        }
        return points;
    }

    /// <summary>
    /// Mark room's walls and inner sections in map
    /// </summary>
    void establishRooms() {
        foreach (RoomInfo room in rooms.rooms)
        {
            int[] coordinates = room.coords;
            roomNumbers.Add(room.Number, room);

                        // If this is not a valid room number, go to next room
            if (room.Number.ToString().Length != 3)
            {
                Debug.Log("Not a real room");
                continue;
            }

            switch (room.shape)
            {
                case "rect": 
                    MarkRectRoom(room.coords);
                    break;
                case "polygon":
                    MarkPolygonRoom(ConvertCoordinateIntegerArrayToPointArray(room.coords));
                    break;
                default:
                    Debug.Log("Not a room!");
                    continue;
            }
            Debug.Log("establishing room: " + room.Number);
        }
    }

    /// <summary>
    /// Mark rectangle room, both inside (0) and wall (1)
    /// </summary>
    private void MarkRectRoom(int[] coordinates)
    {
        int minX, maxX, minY, maxY;
        CoordinateMapHelper.GetMinMaxXY(coordinates, out minX, out maxX, out minY, out maxY);
        //Starting from x1 to x2
        for (int i = minX; i <= maxX; i++)
        {
            //mark current x, y1
            coordinateMap[minY, i] = 1;
            //mark current x, y2
            coordinateMap[maxY, i] = 1;
        }
        //starting from y1 to x2
        for (int i = minY; i <= maxY; i++)
        {
            coordinateMap[i, minX] = 1;
            coordinateMap[i, maxX] = 1;
        }

        // Mark inside room
        markCoordinatesWithValue(coordinates, 0 /*o, inside room*/, 1 /*start outside wall*/);
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
            sb.Append(Environment.NewLine);
        }
        return sb.ToString();
    }
}

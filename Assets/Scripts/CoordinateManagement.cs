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


    void Start()
    {
        setupLogOfMap();

        hallwaysJSON = Resources.Load<TextAsset>("hallways");
        stairsJSON = Resources.Load<TextAsset>("stairs");
        roomsJSON = Resources.Load<TextAsset>("rooms");
        coordinateMap = new int[750, 750];
        roomNumbers = new Dictionary<string, RoomInfo>();

        hallways = JsonUtility.FromJson<Hallways>(hallwaysJSON.text);

        rooms = JsonUtility.FromJson<Rooms>(roomsJSON.text);
        stairs = JsonUtility.FromJson<Stairs>(stairsJSON.text);



        // Start my marking the entire map as outside, before marking the speicifcs of the building
        markAllMapAsOutside();


        
        establishRooms();
        establishHallways();
        
        establishStairs();

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

        File.WriteAllText(logPath, "");  
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
    bool CheckIfPointInRoom(int x, int y) {
        Debug.Log(coordinateMap[y,x]);
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
            Point centerPoint = GetCenterPointOfStair(stair);
            double distance = Math.Sqrt(Math.Pow((centerPoint.X - startPoint.X), 2) + Math.Pow((centerPoint.Y - startPoint.Y), 2));
            if (distance < nearestDistance ) {
                nearestDistance = distance;
                closestStair = stair;
            }
        }
        return closestStair;
    }

    /// <summary>
    /// Uses A* to find true nearest stair to point. Very slow.
    /// </summary>
    public Stair GetNearestStairNavigation(Point startPoint) {
        double nearestDistance = double.MaxValue;
        Stair closestStair = null; 
        foreach (Stair stair in stairs.stairs)
        {
            Point centerPoint = GetCenterPointOfStair(stair);
            List<Point> thisRoute = Navigation.navigate(startPoint, centerPoint, coordinateMap).Item2;
            double totalDistance = 0;
            for(int i = 0; i < thisRoute.Count() - 1; i += 2)
            {
                double thisDistance = Math.Sqrt(Math.Pow((thisRoute[i + 1].X - thisRoute[i].X), 2) + Math.Pow((thisRoute[i+1].Y - thisRoute[i].Y), 2));
                totalDistance += thisDistance;
            }

            if (totalDistance < nearestDistance ) {
                nearestDistance = totalDistance;
                closestStair = stair;
            }
        }
        return closestStair;
    }

    public RoomInfo GetRoomFromNumber(string number) {
        // verify room number is valid
        Debug.Log(number);
        try
        {
            return roomNumbers[number.Substring(0,3)];
        }
        catch (System.Exception)
        {
            
            return null;
        }
    }

    /// <summary>
    /// Get's floor from room number
    /// </summary>
    public static int GetFloor(string number) {
        // Floor number is always the first digit of the room number
        return number[0] - 48;
    }

    public static int GetFloor(RoomInfo room) {
        return GetFloor(room.Number);
    }

    
    /// <summary>
    /// Get rooms center point
    /// </summary>
    public Point GetCenterPointOfRoom(string number) {
        RoomInfo room = GetRoomFromNumber(number);
        return new Point(room.coords[2] - room.coords[0], room.coords[3] - room.coords[1]);
    }

    public static Point GetCenterPointOfRoom(RoomInfo room) {
        return new Point(room.coords[2] - room.coords[0], room.coords[3] - room.coords[1]);
    }

    public static Point GetCenterPointOfStair(Stair stair) {
        return new Point(stair.coords[2] - stair.coords[0], stair.coords[3] - stair.coords[1]);
    }

    public RoomInfo GetFloorStaircase(int floorNumber) {
        foreach (RoomInfo room in rooms.rooms)
        {
            if(GetFloor(room) == floorNumber && room.Number.Length == 2) {
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

    private void markCoordinatesWithValue(int[] coordinates, int value, int modifier=0)
    {
        int minX, maxX, minY, maxY;
        GetMinMaxXY(coordinates, out minX, out maxX, out minY, out maxY);
        for (int x = minX + modifier; x < maxX; x++)
        {
            for (int y = minY + modifier; y < maxY; y++)
            {
                coordinateMap[y, x] = value;
            }
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

    /// <summary>
    /// Mark points in pologyon, adapted from example code here: https://www.codeproject.com/Questions/221022/Get-all-Points-within-a-Polygon-in-Csharp
    // </summary>
    private void MarkPointsInPolygon(Point[] points)
    {
        if (points.Length == 0)
            return;
        int highestx = points[0].X;
        int highesty = points[0].Y;
        int lowestx = points[0].X;
        int lowesty = points[0].Y;
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
                if (IsPointInPolygon(points, new Point(x,y)))
                {
                    coordinateMap[y, x] = 0;
                }
            }
        }
    }

    /// <summary>
    /// Determines if the given point is inside the polygon
    /// From https://stackoverflow.com/a/14998816
    /// </summary>
    /// <param name="polygon">the vertices of polygon</param>
    /// <param name="testPoint">the given point</param>
    /// <returns>true if the point is inside the polygon; otherwise, false</returns>
    private bool IsPointInPolygon(Point[] polygon, Point testPoint)
    {
        bool result = false;
        int j = polygon.Count() - 1;
        for (int i = 0; i < polygon.Count(); i++)
        {
            if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
            {
                if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
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

        }
    }

    /// <summary>
    /// Mark rectangle room, both inside (0) and wall (1)
    /// </summary>
    private void MarkRectRoom(int[] coordinates)
    {
        int minX, maxX, minY, maxY;
        GetMinMaxXY(coordinates, out minX, out maxX, out minY, out maxY);
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

    private static void GetMinMaxXY(int[] coordinates, out int minX, out int maxX, out int minY, out int maxY)
    {
        // Get min and max X,Y values (because the image-map is inconsistantly marked left-to-right or right-to-left)
        minX = Math.Min(coordinates[0], coordinates[2]);
        maxX = Math.Max(coordinates[0], coordinates[2]);
        minY = Math.Min(coordinates[1], coordinates[3]);
        maxY = Math.Max(coordinates[1], coordinates[3]);
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

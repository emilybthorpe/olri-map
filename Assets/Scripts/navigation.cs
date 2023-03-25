using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading;

public class Navigation
{
    public static void logMapToFile(string mapString)
    {
        string logPath = Application.dataPath + "/pathMap.txt";
        //Delete file if already exists
        if(File.Exists(logPath))
        {
            File.Delete(logPath);
        }

        File.WriteAllText(logPath, "Map w/ Path \n\n");

        File.AppendAllText(logPath, "/n/n" + mapString);
    }


    /// <summary>
    /// Navigates between multiple floors
    /// Returns a tuple of (floorMapsList, complete multi-floor route list)
    /// </sumary>
    public static (List<int[,]>, List<Point>) MultiFloorNavigation(ManageCoordinates coordinateManager, Point startPoint, Point endPoint, int[,] map)
    {
        RoomInfo startRoom = coordinateManager.GetRoomContainingPoint(startPoint);
        RoomInfo endRoom = coordinateManager.GetRoomContainingPoint(endPoint);
        int startFloor = CoordinateMapHelper.GetFloor(startRoom);
        int endFloor = CoordinateMapHelper.GetFloor(endRoom);
        List<int[,]> floorMaps = new List<int[,]>();

        //Check if single-floor navigation, then just do regular
        if (startFloor == endFloor ) 
        {  
            (int[,] singleFloorMap, List<Point> singleFloorReute) =  navigate(startPoint, endPoint, map);
            return (new List<int[,]>{singleFloorMap}, singleFloorReute);
        }

        int floorDifference = Mathf.Abs(endFloor - startFloor);

        List<Point> completeRoute = new List<Point>();
        
        Point stairLocation = CoordinateMapHelper.GetCenterPointOfStair(coordinateManager.GetNearestStairEucledian(startPoint));

        (int[,] startMap, List<Point> startRoute) = navigate(startPoint, endPoint, map);
        floorMaps.Add(startMap);
        startRoute = SetZValueOnPoints(startRoute, startFloor);
        completeRoute.AddRange(startRoute);


        for(int i = 0; i < floorDifference; i++) {
            (int[,] thisMap, List<Point> thisRoute) = navigate(stairLocation, stairLocation, map);
            thisRoute = SetZValueOnPoints(thisRoute, i);
            completeRoute.AddRange(thisRoute);
            floorMaps.Add(thisMap);
        }

        (int[,] endMap, List<Point> endRoute) = navigate(stairLocation, endPoint, map);
        endRoute = SetZValueOnPoints(endRoute, endFloor);
        floorMaps.Add(endMap);
        completeRoute.AddRange(endRoute);
        


        return (floorMaps, completeRoute);
    }

    public static void TheadedNavigation(Point startPoint, Point endPoint, int[,] map, BitMapImageGenerator imageGenerator) {

        ResultCallbackDelegate resultCallbackDelegate = new ResultCallbackDelegate(ResultCallBackMethod);

        NavigateHelper obj = new NavigateHelper(startPoint,endPoint,map, imageGenerator, resultCallbackDelegate);

        //Creating the Thread using ThreadStart delegate
        Thread T1 = new Thread(new ThreadStart(obj.CalculatePath));
        
        T1.Start();
    }

    
    public static void ResultCallBackMethod(int[,] path, List<Point> route, BitMapImageGenerator imageGenerator)
    {
        logMapToFile(AStar.generateStringBuilderOfMap(path).ToString());
        imageGenerator.SetMatrix(path);
        Texture2D temp = imageGenerator.Generate();
    }

    private static List<Point> SetZValueOnPoints(List<Point> points, int value) {
        List<Point> modifiedPoints = new List<Point>();
        foreach (Point point in points)
        {
            modifiedPoints.Add(new Point(point.X, point.Y, value));
        }
        return modifiedPoints;
    }

    public static (int[,], List<Point>) navigate(Point startPoint, Point endPoint, int[,] map)
    {
        var start = new Tile();
        start.Y = startPoint.X;
        start.X = startPoint.Y;

        var finish = new Tile();
        finish.Y = endPoint.X;
        finish.X = endPoint.Y;

        start.SetDistance(finish.X, finish.Y);

        var activeTiles = new List<Tile>();
        activeTiles.Add(start);
        var visitedTiles = new List<Tile>();
        
        (int[,], List<Point>) path = AStar.getBestPath(finish, activeTiles, visitedTiles, map);
        string stringPath = AStar.generateStringBuilderOfMap(path.Item1).ToString();
        Debug.Log("Reached destination");
        logMapToFile(stringPath);
        return path;
    }
}

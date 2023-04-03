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
    public static (List<int[,]>, List<Point>) MultiFloorNavigation(NavigationUIHolder uIHolder, Point startPoint, Point endPoint, int[,] map)
    {
        ManageCoordinates coordinateManager = uIHolder.manageCoordinates;
        int startFloor = coordinateManager.getFloorOfPoint(startPoint);
        int endFloor = coordinateManager.getFloorOfPoint(endPoint);
        List<int[,]> floorMaps = new List<int[,]>();

        int floorDifference = Mathf.Abs(endFloor - startFloor);

        List<Point> completeRoute = new List<Point>();

        Point stairLocation = CoordinateMapHelper.GetCenterPointOfStair(coordinateManager.GetNearestStairEucledian(startPoint));

        int[,] startMap;
        List<Point> startRoute;
        Thread firstFloorThread = new System.Threading.Thread(() =>
        {
            (startMap, startRoute) = navigate(startPoint, endPoint, map);
        });

        firstFloorThread.Start();


        for (int i = 0; i < floorDifference; i++)
        {
            (int[,] thisMap, List<Point> thisRoute) = navigate(stairLocation, stairLocation, map);
            completeRoute.AddRange(SetValuesFromNavigation(i, floorMaps, thisMap, thisRoute)); 
        }

        (int[,] endMap, List<Point> endRoute) = navigate(stairLocation, endPoint, map);
        // startRoute = NewMethod(startFloor, floorMaps, completeRoute, startMap, startRoute);

        completeRoute.AddRange(SetValuesFromNavigation(startFloor, floorMaps, startMap, startRoute));
        completeRoute.AddRange(SetValuesFromNavigation(endFloor, floorMaps, endMap, endRoute));

        return (floorMaps, completeRoute);
    }

    private static List<Point> SetValuesFromNavigation(int floor, List<int[,]> floorMaps, int[,] map, List<Point> route)
    {
        floorMaps.Add(map);
        route = SetZValueOnPoints(route, floor);
        return route;
    }

    private static List<Point> NewMethod(int startFloor, List<int[,]> floorMaps, List<Point> completeRoute, int[,] startMap, List<Point> startRoute)
    {
        floorMaps.Add(startMap);
        startRoute = SetZValueOnPoints(startRoute, startFloor);
        completeRoute.AddRange(startRoute);
        return startRoute;
    }

    public static void TheadedNavigation(Point startPoint, Point endPoint, int[,] map, NavigationUIHolder uiholder) {

        ResultCallbackDelegate resultCallbackDelegate = new ResultCallbackDelegate(ResultCallBackMethod);

        NavigateHelper obj = new NavigateHelper(startPoint,endPoint,map, ref uiholder, resultCallbackDelegate);

        //Creating the Thread using ThreadStart delegate
        Thread T1 = new Thread(new ThreadStart(obj.CalculatePath));
        
        T1.Start();
    }

    public static void ThreadedMultiFloorNavigation(Point startPoint, Point endPoint, int[,] map, NavigationUIHolder uiholder) {

        ResultCallbackDelegate resultCallbackDelegate = new ResultCallbackDelegate(ResultCallBackMethod);

        NavigateHelper obj = new NavigateHelper(startPoint,endPoint,map, ref uiholder, resultCallbackDelegate);

        //Creating the Thread using ThreadStart delegate
        Thread T1 = new Thread(new ThreadStart(obj.CalculatePath));
        
        T1.Start();
    }

    
    public static void ResultCallBackMethod(int[,] path, List<Point> route, ref NavigationUIHolder navigationUIHolder)
    {
        Debug.Log("Finished thread");
        navigationUIHolder.path = path;
        navigationUIHolder.route = route;
        navigationUIHolder.finishedNavigation = true;
    }

    public static void MultiFloorResultCallBackMethod(int[,] path, List<Point> route, ref NavigationUIHolder navigationUIHolder) 
    {

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
        //logMapToFile(stringPath);
        return path;
    }
}

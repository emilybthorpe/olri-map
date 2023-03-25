using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;
using System;
/**
Adapted with modifications from https://dotnetcoretutorials.com/2020/07/25/a-search-pathfinding-algorithm-in-c/
**/

public class Navigation
{

    private static List<Point> route = new List<Point>();

    
    

    /// <Summary>
    /// Generates start end location from center of first room to center of second room
    /// </Summary>
    public static StartEndLocation GetStartEndLocationFromRoomNumbers(ManageCoordinates coordinateManager, string room1, string room2) 
    {    
        RoomInfo firstRoom = coordinateManager.GetRoomFromNumber(room1);
        RoomInfo secondRoom = coordinateManager.GetRoomFromNumber(room2);
        Point firstRoomPoint = ManageCoordinates.GetCenterPointOfRoom(firstRoom);
        Point secondRoomPoint = ManageCoordinates.GetCenterPointOfRoom(secondRoom);

        return new StartEndLocation(firstRoomPoint.X,firstRoomPoint.Y,secondRoomPoint.X,secondRoomPoint.Y);
    }

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
        int startFloor = ManageCoordinates.GetFloor(startRoom);
        int endFloor = ManageCoordinates.GetFloor(endRoom);
        List<int[,]> floorMaps = new List<int[,]>();
        //Check if multi-floor navigation
        if (startFloor == endFloor ) 
        {  
            (int[,] singleFloorMap, List<Point> singleFloorReute) =  navigate(startPoint, endPoint, map);
            return (new List<int[,]>{singleFloorMap}, singleFloorReute);
        }

        int floorDifference = Mathf.Abs(endFloor - startFloor);

        List<Point> completeRoute = new List<Point>();
        
        Point stairLocation = ManageCoordinates.GetCenterPointOfStair(coordinateManager.GetNearestStairEucledian(startPoint));

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
        logMapToFile(generateStringBuilderOfMap(path).ToString());
        imageGenerator.SetMatrix(path);
        Texture2D temp = imageGenerator.Generate();
    }

    public static List<Point> SetZValueOnPoints(List<Point> points, int value) {
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
        
        (int[,], List<Point>) path = getBestPath(finish, activeTiles, visitedTiles, map);
        string stringPath = generateStringBuilderOfMap(path.Item1).ToString();
        Debug.Log("Reached destination");
        logMapToFile(stringPath);
        return path;
    }

    public static List<Point> getRoute() {

        return null;
    }

    

    private static (int[,], List<Point>) getBestPath(Tile finish, List<Tile> activeTiles, List<Tile> visitedTiles, int [,] map)
    {
        while (activeTiles.Any())
        {
            var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();
            Debug.Log(checkTile);
            if (checkTile.X == finish.X && checkTile.Y == finish.Y)
            {
                Debug.Log("finished");
                int[,] path = getArrayMap(checkTile, map);
                List<Point> route = generateRouteFromMap(map);
                return (path, route);
            }

            visitedTiles.Add(checkTile);
            activeTiles.Remove(checkTile);

            var walkableTiles = GetWalkableTiles(map, checkTile, finish);

            int count = 0;
            foreach (var walkableTile in walkableTiles)
            {
                count++;
                Debug.Log("Walkable tile " + count + " " + walkableTile);
                //Allready visited this tile
                if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    continue;

                //Check for better route
                if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                {
                    var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                    if (existingTile.CostDistance > checkTile.CostDistance)
                    {
                        activeTiles.Remove(existingTile);
                        activeTiles.Add(walkableTile);
                    }
                }
                else
                {
                    //Add new tile to the list of active tiles
                    activeTiles.Add(walkableTile);
                }
            }
        }
        Debug.Log("No path!");

        return (null, null);
    }

    /// <summary>
    /// Generates a represenation of the coordinate map with the path marked
    /// </summary> 
    /// <param name="checkTile">final tile in path</param>
    /// <param name="map"><c>int[,]</c> coordinate map to check</param>
    private static string getPath(Tile checkTile, int[,] map)
    {
        StringBuilder sb = new StringBuilder(); 
        var tile = checkTile;

        int[,] mapWithPath = map;

        while (tile != null) {
            if (map[tile.Y, tile.X] == 0 || map[tile.Y, tile.X] == 2) //if traversable area
            {
                mapWithPath[tile.Y, tile.X] = 3; // mark as path 
            }
            tile = tile.Parent;
        }

        sb = generateStringBuilderOfMap(mapWithPath);
        return sb.ToString();
    }

    /// <summary>
    /// Generates a represenation of the coordinate map with the path marked
    /// </summary> 
    /// <param name="checkTile">final tile in path</param>
    /// <param name="map"><c>int[,]</c> coordinate map to check</param>
    private static int[,] getArrayMap(Tile checkTile, int[,] map)
    {
        StringBuilder sb = new StringBuilder(); 
        var tile = checkTile;

        int[,] mapWithPath = map;

        while (tile != null) {
            if (map[tile.Y, tile.X] == 0 || map[tile.Y, tile.X] == 2) //if traversable area
            {
                mapWithPath[tile.Y, tile.X] = 3; // mark as path 
            }
            tile = tile.Parent;
        }

        return mapWithPath;
    }

    public static StringBuilder generateStringBuilderOfMap(int[,] outMap)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < outMap.GetLength(0); i++)
        {
            for (int j = 0; j < outMap.GetLength(1); j++)
            {
                if (outMap[i,j] == 3)
                {
                    sb.Append("*\t");
                    route.Add(new Point(i, j));
                }
                else {sb.Append(outMap[i, j] + "\t");}
            }
            sb.Append("\n");
        }
        return sb;
    }

    private static List<Point> generateRouteFromMap(int[,] outMap)
    {
        List<Point> route = new List<Point>();
        for (int i = 0; i < outMap.GetLength(0); i++)
        {
            for (int j = 0; j < outMap.GetLength(1); j++)
            {
                if (outMap[i,j] == 3)
                {
                    route.Add(new Point(i, j));
                }
            }
        }
        return route;
    }

    private static List<Tile> GetWalkableTiles(int[,] map, Tile currentTile, Tile targetTile)
    {
        var possibleTiles = new List<Tile>()
        {
            new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
            new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
            new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
            new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
        };

        possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

        var maxX = map.GetLength(0) - 1;
        var maxY = map.GetLength(1) - 1;   

        return possibleTiles
                .Where(tile => tile.X >= 0 && tile.X <= maxX)
                .Where(tile => tile.Y >= 0 && tile.Y <= maxY)
                .Where(tile => map[tile.Y, tile.X] == 0 || map[tile.Y,tile.X] == 2 || (targetTile.X == tile.X && targetTile.Y == tile.Y))
                .ToList();
    }

}

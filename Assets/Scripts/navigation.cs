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

/**
Adapted with modifications from https://dotnetcoretutorials.com/2020/07/25/a-search-pathfinding-algorithm-in-c/
**/

public class Navigation
{

    private static List<Point> route = new List<Point>();

    
    

    /// <Summary>
    /// Generates start end location from center of first room to center of second room
    /// </Summary>
    public static StartEndLocation GetStartEndLocationFromRoomNumbers(ManageCoordinates coordinateManager, int room1, int room2) 
    {    
        RoomInfo firstRoom = coordinateManager.GetRoomFromNumber(room1);
        RoomInfo secondRoom = coordinateManager.GetRoomFromNumber(room2);
        int x1 = (firstRoom.coords[0] + firstRoom.coords[2])/2;
        int y1 = (firstRoom.coords[1] + firstRoom.coords[3]) / 2;
        int x2 = (secondRoom.coords[0] + secondRoom.coords[2])/2;
        int y2 = (secondRoom.coords[1] + secondRoom.coords[3]) / 2;
        return new StartEndLocation(x1,y1,x2,y2);
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

    public static (string, List<Point>) navigate(int startX, int startY, int endX, int endY, int [,] map)
    {
        var start = new Tile();
        start.Y = startY;
        start.X = startX;

        var finish = new Tile();
        finish.Y = endY;
        finish.X = endX;

        start.SetDistance(finish.X, finish.Y);

        var activeTiles = new List<Tile>();
        activeTiles.Add(start);
        var visitedTiles = new List<Tile>();
        
        (string, List<Point>) path = getBestPath(finish, activeTiles, visitedTiles, map);
        string stringPath = path.Item1;
        Debug.Log("Reached destination");
        logMapToFile(stringPath);
        return path;
    }

    public static List<Point> getRoute() {

        return null;
    }

    

    private static (string, List<Point>) getBestPath(Tile finish, List<Tile> activeTiles, List<Tile> visitedTiles, int [,] map)
    {
        while (activeTiles.Any())
        {
            var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();
            Debug.Log(checkTile);
            if (checkTile.X == finish.X && checkTile.Y == finish.Y)
            {
                string path = getPath(checkTile, map);
                List<Point> route = generateRouteFromMap(map);
                return (path, route);
            }

            visitedTiles.Add(checkTile);
            activeTiles.Remove(checkTile);

            var walkableTiles = GetWalkableTiles(map, checkTile, finish);

            foreach (var walkableTile in walkableTiles)
            {
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

    private static StringBuilder generateStringBuilderOfMap(int[,] outMap)
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

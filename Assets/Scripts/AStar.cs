using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;


/**
Adapted with modifications from https://dotnetcoretutorials.com/2020/07/25/a-search-pathfinding-algorithm-in-c/
**/

public class AStar 
{
    /// <summary>
    /// Main navigation method
    /// Returns a tuple of the navigated (altered) map and the route 
    /// </summary>
    public static (int[,], List<Point>) getBestPath(Tile finish, List<Tile> activeTiles, List<Tile> visitedTiles, int [,] map)
    {
        while (activeTiles.Any())
        {
            var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();
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
    public static string getPath(Tile checkTile, int[,] map)
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
    public static int[,] getArrayMap(Tile checkTile, int[,] map)
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


    /// <summary>
    /// Generates string builder representation of 2d array map
    /// </summary>
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
                }
                else {sb.Append(outMap[i, j] + "\t");}
            }
            sb.Append("\n");
        }
        return sb;
    }

    public static List<Point> generateRouteFromMap(int[,] outMap)
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

    /// <summary>
    /// Gets a list of all walkable tiles 
    /// </summary>
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
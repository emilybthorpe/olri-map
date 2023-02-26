using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/**
Adapted with modifications from https://dotnetcoretutorials.com/2020/07/25/a-search-pathfinding-algorithm-in-c/
**/

public class Navigation: MonoBehaviour
{

    public ManageCoordinates coordinateManager;

    int[,] map;

    List<Point> route;

    void Start()
    {
        this.route = new List<Point>();
        StartCoroutine(getCompletedMap());

        //Testing:
        
    }

    IEnumerator getCompletedMap()
    {
        //Wait for 5 seconds then get map
        while(!coordinateManager.finishedSettingUpMap) 
        {
            yield return new WaitForSeconds(5);
        }
        map = coordinateManager.coordinateMap;
        Debug.Log(map.ToString());

        navigate(510,132,546,134);
    }

    
    public List<Point> getRoute() 
    {
        return this.route;
    }

    public void logMapToFile(string mapString)
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

    public void navigate(int startX, int startY, int endX, int endY)
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
        
        
        string path = getBestPath(finish, activeTiles, visitedTiles);
        Debug.Log("Reached destination");
        logMapToFile(path);
    }

    private string getBestPath(Tile finish, List<Tile> activeTiles, List<Tile> visitedTiles)
    {
        while (activeTiles.Any())
        {
            var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

            if (checkTile.X == finish.X && checkTile.Y == finish.Y)
            {
                return getPath(checkTile);
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

        return generateStringBuilderOfMap(map).ToString();
    }

    private string getPath(Tile checkTile)
    {
        StringBuilder sb = new StringBuilder(); 
        var tile = checkTile;

        int[,] mapWithPath = map;
        
        while (true)
        {
            if (map[tile.Y, tile.X] == 0 || map[tile.Y, tile.X] == 2)
            {
                mapWithPath[tile.Y, tile.X] = 3;
            }
            tile = tile.Parent;
            if (tile == null)
            {
                sb = generateStringBuilderOfMap(mapWithPath);
                return sb.ToString();
            }
        }
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
                    this.route.Add(new Point(i, j));
                }
                else {sb.Append(outMap[i, j] + "\t");}
            }
            sb.Append("\n");
        }
        return sb;
    }

    private static List<Tile> GetWalkableTiles(int[,] map, Tile currentTile, Tile targetTile)
    {
        var possibleTiles = new List<Tile>()
        {
            new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
            new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1},
            new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
            new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
        };

        possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

        var maxX = map.GetLength(0) - 1;
        var maxY = map.GetLength(1) - 1;

        var newTiles = possibleTiles
                .Where(tile => tile.X >= 0 && tile.X <= maxX)
                .Where(tile => tile.Y >= 0 && tile.Y <= maxY)
                .ToList();

        newTiles.ForEach(t => Debug.Log(t.ToString()));
        newTiles.ForEach(t => Debug.Log(map[t.Y, t.X]));
        

        return possibleTiles
                .Where(tile => tile.X >= 0 && tile.X <= maxX)
                .Where(tile => tile.Y >= 0 && tile.Y <= maxY)
                .Where(tile => map[tile.Y, tile.X] == 0 || map[tile.Y,tile.X] == 2 || (targetTile.X == tile.X && targetTile.Y == tile.Y))
                .ToList();
    }

}

/**
Adapted with modifications from https://dotnetcoretutorials.com/2020/07/25/a-search-pathfinding-algorithm-in-c/
**/
using System;

class Tile
{
	public int X { get; set; }
	public int Y { get; set; }
	public int Cost { get; set; } // How many tiles needed to cross to get here
	public int Distance { get; set; } // Distance to destination
	public int CostDistance => Cost + Distance;
	public Tile Parent { get; set; } // Previous tile

	public void SetDistance(int targetX, int targetY)
	{
		this.Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
	}

    public override string ToString()
    {
        return "Tile at " + X + "," + Y;
    }
}
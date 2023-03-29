using System;
using System.Linq;

public class CoordinateMapHelper 
{
    /// <summary>
    /// Gets floor from room number
    /// </summary>
    public static int GetFloor(string number) {
        // Floor number is always the first digit of the room number
        return number[0] - 48;
    }

    /// <summary>
    /// Gets floor from room clfass
    /// </summary>
    public static int GetFloor(RoomInfo room) {
        return GetFloor(room.Number);
    }


    /// <summary>
    /// Converts array of x,y coordinates to array of Points(x,y)
    /// </summary>
    public static Point[] ConvertIntCoordinateArrayToPointArray(int[] coordinates)
    {
        Point[] pointArray = new Point[coordinates.Length];
        int count = 0;
        for(int i = 0; i < coordinates.Length - 2; i++) {
            Point thisPoint = new Point(coordinates[i], coordinates[i+1]);
            pointArray[count] = thisPoint;
            count++;
        }
        return pointArray;
    }

    /// <summary>
    /// Get genter point of room
    /// </summary>
    public static Point GetCenterPointOfRoom(RoomInfo room) {
        int minX, maxX, minY, maxY;
        GetMinMaxXY(room.coords, out minX, out maxX, out minY, out maxY);
        return new Point((maxX + minX)/ 2, (maxY + minY) / 2);
    }

    /// <summary>
    /// Get genter point of stairway/elavator
    /// </summary>
    public static Point GetCenterPointOfStair(Stair stair) {
        return GetCenterPointOfRoom(stair);
    }


    public static (Point topLeft, Point bottomRight) normalizePoints(Point p1, Point p2) {
        int x1 = p1.X;
        int x2 = p2.X;
        int y1 = p1.Y;
        int y2 = p2.Y;

        Point topLeft = new Point(Math.Max(x1, x2), Math.Min(y1, y2));
        Point bottomRight = new Point(Math.Min(x1, x2), Math.Max(y1, y2));

        return (topLeft, bottomRight);
    }


    /// <summary>
    /// Get min,max (x,Y) points of rectangle
    /// Purpose: avoiding issue where rectangle's (x,y) coords may be out of order
    /// </summary>
    public static void GetMinMaxXY(int[] coordinates, out int minX, out int maxX, out int minY, out int maxY)
    {
        // Get min and max X,Y values (because the image-map is inconsistantly marked left-to-right or right-to-left)
        minX = Math.Min(coordinates[0], coordinates[2]);
        maxX = Math.Max(coordinates[0], coordinates[2]);
        minY = Math.Min(coordinates[1], coordinates[3]);
        maxY = Math.Max(coordinates[1], coordinates[3]);
    }

    /// <summary>
    /// Determines if the given point is inside the polygon
    /// From https://stackoverflow.com/a/14998816
    /// </summary>
    /// <param name="polygon">the vertices of polygon</param>
    /// <param name="testPoint">the given point</param>
    /// <returns>true if the point is inside the polygon; otherwise, false</returns>
    public static bool IsPointInPolygon(Point[] polygon, Point testPoint)
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

}
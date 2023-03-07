using System.Collections.Generic;
using UnityEngine;
using System;

public class coordinateTranslate : MonoBehaviour
{
    //true is positive
    public static bool positive_or_negative_x = false;
    //true is positive
    public static bool positive_or_negative_y = false;
    //true means turn right
    public static bool turn_right = false;
    //true means turn left
    public static bool turn_left = false;
    //These are tester lists
    //public static List<Point> tester1 = new List<Point>();
    //public static List<Point> tester2 = new List<Point>();
    //public static List<Point> tester3 = new List<Point>();
    //public static List<Point> tester4 = new List<Point>();
    public static Point[] tester1 = { new Point(0, 0), new Point(0, 375), new Point(250, 375), new Point(250, 125), new Point(375, 125), new Point(375, 375)};



    //This method will take in two lists coordinates. It will then 
    //calculate the difference in pixels between those coordinates.
    public static void Calculate_Coordnite_Distance(List<Point> coordinates)
    {

        //accumulator for every coordinate in the list.
        int coordinate_accumulator = 0;
        //for every coordinate in the list
        foreach (Point coordinate in coordinates)
        {
            //add one to the coordinate accumulator to keep count of which coordinate we are on
            coordinate_accumulator++;
            if (coordinate_accumulator != 1)
            {

                double x_change = coordinate.X - coordinates[coordinate_accumulator - 1].X;
                double y_change = coordinate.Y - coordinates[coordinate_accumulator - 1].Y;

                double x_feet = Translate_To_Feet(x_change);
                double y_feet = Translate_To_Feet(y_change);

                if (coordinate_accumulator == 2)
                {
                    First_Movement(x_change, y_change);
                }
                else
                {
                    Right_Or_Left(x_change, y_change);
                }
                Print_Directions(x_feet, y_feet, coordinate_accumulator);
            }
        }
    }

    // This method will take in coordinate values and translate into feet.
    public static double Translate_To_Feet(double coordinate_distance)
    {
   
        return Math.Abs((coordinate_distance * .59375));

    }

    public static void First_Movement(double x_change, double y_change)
    {
        if (x_change != 0)
        {
            if (x_change > 0)
            {
                positive_or_negative_x = true;
            }
            else
            {
                positive_or_negative_x = false;
            }
        }
        else
        {
            if (y_change > 0)
            {
                positive_or_negative_y = true;
            }
            else
            {
                positive_or_negative_y = false;
            }
        }
    }

    //This method will check to see which direction a user will turn.
    //True if right, False if left.
    public static void Right_Or_Left(double changed_x, double changed_y)
    {

        if (changed_x > 0)
        {
            if (positive_or_negative_y)
            {

                turn_left = true;
                turn_right = false;

            }
            else
            {
                turn_left = false;
                turn_right = true;
            }
        }

        if (changed_x < 0)
        {
            if (positive_or_negative_y)
            {

                turn_left = false;
                turn_right = true;

            }
            else
            {
                turn_left = true;
                turn_right = false;
            }
        }

        if (changed_y > 0)
        {
            if (positive_or_negative_x)
            {

                turn_left = true;
                turn_right = false;

            }
            else
            {
                turn_left = false;
                turn_right = true;
            }
        }

        if (changed_y < 0)
        {
            if (positive_or_negative_x)
            {

                turn_left = false;
                turn_right = true;

            }
            else
            {
                turn_left = true;
                turn_right = false;
            }
        }
    }

    //This method will take in feet and a direction (if there is any) it
    //will then print out the directions to the user.
    public static void Print_Directions(double x_feet, double y_feet, int path_accumulator)
    {
        if (path_accumulator == 2)
        {
            Debug.Log("Please move forward " + (x_feet + y_feet) + " feet.");
        }
        else
        {
            if (turn_right)
            {
                Debug.Log("Please turn right.");
                if (x_feet != 0)
                {
                    Debug.Log("Now move forward " + Math.Floor(x_feet) + "feet.");
                }
                else
                {
                    Debug.Log("Now move forward " + Math.Floor(y_feet) + "feet.");
                }

            }
            else
            {
                Debug.Log("Please turn left.");
                if (x_feet != 0)
                {
                    Debug.Log("Now move forward " + Math.Floor(x_feet) + "feet.");
                }
                else
                {
                    Debug.Log("Now move forward " + Math.Floor(y_feet) + "feet.");
                }
            }
        }
    }

    void Start()
    {
        List<Point> test1 = new List<Point>(tester1);
        Calculate_Coordnite_Distance(test1);
    }

}

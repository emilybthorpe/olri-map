using System.Collections.Generic;
using UnityEngine;
using System;

public class coordinateTranslate : MonoBehaviour
{
    //true is positive
    public static bool positive_or_negative_x, positive_or_negative_y, turn_right;

    //These are tester lists
    public static Point[] tester1 = { new Point(0d, 0d), new Point(0d, 375d), new Point(250d, 375d), new Point(250d, 125d), new Point(375d, 125d), new Point(375d, 375d)};
    public static Point[] tester2 = { new Point(375d, 375d), new Point(375d, 125d), new Point(250d, 125d), new Point(250d, 375d), new Point(0d, 375d), new Point(0d, 0d)};
    public static Point[] tester3 = { new Point(0d, 375d), new Point(0d, 125d), new Point(125d, 125d), new Point(125d, 375d), new Point(250d, 375d), new Point(250d, 125d)};
    public static Point[] tester4 = { new Point(250d, 125d), new Point(250d, 375d), new Point(125d, 375d), new Point(125d, 125d), new Point(0d, 125d), new Point(0d, 375d)};
    public static Point[] tester5 = { new Point(0d, 375d), new Point(0d, 125d), new Point(125d, 125d), new Point(125d, 0d), new Point(250d, 0d)};
    public static Point[] tester6 = { new Point(250d, 0d), new Point(125d, 0d), new Point(125d, 125d), new Point(0d, 125d), new Point(0d, 375d)};


    /*
     *This method will take in a list of Points. It will then
     *calculate the difference in pixels between those coordinates.
     */
    public static void Calculate_Coordnite_Distance(List<Point> coordinates)
    {
        //initializing booleans
        positive_or_negative_x = positive_or_negative_y = turn_right = false;

        //accumulator for every coordinate in the list.
        int coordinate_accumulator = 0;
        //for every coordinate in the list
        foreach (Point coordinate in coordinates)
        {
            //add one to the coordinate accumulator to keep count of which coordinate we are on
            coordinate_accumulator++;
            if (coordinate_accumulator != 1)
            {
                //grabs the change of x and change of y from point to point
                double x_change = (double) coordinate.X - coordinates[coordinate_accumulator - 2].X;
                double y_change = (double) coordinate.Y - coordinates[coordinate_accumulator - 2].Y;

                //These methods will help us decide whether or not we need to turn left or right
                Right_Or_Left(x_change, y_change, coordinate_accumulator);
                Assign_Positive_Or_Negative(x_change, y_change);
                
                //These will translate our pixels to feet
                double x_feet = Translate_To_Feet(x_change);
                double y_feet = Translate_To_Feet(y_change);

                //This will print out the directions 
                Print_Directions(x_feet, y_feet, coordinate_accumulator, coordinates);

            }
        }
    }
    /*
     * This method will take in coordinate values and translate them into feet
     */
    public static double Translate_To_Feet(double coordinate_distance)
    {
        //returns absolute value because we won't ask someone to move backwards
        return Math.Abs((coordinate_distance * .59375));

    }

    /*
     * This method will be used to set booleans to indicate whether which direction
     * our last movement was. True for positive, false for negative.
     */
    public static void Assign_Positive_Or_Negative(double x_change, double y_change)
    {
        //if there was movement on the x plane
        if (x_change != 0)
        {
            //if it was positive
            if (x_change > 0)
            {
                //indicate there was a positive x movement
                positive_or_negative_x = true;
            }
            else
            {
                //indicate there was a negative x movement
                positive_or_negative_x = false;
            }
        }
        //there was a movement on the y plane
        else
        {
            //if it was positive
            if (y_change > 0)
            {
                //indicate there was a positive y movement
                positive_or_negative_y = true;
            }
            else
            {
                //indicate there was negative y movement.
                positive_or_negative_y = false;
            }
        }
    }


    /*
     * This method will check to see which direction a user will turn.
     * True if right, False if left.
     */
    public static void Right_Or_Left(double changed_x, double changed_y, int accumulator)
    {
        //As long as we are not on the first walking path of the user
        if (accumulator != 2)
        {
            //if there was a positive change in x
            if (changed_x > 0)
            {
                //if we had previously been moving positively on the y plane
                if (positive_or_negative_y)
                {
                    //we turn left
                    turn_right = false;

                }
                //if we had previously been moving negatively on the x plane
                else
                {
                    //turn right
                    turn_right = true;
                }
            }
            //if there was a negative change in x
            if (changed_x < 0)
            {
                //if we had previously been moving positively on the y plane
                if (positive_or_negative_y)
                {
                    //turn right
                    turn_right = true;

                }
                //if we had previously been moving negatively on the y plane
                else
                {
                    //turn left
                    turn_right = false;
                }
            }
            //if there was a positive change in y
            if (changed_y > 0)
            {
                //if we were previously moving positively on the x plane
                if (positive_or_negative_x)
                {
                    //turn right
                    turn_right = true;

                }
                //if we were previously moving negatively on the x plane
                else
                {
                    //turn left
                    turn_right = false;
                }
            }
            //if there was a negative change in the y plane
            if (changed_y < 0)
            {
                //if we were previously moving positively on the x plane
                if (positive_or_negative_x)
                {
                    //turn left
                    turn_right = false;

                }
                //if we were previously moving negatively on the x plane
                else
                {
                    //turn right
                    turn_right = true;

                }
            }
        }

        
    }

    /*
     * This method will take in feet and a direction (if there is any) it 
     * will then print out the directions to the user. It will also use an 
     * accumulator to decide how far in the directions process we are in.
     */
    public static void Print_Directions(double x_feet, double y_feet, int path_accumulator, List<Point> coordinates)
    {
        //if this is the first direction
        if (path_accumulator == 2)
        {
            Debug.Log("Please move forward " + Math.Ceiling(x_feet + y_feet) + " feet.");
        }
        //otherwise add a turn and more forward movement
        else
        {
            if (turn_right)
            {
                Debug.Log("Please turn right.");
                if (x_feet != 0)
                {
                    Debug.Log("Now move forward " + Math.Ceiling(x_feet) + "feet.");
                }
                else
                {
                    Debug.Log("Now move forward " + Math.Ceiling(y_feet) + "feet.");
                }

            }
            else
            {
                Debug.Log("Please turn left.");
                if (x_feet != 0)
                {
                    Debug.Log("Now move forward " + Math.Ceiling(x_feet) + "feet.");
                }
                else
                {
                    Debug.Log("Now move forward " + Math.Ceiling(y_feet) + "feet.");
                }
            }
        }
        //if this is the last forward movement needed, then end the directions
        if (path_accumulator == coordinates.Count)
        {
            Debug.Log("You have reached your destination!");
        }
    }

    void Start()
    {
        List<Point> test1 = new List<Point>(tester1);
        Calculate_Coordnite_Distance(test1);

        List<Point> test2 = new List<Point>(tester2);
        Calculate_Coordnite_Distance(test2);

        List<Point> test3 = new List<Point>(tester3);
        Calculate_Coordnite_Distance(test3);

        List<Point> test4 = new List<Point>(tester4);
        Calculate_Coordnite_Distance(test4);

        List<Point> test5 = new List<Point>(tester5);
        Calculate_Coordnite_Distance(test5);

        List<Point> test6 = new List<Point>(tester6);
        Calculate_Coordnite_Distance(test6);

    }

}

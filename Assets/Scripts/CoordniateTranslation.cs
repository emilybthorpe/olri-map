using System;
using System.IO;
using System.Text;

class Coordinate_Translation {
    //true is positive
    Boolean positive_or_negative_x = false;
    //true is positive
    Boolean positive_or_negative_y = false;
    //true means turn right
    Boolean turn_right = false;
    //true means turn left
    Boolean turn_left = false;
    public static void Main() {

    }

    //This method will take in two lists coordinates. It will then 
    //calculate the difference in pixels between those coordinates.
    public static Double Calculate_Coordnite_Distance_X(List<List<Double>> coordinates) {
        
        //accumulator for every coordinate in the list.
        int coordinate_accumulator = 0; 
        //for every coordinate in the list
        foreach(List<Double> coordinate in coordinates) {
            //add one to the coordinate accumulator to keep count of which coordinate we are on
            coordinate_accumulator++;
            if (coordinate_accumulator != 1) {
                Double x_change = coordinate[0] - coordinates[coordinate_accumulator - 1][0];
                Double y_change = coordinate[1] - coordinates[coordinate_accumulator - 1][1];

                Double x_feet = Abs(Translate_To_Feet(x_change));
                Double y_feet = Abs(Translate_To_Feet(y_change));

            }
        }

    }

    // This method will take in coordinate values and translate into feet.
    public static Double Translate_To_Feet(Double coordinate_distance) {
        return coordinate_distance * .59375;
    }

    //This method will check to see which direction a user will turn.
    //True if right, False if left.
    public static Boolean Right_Or_Left(Double changed_x, Double changed_y) {

        if (changed_x > 0) {
            if (positive_or_negative_y) {

                turn_left = true;
                turn_right = false;

            } else {
                turn_left = false;
                turn_right = true;
            }
        }

        if (changed_x < 0) {
            if (positive_or_negative_y) {

                turn_left = false;
                turn_right = true;

            } else {
                turn_left = true;
                turn_right = false;
            }
        }

        if (changed_y > 0) {
            if (positive_or_negative_x) {

                turn_left = true;
                turn_right = false;

            } else {
                turn_left = false;
                turn_right = true;
            }
        }

        if (changed_y < 0) {
            if (positive_or_negative_x) {

                turn_left = false;
                turn_right = true;

            } else {
                turn_left = true;
                turn_right = false;
            }
        }
    }

    //This method will take in feet and a direction (if there is any) it
    //will then print out the directions to the user.
    public static String Print_Directions(Double x_feet, Double y_feet) {

        if (turn_right) {
            Console.WriteLine("Please turn right.");
            if(x_feet != 0) {
                Console.WriteLine("Now move forward " + x_feet + "feet.");
            } else {
                Console.WriteLine("Now move forward " + y_feet + "feet.");             
            }
            
        } else {
            Console.writeLine("Please turn left.");
            if(x_feet != 0) {
                Console.WriteLine("Now move forward " + x_feet + "feet.");
            } else {
                Console.WriteLine("Now move forward " + y_feet + "feet.");             
            }
        }

    }
}
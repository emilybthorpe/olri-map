using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.EventSystems;

/*
* To1DArray method is from https://www.dotnetperls.com/flatten-array
* To2DArray method is from https://stackoverflow.com/a/28113181
*/



public class NavigateButton: MonoBehaviour
{
    public Button navigateButton;

    public ManageCoordinates manageCoordinates;

    public BitMapImageGenerator imageGenerator;
    

    public StartEndLocation startEndLocation;
    
    public string startRoom;
    public string endRoom;

    private string Map;
    private List<Point> route;

    

    void Start()
    {
        imageGenerator = new BitMapImageGenerator();
        Button btn = navigateButton.GetComponent<Button>();
        //startEndLocation = new StartEndLocation(510, 132,546,134);

       
        setRooms(PlayerPrefs.GetString("currentLocation"), PlayerPrefs.GetString("destination"));



        btn.onClick.AddListener(TaskOnClick);
    }

    void processStartEndRoom() {
        startEndLocation = Navigation.GetStartEndLocationFromRoomNumbers(manageCoordinates, startRoom.Substring(0,3),endRoom.Substring(0,3));
    }

    void setRooms(string startRoom, string endRoom) {
        this.startRoom = startRoom;
        this.endRoom = endRoom;
    }

    void setPathRoute(string Map, List<Point> route) {
        this.Map = Map;
        this.route = route;
    }

    public struct NavigationJob : IJob
    {
        public int startX;
        public int startY;
        public int endX;
        public int endY;
        public NativeArray<char> path;

        public NativeArray<int> map;


        public void Execute()
        {
            string pathStr = Navigation.generateStringBuilderOfMap(Navigation.navigate(new Point(startX, startY), new Point(endX, endY), Make2DArray<int>(map.ToArray(), 750, 750)).Item1).ToString();
            Debug.Log("Reached destination");
            Debug.Log(pathStr.Length);
            Debug.Log(path.Length);

            // Copy the path string to the NativeArray
            for (int i = 0; i < pathStr.Length - 1; i++)
            {
                path[i] = pathStr[i];
            }
        }
    }


    
    private static T[,] Make2DArray<T>(T[] input, int height, int width)
    {
        T[,] output = new T[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                output[i, j] = input[i * width + j];
            }
        }
        return output;
    }

    static int[] To1DArray(int[,] input)
    {
        // Step 1: get total size of 2D array, and allocate 1D array.
        int size = input.Length;
        int[] result = new int[size];
        
        // Step 2: copy 2D array elements into a 1D array.
        int write = 0;
        for (int i = 0; i <= input.GetUpperBound(0); i++)
        {
            for (int z = 0; z <= input.GetUpperBound(1); z++)
            {
                result[write++] = input[i, z];
            }
        }
        // Step 3: return the new array.
        return result;
    }


    void TaskOnClick(){
        processStartEndRoom();
        
        Point startPoint = new Point(startEndLocation.StartX,startEndLocation.StartY);
        Point endPoint = new Point(startEndLocation.EndX,startEndLocation.EndY);
        Navigation.TheadedNavigation(startPoint, endPoint, manageCoordinates.coordinateMap, imageGenerator);
        Texture2D temp = imageGenerator.Generate();
	}


    

}

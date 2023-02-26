using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.EventSystems;


public class NavigateButton: MonoBehaviour
{
    public Button navigateButton;

    public ManageCoordinates manageCoordinates;

    void Start()
    {
        Button btn = navigateButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
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
            string pathStr = Navigation.navigate(startX, startY, endX, endY, Make2DArray<int>(map.ToArray(), 750, 750));
            Debug.Log("Reached destination");

            // Copy the path string to the NativeArray
            for (int i = 0; i < pathStr.Length; i++)
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


		// Create the job
        var path = new NativeArray<char>(256, Allocator.TempJob);
        var map = new NativeArray<int>(To1DArray(manageCoordinates.coordinateMap), Allocator.TempJob);
        // Temp values
        var job = new NavigationJob
        {
            startX = 510,
            startY = 132,
            endX = 546,
            endY = 134,
            path = path,
            map = map
        };

        // Schedule the job
        var handle = job.Schedule();

        // Wait for the job to complete
        handle.Complete();

        // Convert the NativeArray to a string and return it
        string result = new string(path.ToArray()).TrimEnd('\0');
        path.Dispose();
        map.Dispose();
        Debug.Log(result);
	}


    

}

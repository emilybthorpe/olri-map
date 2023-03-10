using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform MapFrame;
    private LineRenderer lineRenderer;

    public SpriteRenderer spriteRenderer;
    public Color lineColor; 
    public Color pathColor;
    public float lineWidth;
    public Material mat;

    //Get these from the singleton instance of the data container from menu scene.
    public int curRoomNumber; 
    public int destRoomNumber;


    public int mapWidth ; 
    public int mapHeight; 

    void Start()
    {
        //declare curRoomNumber and destRoomNumber


        
    }

    // Update is called once per frame
    void Update()
{
    
    // Vector3 minCorner = spriteRenderer.bounds.min;
    // Vector3 maxCorner = spriteRenderer.bounds.max;
    // DrawLine(minCorner, maxCorner,"wall");
    // DrawLine(new Vector3(10,30,0), new Vector3(10,80,0),"wall");
    
}

void drawMap(char[,] map, int width ,int height){
    int counter = 0 ; 
    Debug.Log(map);
    // List<int[]> pathList = new List<int[2]>();
    List<List<char>> wallList = new List<List<char>>();
    
    
    for (int k = 0; k < map.GetLength(0); k++){
        for (int l = 0; l < map.GetLength(1); l++){
            var val = map[k, l];
            // if (val == "1"){//check if wall
            
            // }
            // else if (val == "-1"){//check if 

            // }
            // else if ( val == "0"){//Check if inside room 
            
            // }else if (val == "2"){//check if hallway 

            // }else if (val =="*"){//check if path
               
            // }
        }
    }

    //Create map from map;



}

void DrawLine(Vector3 pos1, Vector3 pos2, string tag){
    GameObject lineObject = new GameObject("Line");
    lineObject.transform.SetParent(transform);
    lineRenderer = lineObject.AddComponent<LineRenderer>();


    lineRenderer.material = mat;
    if (tag=="wall"){
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }
    lineRenderer.startWidth = lineWidth;
    lineRenderer.endWidth = lineWidth;
    lineRenderer.positionCount = 2;
    lineRenderer.sortingLayerName = "Default";
    lineRenderer.sortingOrder = 1;
    lineRenderer.SetPosition(0, pos1);
    lineRenderer.SetPosition(1, pos2);
}
}

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

    public coordinateTranslate coordinateTranslate;
    
    public StartEndLocation startEndLocation;

    public NavigationUIHolder navigationUIHolder;
    
    public string startRoom;
    public string endRoom;
    private string Map;
    private List<Point> route;

    void Start()
    {
        // imageGenerator = new BitMapImageGenerator();
        navigationUIHolder.imageGenerator = imageGenerator;
        navigationUIHolder.coordinateTranslate = coordinateTranslate;
        Button btn = navigateButton.GetComponent<Button>();
       
        // Get rooms from playerprefs
        setRooms(PlayerPrefs.GetString("currentLocation").Substring(0,PlayerPrefs.GetString("currentLocation").Length - 1), PlayerPrefs.GetString("destination").Substring(0,PlayerPrefs.GetString("destination").Length - 1));

        btn.onClick.AddListener(TaskOnClick);
    }

    void processStartEndRoom() {
        try
        {
            startEndLocation = manageCoordinates.GetStartEndLocationFromRoomNumbers(startRoom.Substring(0,3),endRoom.Substring(0,3));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Not a valid room selected!");
            Debug.LogError(e);
            throw;
        }
    }

    void setRooms(string startRoom, string endRoom) {
        this.startRoom = startRoom;
        this.endRoom = endRoom;
    }

    void setPathRoute(string Map, List<Point> route) {
        this.Map = Map;
        this.route = route;
    }
    void TaskOnClick(){
        processStartEndRoom();

        Debug.Log(startEndLocation.StartX + " , " + startEndLocation.StartY);

        Point startPoint = new Point(startEndLocation.StartX, startEndLocation.StartY);
        Point endPoint = new Point(startEndLocation.EndX, startEndLocation.EndY);
        Navigation.TheadedNavigation(startPoint, endPoint, manageCoordinates.coordinateMap, navigationUIHolder);
	}

    void Update() {
        if(navigationUIHolder.finishedNavigation) {
            finishNavigationJobs();
            navigationUIHolder.finishedNavigation = false;
        }
    }

    void finishNavigationJobs() {
        Debug.Log("starting navigation jobs");
        int [,] path = navigationUIHolder.path;
        List<Point> route = navigationUIHolder.route;
        Navigation.logMapToFile(AStar.generateStringBuilderOfMap(path).ToString());
        BitMapImageGenerator imageGenerator = navigationUIHolder.imageGenerator;
        coordinateTranslate coordinateTranslator = navigationUIHolder.coordinateTranslate;
        coordinateTranslate.Calculate_Coordnite_Distance(route);
        imageGenerator.SetMatrix(path);
        imageGenerator.Generate();
    }
}

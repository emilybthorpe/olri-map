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
    
    public string startRoom;
    public string endRoom;
    private string Map;
    private List<Point> route;

    void Start()
    {
        imageGenerator = new BitMapImageGenerator();
        Button btn = navigateButton.GetComponent<Button>();
       
        // Get rooms from playerprefs
        setRooms(PlayerPrefs.GetString("currentLocation"), PlayerPrefs.GetString("destination"));

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

        NavigationUIHolder navigationUIHolder = new NavigationUIHolder(imageGenerator,coordinateTranslate);
        Point startPoint = new Point(startEndLocation.StartX, startEndLocation.StartY);
        Point endPoint = new Point(startEndLocation.EndX, startEndLocation.EndY);
        Navigation.TheadedNavigation(startPoint, endPoint, manageCoordinates.coordinateMap, navigationUIHolder);
	}
}

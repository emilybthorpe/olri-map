using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DropDownSearch: MonoBehaviour
{
    public InputField searchField;
    public Dropdown resultsDropdown;

    TextAsset jsonFile;


    private List<string> roomNumbers;

    //private ManageRooms manageRooms;

    void Start()
    {
        jsonFile = Resources.Load<TextAsset>("rooms");

        //Debug.Log(Rooms.CreateFromJSON(jsonFile.text));
        roomNumbers = GetAllRoomNumbers();
        


        // Add an event listener to the search field to update the dropdown menu as the user types
        searchField.onValueChanged.AddListener(delegate { OnSearchTextChanged(); });

        // // Add an event listener to the search field to reattach the OnValueChanged listener when the user finishes editing the field
        // searchField.onEndEdit.AddListener(delegate { searchField.onValueChanged.AddListener(delegate { OnSearchTextChanged(); }); });
    }

    public void OnSearchTextChanged()
    {
        // Get the current text in the search field
        string searchText = searchField.text;

        // Filter the list of room numbers to only include those that start with the search text
        List<string> filteredRoomNumbers = roomNumbers.Where(rn => rn.StartsWith(searchText)).ToList();

        // Clear the drop-down menu and add the filtered room numbers as new options
        resultsDropdown.ClearOptions();
        resultsDropdown.AddOptions(filteredRoomNumbers);

        // Show the drop-down menu
        resultsDropdown.enabled = false;
        resultsDropdown.enabled = true;

        resultsDropdown.Show();
        searchField.ActivateInputField();
        searchField.caretPosition = 2;       
    }

    /// <summary>
    /// Returns a list of room numbers extracted from JSON file
    /// </summary>
    List<string> GetAllRoomNumbers()
    {
        Rooms roomsFromJSON = JsonUtility.FromJson<Rooms>(jsonFile.text);
        Debug.Log(roomsFromJSON.ToString());
        List<string> roomNames = new List<string>();
        foreach (RoomInfo room in roomsFromJSON.rooms) 
        {
            Debug.Log("Number: " + room.Number);
            if(RoomNumberIsValid(room.Number)) {
                roomNames.Add(room.Number.ToString());
            }
        }
        return roomNames;
    }

    bool RoomNumberIsValid(int number) {
        return number.ToString().Length == 3;
    }
}

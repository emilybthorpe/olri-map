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

    private List<string> roomNumbers;

    void Start()
    {
        // Retrieve the list of all room numbers and store it in the roomNumbers list
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

    private List<string> GetAllRoomNumbers()
    {
        // TODO: Retrieve the list of all room numbers from your data source
        // For this example, we'll just hard-code some sample data
        return new List<string> { "101", "102", "103", "201", "202", "203", "301", "302", "303" };
    }
}

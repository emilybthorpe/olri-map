using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownSearch: MonoBehaviour
{
    [SerializeField] private Dropdown dropdown;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Generate list of room number options
            List<string> roomNumberOptions = new List<string>();
            for (int i = 1; i <= 9; i++)
            {
                roomNumberOptions.Add(GetComponent<InputField>().text + i);
            }

            // Update dropdown options
            dropdown.ClearOptions();
            dropdown.AddOptions(roomNumberOptions);
            dropdown.Show();
            dropdown.onValueChanged.Invoke(0); // Trigger the dropdown event to update its text field
        }
    }
}

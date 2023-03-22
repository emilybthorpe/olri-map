using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ButtonController : MonoBehaviour {

    public TMP_Text floorLevelText; // Reference to the Text component that displays the floor level

    private int floorLevel; // The current value of the floor level

    // Start is called before the first frame update
    void Start() {
        // Load the current value of the floor level from PlayerPref
        floorLevel = 0 ;
        PlayerPrefs.SetInt("floorLevel", 0);
        UpdateFloorLevelText(); 
    }

    // Called when the Up button is clicked
    public void OnUpButtonClick() {
        floorLevel ++; 
        PlayerPrefs.SetInt("floorLevel", floorLevel); 
        UpdateFloorLevelText(); 
    }

    // Called when the Down button is clicked
    public void OnDownButtonClick() {
        floorLevel --; 
        PlayerPrefs.SetInt("floorLevel", floorLevel); 
        UpdateFloorLevelText(); 
    }

    // Updates the Text component with the current value of the floor level
    private void UpdateFloorLevelText() {
        floorLevelText.text =  "Floor Level: " + floorLevel.ToString(); 
    }
}

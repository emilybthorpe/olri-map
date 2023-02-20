using UnityEngine;

[System.Serializable]
public class RoomInfo
{
    public string Number {get; set;}
    
    public string shape {get; set;}

    public int[] coords {get; set;}

    public static RoomInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<RoomInfo>(jsonString);
    }


}

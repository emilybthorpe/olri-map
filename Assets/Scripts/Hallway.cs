using UnityEngine;

[System.Serializable]
public class Hallway
{
    public string Number {get; set;}
    
    public string shape {get; set;}

    public int[] coords {get; set;}

    public static Hallway CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Hallway>(jsonString);
    }
}

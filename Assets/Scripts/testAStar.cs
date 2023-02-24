using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SearchParameters
{
    public Point StartLocation { get; set; }
    public Point EndLocation { get; set; }
    public bool[,] Map { get; set; }
    
}
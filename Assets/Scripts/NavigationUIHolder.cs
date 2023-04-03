using System;
using UnityEngine;
using System.Collections.Generic;
public class NavigationUIHolder :  MonoBehaviour 
{
    public BitMapImageGenerator imageGenerator {get;set;}
    public coordinateTranslate coordinateTranslate {get;set;}

    public ManageCoordinates manageCoordinates {get; set;}

    public int[,] path {get;set;}
    
    public List<Point> route {get;set;}

    public bool finishedNavigation = false;

    public NavigationUIHolder(BitMapImageGenerator bmp, coordinateTranslate cord) {
        this.imageGenerator = bmp;
        this.coordinateTranslate = cord;
        this.manageCoordinates = null;
    }

    public NavigationUIHolder(BitMapImageGenerator bmp, coordinateTranslate cord, ManageCoordinates manageCoordinates) {
        this.imageGenerator = bmp;
        this.coordinateTranslate = cord;
        this.manageCoordinates = manageCoordinates;
    }

    public NavigationUIHolder() {
        this.imageGenerator = null;
        this.coordinateTranslate = null;
    }


}
using System;

public class NavigationUIHolder {
    public BitMapImageGenerator imageGenerator {get;set;}
    public coordinateTranslate coordinateTranslate {get;set;}

    public NavigationUIHolder(BitMapImageGenerator bmp, coordinateTranslate cord) {
        this.imageGenerator = bmp;
        this.coordinateTranslate = cord;
    }


}
using System.Collections.Generic;

public delegate void ResultCallbackDelegate(int[,] Map, List<Point> route, BitMapImageGenerator imageGenerator);

public class NavigateHelper
{
    private Point _StartPoint;
    private Point _EndPoint;
    private int[,] _Map;

    private BitMapImageGenerator _imageGenerator;

    private ResultCallbackDelegate _resultCallbackDelegate;

    public NavigateHelper(Point StartPoint, Point EndPoint, int[,] map, BitMapImageGenerator imageGenerator, ResultCallbackDelegate resultCallbackDelagate)
    {
        _StartPoint = StartPoint;
        _EndPoint = EndPoint;
        _Map = map;
        _imageGenerator = imageGenerator;
        _resultCallbackDelegate = resultCallbackDelagate;
    }

    public void CalculatePath()
    {
        int[,] path;
        List<Point> route;

        (path, route) = Navigation.navigate(_StartPoint,_EndPoint,_Map);
        

        //Before the end of the thread function call the callback method
        if (_resultCallbackDelegate != null)
        {
            _resultCallbackDelegate(path,route,_imageGenerator);
        }
    }
}

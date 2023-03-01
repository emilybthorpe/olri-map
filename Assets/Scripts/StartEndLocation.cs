public struct StartEndLocation
{
    public int StartX { get; }
    public int StartY { get; }
    public int EndX { get; }
    public int EndY { get; }

    public StartEndLocation(int startX, int startY, int endX, int endY) {
        StartX = startX;
        StartY = startY;
        EndX = endX;
        EndY = endY;
    }
}
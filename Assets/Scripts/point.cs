public struct Point
{
    public Point(int x, int y)
    {
        X = x;
        Y = y;
        Z = 0;
    }

    public Point(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }

    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public override string ToString() => $"({X}, {Y})";
}

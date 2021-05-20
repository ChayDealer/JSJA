using System;
using UnityEngine;

public class ArrayPosHandler
{
    public int x;
    public int y;

    private readonly static ArrayPosHandler up = new ArrayPosHandler(-1, 0);
    private readonly static ArrayPosHandler down = new ArrayPosHandler(1, 0);
    private readonly static ArrayPosHandler left = new ArrayPosHandler(0, -1);
    private readonly static ArrayPosHandler right = new ArrayPosHandler(0, 1);

    internal static ArrayPosHandler Up => up;
    internal static ArrayPosHandler Down => down;
    internal static ArrayPosHandler Left => left;
    internal static ArrayPosHandler Right => right;

    public ArrayPosHandler()
    {
        this.x = 0;
        this.y = 0;
    }

    public ArrayPosHandler(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static ArrayPosHandler operator +(ArrayPosHandler left, Vector2 right)
    {
        ArrayPosHandler temp = new ArrayPosHandler();
        temp.x = left.x + Convert.ToInt32(right.x);
        temp.y = left.y + Convert.ToInt32(right.y);
        return temp;
    }

    public static ArrayPosHandler operator +(ArrayPosHandler left, ArrayPosHandler right)
    {
        ArrayPosHandler temp = new ArrayPosHandler();
        temp.x = left.x + right.x;
        temp.y = left.y + right.y;
        return temp;
    }

    public static ArrayPosHandler operator -(ArrayPosHandler left, ArrayPosHandler right)
    {
        ArrayPosHandler temp = new ArrayPosHandler();
        temp.x = left.x - right.x;
        temp.y = left.y - right.y;
        return temp;
    }

    public static ArrayPosHandler operator /(ArrayPosHandler left, float value)
    {
        ArrayPosHandler temp = new ArrayPosHandler();
        temp.x = Convert.ToInt32(Math.Round(left.x / value, 0, MidpointRounding.ToEven));
        temp.y = Convert.ToInt32(Math.Round(left.y / value, 0, MidpointRounding.ToEven));
        return temp;
    }

    public static bool operator ==(ArrayPosHandler left, ArrayPosHandler right)
    {

        return (left.x == right.y && left.y == right.y);
    }

    public static bool operator !=(ArrayPosHandler left, ArrayPosHandler right)
    {

        return !(left.x == right.y && left.y == right.y);
    }

    public override bool Equals(object obj)
    {
        try
        {
            var item = obj as ArrayPosHandler;
            return (this.x.Equals(item.x) && this.y.Equals(item.y));
        }
        catch
        {
            return false;
        }               
    }

    public static bool IsLeftBorderCrossed(ArrayPosHandler position)
    {
        return position.y < 0;
    }

    public static bool IsRightBorderCrossed(ArrayPosHandler position, int maxValue)
    {
        return position.y > maxValue;
    }

    public static bool IsTopBorderCrossed(ArrayPosHandler position)
    {
        return position.x < 0;
    }

    public static bool IsBottomBorderCrossed(ArrayPosHandler position, int maxValue)
    {
        return position.x > maxValue;
    }
}

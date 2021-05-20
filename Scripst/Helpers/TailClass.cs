using UnityEngine;

public class TailClass
{
    public Transform transform;

    public ArrayPosHandler arrayPos;

    public TailClass(Transform transform, int x, int y)
    {
        this.transform = transform;
        this.arrayPos.x = x;
        this.arrayPos.y = y;
    }

    public TailClass(Transform transform, ArrayPosHandler arrayPos)
    {
        this.transform = transform;
        this.arrayPos = arrayPos;
    }
}

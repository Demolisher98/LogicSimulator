using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct Connection
{
    public ushort outputObj, outPutIndex, inputObject, inputIndex;
    public List<SerializableVector> pathPoints;

    public Connection(ushort a,ushort b,ushort c,ushort d, List<SerializableVector> points)
    {
        outputObj = a;
        outPutIndex = b;
        inputObject = c;
        inputIndex = d;
        pathPoints = points.ToList();
    }
}
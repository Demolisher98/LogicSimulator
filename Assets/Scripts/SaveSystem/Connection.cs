[System.Serializable]
public struct Connection
{
    public ushort outputObj, outPutIndex, inputObject, inputIndex;

    public Connection(ushort a,ushort b,ushort c,ushort d)
    {
        outputObj = a;
        outPutIndex = b;
        inputObject = c;
        inputIndex = d;
    }
}
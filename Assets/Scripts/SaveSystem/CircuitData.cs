using System.Collections.Generic;

[System.Serializable]
public class CircuitData
{
    public List<int> objectIDs = new List<int>();
    public List<float> xPos = new List<float>();
    public List<float> yPos = new List<float>();
    public List<Connection> connections = new List<Connection>();
    public List<string> renamedObjects = new List<string>();
    public List<int> switchesOn = new List<int>();
    public float camX = 0f,camY = 0f,size = 5f;
}
using System.Collections.Generic;

[System.Serializable]
public class BlockData
{
    public string origin;
    public enum Type {Combinational, Sequential};
    public Type type;
    public List<string> inputs = new List<string>();
    public List<string> outputs = new List<string>();
    public List<List<int>> minTerms = new List<List<int>>();
}
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadSystem
{
    public static void Save(CircuitData circuit,string fileName)
    {
        FileStream fileStream = File.Open(fileName, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(fileStream, circuit);
        fileStream.Close();
    }

    public static CircuitData Load(string fileName)
    {
        if (File.Exists(fileName))
        {
            FileStream fileStream = File.Open(fileName, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            CircuitData retrieved = formatter.Deserialize(fileStream) as CircuitData;
            fileStream.Close();
            return retrieved;
        }
        return null;
    }
}
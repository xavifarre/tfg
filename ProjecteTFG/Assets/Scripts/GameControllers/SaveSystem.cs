using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSystem
{
    private static readonly string path = Application.persistentDataPath + "/player.ox";

    public static void SaveGame()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, new SaveData());
        stream.Close();
    }

    public static void LoadGame()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            Globals.LoadGlobals(data);
        }
        else
        {
            Debug.LogWarning("Save file not found in " + path);
        }

    }

    public static void DeleteGame()
    {
        Globals.ResetGlobals();
        File.Delete(path);
    }
}

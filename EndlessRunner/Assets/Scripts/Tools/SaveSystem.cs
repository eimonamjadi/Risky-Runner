using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem
{
    private const string Save_Path = "/saves";

    public static void Save<T>(T obj, string key)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + Save_Path;
        Directory.CreateDirectory(path);

        using (FileStream stream = new FileStream(path + "/" + key, FileMode.Create))
        {
            formatter.Serialize(stream, obj);
        }
    }

    public static T Load<T>(string key)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        T data = default;
        string path = Application.persistentDataPath + Save_Path;

        if (SaveExists(key))
        {
            using FileStream stream = new FileStream(path + "/" + key, FileMode.Open);
            data = (T)formatter.Deserialize(stream);
        }
        else
        {
            Debug.LogWarning("Key " + key + " is not found at " + path);
        }

        return data;
    }

    public static bool SaveExists(string key)
    {
        string path = Application.persistentDataPath + Save_Path;
        return File.Exists(path + "/" + key);
    }
}

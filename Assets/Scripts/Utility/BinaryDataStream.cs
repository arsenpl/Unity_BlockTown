using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Windows;
using Directory = System.IO.Directory;
using File = System.IO.File;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;


public class BinaryDataStream
{
    public static void Save<T>(T serializedObject, string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        Directory.CreateDirectory(path);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + fileName + ".dat", FileMode.Create);

        try
        {
            formatter.Serialize(fileStream, serializedObject);
        }
        catch(SerializationException e)
        {
            Debug.Log("Error saving file :" + e.Message);
        }
        finally
        {
            fileStream.Close();
        }
    }

    public static bool Exist(string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        string fullName = fileName + ".dat";
        return File.Exists(path + fullName);
    }

    public static T Read<T>(string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + fileName + ".dat", FileMode.Open);
        T returnType = default(T);
        try
        {
            returnType = (T)formatter.Deserialize(fileStream);
        }
        catch (SerializationException e)
        {
            Debug.Log("Error opening file :" + e.Message);
        }
        finally
        {
            fileStream.Close();
        }
        return returnType;
    }
}

using System;
using System.Collections;
using System.IO;
using System.Linq;
using SimpleFileBrowser;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public static class Serializer {
    /// <summary>
    /// Writes the given object instance to a binary file.
    /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
    /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
    /// </summary>
    /// <typeparam name="T">The type of object being written to the binary file.</typeparam>
    /// <param name="filePath">The file path to write the object instance to.</param>
    /// <param name="objectToWrite">The object instance to write to the binary file.</param>
    /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
    public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false) {
        using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create)) {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }
    }

    /// <summary>
    /// Reads an object instance from a binary file.
    /// </summary>
    /// <typeparam name="T">The type of object to read from the binary file.</typeparam>
    /// <param name="filePath">The file path to read the object instance from.</param>
    /// <returns>Returns a new instance of the object read from the binary file.</returns>
    public static T ReadFromBinaryFile<T>(string filePath) {
        using (Stream stream = File.Open(filePath, FileMode.Open)) {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }
    }

    public static IEnumerator SaveEntity(SerializableEntity entity) {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, Application.persistentDataPath, "Species_" + entity.Network.SpeciesID + ".entity", "Save", "Save");

        if (FileBrowser.Success) {
            WriteToBinaryFile(FileBrowser.Result[0], entity);
        }
    }

    public static IEnumerator LoadEntity(Action<SerializableEntity> result) {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, Application.persistentDataPath, "", "Load", "Select");

        if (FileBrowser.Success) {
            result.Invoke(ReadFromBinaryFile<SerializableEntity>(FileBrowser.Result[0]));
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Relu.Utils
{
    public static class StorageSystem
    {
        public static void Save(object data, string fileName)
        {
            string savePath = GetSavePath(fileName);
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
            {
                formatter.Serialize(fileStream, data);
            }
        }

        public static T Load<T>(string fileName)
        {
            string savePath = GetSavePath(fileName);
            if (!File.Exists(savePath))
            {
                Debug.LogWarning("Save file not found: " + fileName);
                return default;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(savePath, FileMode.Open))
            {
                return (T)formatter.Deserialize(fileStream);
            }
        }

        private static string GetSavePath(string fileName)
        {
            string saveFolder = Application.persistentDataPath + "/Saves";
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }

            return Path.Combine(saveFolder, fileName);
        }
    }

}

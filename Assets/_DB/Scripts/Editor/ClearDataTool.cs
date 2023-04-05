using System.IO;
using UnityEditor;
using UnityEngine;

namespace DB_Core
{
    public class ClearDataTool
    {
        [MenuItem("DB/ClearData")]

        public static void ClearAllDataTool()
        {
            var path = Application.persistentDataPath;
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                if (file.Contains("DB_Game") || file.Contains("DB_Core"))
                {
                    File.Delete(file);
                }
            }

            PlayerPrefs.DeleteAll();
        }
    }
}
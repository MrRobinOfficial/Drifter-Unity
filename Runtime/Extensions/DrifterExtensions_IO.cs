using System.IO;
using UnityEngine;
using Drifter.Vehicles;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Drifter.Extensions
{
    public static partial class DrifterExtensions
    {
        public static string GetSubDirectory() => 
            Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "Vehicles/"));

        public static bool IsValidFilePath(string filePath) => 
            filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;

        public static bool TryFetchData(this BaseVehicle vehicle, string fileName, out FileData data)
        {
            data = FileData.Empty;

            if (!vehicle.IsDirectoryValid())
                return false;

            var filePath = Path.Combine(vehicle.DirectoryPath, fileName);

            if (!File.Exists(filePath))
                return false;

            try
            {
                using var stream = new FileStream(filePath, FileMode.Open);
                using var reader = new StreamReader(stream);

                data = FileData.Parse(reader.ReadToEnd());
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public static bool TryPushData(this BaseVehicle vehicle, string fileName, FileData data)
        {
            try
            {
                var filePath = Path.Combine(vehicle.DirectoryPath, fileName);

                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    return false;

                using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
                using var writer = new StreamWriter(stream);

                writer.Write(data.ToString());

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif

                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public static bool TryFetchData(this BaseVehicle vehicle, string fileName, IDatatable datable)
        {
            if (datable == null)
                return false;

            if (!vehicle.IsDirectoryValid())
                return false;

            var filePath = Path.Combine(vehicle.DirectoryPath, fileName);

            if (!File.Exists(filePath))
                return false;

            try
            {
                using var stream = new FileStream(filePath, FileMode.Open);
                using var reader = new StreamReader(stream);

                datable.Load(FileData.Parse(reader.ReadToEnd()));
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public static bool TryPushData(this BaseVehicle vehicle, string fileName, IDatatable datable)
        {
            if (datable == null)
                return false;

            try
            {
                var filePath = Path.Combine(vehicle.DirectoryPath, fileName);

                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    return false;

                using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
                using var writer = new StreamWriter(stream);

                var newData = datable.Save();
                writer.Write(newData.ToString());

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif

                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public static bool IsDirectoryValid(this BaseVehicle vehicle, out string directoryPath)
        {
            directoryPath = string.Empty;

            foreach (var dir in Directory.GetDirectories(GetSubDirectory(),
                searchPattern: "*", SearchOption.AllDirectories))
            {
                if (!dir.EndsWith(vehicle.ID, System.StringComparison.OrdinalIgnoreCase))
                    continue;

                directoryPath = dir;
            }

            return !string.IsNullOrEmpty(directoryPath) &&
                Directory.Exists(directoryPath);
        }
    }
}
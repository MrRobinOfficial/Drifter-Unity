using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Drifter.Extensions
{
    public static partial class DrifterExtensions
    {
        public static bool IsValidPath(string path) => path.IndexOfAny(Path.GetInvalidPathChars()) == -1;

        public static bool TryImportData(this BaseVehicle vehicle)
        {
            var fullPath = Path.Combine(Application.streamingAssetsPath, $"{vehicle.SubPath}.ini");

            if (!IsValidPath(fullPath))
                return false;

            if (!File.Exists(fullPath))
                File.Create(fullPath);

            vehicle.LoadData(FileData.Parse(File.ReadAllText(fullPath)));

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            return true;
        }

        public static bool TryExportData(this BaseVehicle vehicle)
        {
            var fullPath = Path.Combine(Application.streamingAssetsPath, $"{vehicle.SubPath}.ini");

            if (!IsValidPath(fullPath))
                return false;

            File.WriteAllText(fullPath, vehicle.SaveData().ToString());

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            return true;
        }
    }
}
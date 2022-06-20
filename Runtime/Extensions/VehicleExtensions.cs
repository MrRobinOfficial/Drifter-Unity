using System.Collections.Generic;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace Drifter.Extensions
{
    public static class VehicleExtensions
    {
        //        public static void ReadDataFromFile<T>(this T vehicle) where T : BaseVehicle
        //        {
        //            if (string.IsNullOrEmpty(vehicle.FolderPath) || !IsFilePathValid(vehicle.FolderPath))
        //                return;

        //            if (!File.Exists(vehicle.FolderPath))
        //                File.Create(vehicle.FolderPath).Close();

        //            var contents = File.ReadAllText(vehicle.FolderPath);
        //            var data = FileData.Parse(contents);
        //            vehicle.LoadData(data);

        //#if UNITY_EDITOR
        //            UnityEditor.AssetDatabase.Refresh();
        //#endif
        //        }

        //        private static bool IsFilePathValid(string fileName) => true; // !fileName.Any(f => Path.GetInvalidFileNameChars().Contains(f));

        //        public static void SaveDataToFile<T>(this T vehicle) where T : BaseVehicle
        //        {
        //            if (string.IsNullOrEmpty(vehicle.FolderPath))
        //                return;

        //            var folderPath = vehicle.FolderPath;

        //            if (!folderPath.EndsWith("/"))
        //                folderPath += "/";

        //            if (!Directory.Exists(folderPath))
        //                Directory.CreateDirectory(folderPath);

        //            var fileName = Path.GetFileName(folderPath);
        //            var fullPath = Path.Combine(Application.streamingAssetsPath, $"{fileName}.ini");

        //            if (!IsFilePathValid(fullPath))
        //                return;

        //            var data = vehicle.SaveData();
        //            File.WriteAllText(fullPath, data.ToString());

        //#if UNITY_EDITOR
        //            UnityEditor.AssetDatabase.Refresh();
        //#endif
        //        }

        public static string GenerateLicensePlate()
        {
            var rnd = new System.Random();

            var letter1 = (char)rnd.Next('a', 'z');
            var letter2 = (char)rnd.Next('a', 'z');
            var letter3 = (char)rnd.Next('a', 'z');

            var number1 = (int)(Random.value * 10);
            var number2 = (int)(Random.value * 10);
            var number3 = (int)(Random.value * 10);

            var builder = new System.Text.StringBuilder(capacity: 6);

            builder.Append(char.ToUpper(letter1));
            builder.Append(char.ToUpper(letter2));
            builder.Append(char.ToUpper(letter3));
            builder.Append('-');
            builder.Append(number1);
            builder.Append(number2);
            builder.Append(number3);

            return builder.ToString();
        }

        public static bool TryFindVehicle<T>(string licensePlate, out T vehicle) where T : BaseVehicle
        {
            vehicle = null;

            if (_vehicles.TryGetValue(licensePlate, out var obj))
                vehicle = obj as T;

            return vehicle != null;
        }

        private static Dictionary<string, BaseVehicle> _vehicles = new();

        public static IReadOnlyDictionary<string, BaseVehicle> Vehicles => _vehicles;

        public static bool RegisterVehicle(this BaseVehicle vehicle)
        {
            if (vehicle == null)
                return false;

            return _vehicles.TryAdd(vehicle.LicensePlate, vehicle);
        }

        public static bool UnregisterVehicle(this BaseVehicle vehicle)
        {
            if (vehicle == null)
                return false;

            var key = vehicle.LicensePlate;

            if (!_vehicles.ContainsKey(key))
                return false;

            _vehicles.Remove(key);
            return true;
        }
    }
}
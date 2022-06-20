using IniParser.Model;
using IniParser.Parser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Drifter
{
    public class FileData : IniData
    {
        public static readonly FileData Empty = new();

        public FileData() : base() { }
        public FileData(IniData ori) : base(ori) { }

        public void ReadValue<T>(string sectionName, string name, out T value)
        {
            var data = base[sectionName].GetKeyData(name);

            if (data == null || string.IsNullOrEmpty(data.Value))
            {
                value = default;
                return;
            }

            value = GetTValue(typeof(T));

            T GetTValue(System.Type type)
            {
                if (type == typeof(string))
                    return (T)System.Convert.ChangeType(data.Value, typeof(T));
                else if (IsIEnumerableOfT(type))
                {
                    return default;
                    //return (T)data.Value.Split(',').Select(item => int.Parse(item));
                }
                else if (type.IsEnum)
                {
                    if (System.Enum.TryParse(typeof(T), data.Value, out var result))
                        return (T)result;
                    else
                        return default;
                }
                else
                {
                    var isStruct = type.IsValueType &&
                        !type.IsEnum &&
                        !type.IsEquivalentTo(typeof(decimal)) &&
                        !type.IsPrimitive;

                    var isClass = type.IsClass;

                    if (isStruct | isClass)
                        return default;
                    else
                        return (T)System.Convert.ChangeType(data.Value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                }

                static bool IsIEnumerableOfT(System.Type type)
                {
                    return type.GetInterfaces().Any(x => x.IsGenericType
                           && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                }
            }
        }

        public void WriteValue<T>(string sectionName, string name, T value, params string[] comments)
        {
            var keyData = new KeyData(name)
            {
                Comments = new(comments),
                Value = GetStringValue(value)
            };
            base[sectionName].SetKeyData(keyData);

            static string GetStringValue(T value)
            {
                if (value is string)
                    return value.ToString();
                if (value is IEnumerable enumerable)
                    return string.Join(", ", enumerable.Cast<object>());
                else if (value is System.Enum)
                    return value.ToString();
                else
                {
                    return System.FormattableString.Invariant($"{value}");

                    //var type = value.GetType();

                    //var isStruct = type.IsValueType &&
                    //    !type.IsEnum &&
                    //    !type.IsEquivalentTo(typeof(decimal)) &&
                    //    !type.IsPrimitive;

                    //var isClass = type.IsClass;

                    //if (isStruct | isClass)
                    //    return GetStringValue(value);
                }
            }
        }

        public static FileData Parse(string contents) => new(new IniDataParser().Parse(contents));
    }
}
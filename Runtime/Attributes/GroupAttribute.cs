using System;
using UnityEngine;

namespace Drifter.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class GroupAttribute : PropertyAttribute
    {
        public readonly string name;

        public GroupAttribute(string name) => this.name = name;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drifter.Extensions
{
    public class BusChannel // <T> where T : unmanaged
    {
        private object value;

        public BusChannel(object value) => this.value = value;

        public void Set<TValue>(TValue newValue) => value = newValue;

        public TValue Get<TValue>() => (TValue)value;

        public void SetAsObject(object newValue) => value = newValue;

        public object GetAsObject() => value;
    }

    public class DataBus : IReadOnlyDictionary<ulong, BusChannel>
    {
        public const ulong SPEED_KPH = 0x0000000001;
        public const ulong TACHO_RPM = 0x0000000002;
        public const ulong ODO_KM = 0x0000000003;

        private Dictionary<ulong, BusChannel> _connections = new();

        public DataBus()
        {
            _connections = new();
            _connections.Add(SPEED_KPH, new BusChannel(0f));
            _connections.Add(TACHO_RPM, new BusChannel(0f));
            _connections.Add(ODO_KM, new BusChannel(0L));
        }

        public BusChannel this[ulong id] => _connections[id];

        BusChannel IReadOnlyDictionary<ulong, BusChannel>.this[ulong key] => _connections[key];
        public IEnumerable<ulong> Keys => _connections.Keys;
        public IEnumerable<BusChannel> Values => _connections.Values;
        public int Count => _connections.Count;
        public bool ContainsKey(ulong key) => _connections.ContainsKey(key);
        public IEnumerator<KeyValuePair<ulong, BusChannel>> GetEnumerator() => _connections.GetEnumerator();
        public bool TryGetValue(ulong key, out BusChannel value) => _connections.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => _connections.GetEnumerator();
    }

    public static partial class DrifterExtensions
    {

    }
}
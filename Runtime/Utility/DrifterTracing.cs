using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Drifter.Utility
{
    [HelpURL(url: "https://docs.google.com/document/d/1CvAClvFfyA5R-PhYUmn5OOQtYMH4h6I0nSsKchNAySU/preview")]
    public class DrifterTracer
    {
        private static DrifterTracer _instance = null;

        public static DrifterTracer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DrifterTracer();

                return _instance;
            }
        }

        ~DrifterTracer()
        {
            if (Instance == null)
                return;

            Instance.EndSession();
        }

        private readonly struct TraceSession
        {
            public readonly string name;

            public TraceSession(string name) => this.name = name;
        }

        private string currentSession = string.Empty;
        private StringBuilder stringBuilder;
        private StringWriter writer;
        private JsonTextWriter jsonWriter;

        public bool HasSession => !string.IsNullOrEmpty(currentSession);

        public readonly struct ProfileResult
        {
            public readonly string name;
            public readonly int threadID;
            public readonly DateTime startTime;
            public readonly DateTime endTime;

            public ProfileResult(string name, int threadID, DateTime startTime, DateTime endTime)
            {
                this.name = name;
                this.threadID = threadID;
                this.startTime = startTime;
                this.endTime = endTime;
            }
        }

        public void BeginSession(string name)
        {
            //TODO: Write to file path

            currentSession = name;
            stringBuilder = new StringBuilder();
            writer = new(stringBuilder);
            jsonWriter = new(writer);

            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName(name: "meta_user");
            jsonWriter.WriteValue(value: Environment.UserName);

            jsonWriter.WritePropertyName(name: "meta_cpu_count");
            jsonWriter.WriteValue(value: SystemInfo.processorCount);

            jsonWriter.WritePropertyName(name: "traceEvents");
            jsonWriter.WriteStartArray();

            jsonWriter.Flush();
        }

        public void EndSession()
        {
            if (!HasSession)
                return;

            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
            jsonWriter.Flush();
            jsonWriter.Close();

            currentSession = string.Empty;

            GUIUtility.systemCopyBuffer = stringBuilder.ToString();
            Debug.Break();
        }

        public void WriteProfile(ProfileResult result)
        {
            if (jsonWriter == null)
                return;

            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName(name: "cat");
            jsonWriter.WriteValue(value: "function");

            jsonWriter.WritePropertyName(name: "dur");
            jsonWriter.WriteValue(value: GetTime(result.endTime - result.startTime));

            jsonWriter.WritePropertyName(name: "name");
            jsonWriter.WriteValue(value: result.name);

            jsonWriter.WritePropertyName(name: "ph");
            jsonWriter.WriteValue(value: "X");

            jsonWriter.WritePropertyName(name: "pid");
            jsonWriter.WriteValue(value: 0ul);

            jsonWriter.WritePropertyName(name: "tid");
            jsonWriter.WriteValue(value: result.threadID);

            jsonWriter.WritePropertyName(name: "ts");
            jsonWriter.WriteValue(value: GetTime(new TimeSpan(result.startTime.Ticks)));
            jsonWriter.WriteEndObject();

            jsonWriter.Flush();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            double GetTime(TimeSpan span) => span.TotalMilliseconds * 1000d;
        }
    } 
}
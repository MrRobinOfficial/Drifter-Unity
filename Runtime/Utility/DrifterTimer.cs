using System;
using System.Diagnostics;
using UnityEngine;

namespace Drifter.Utility
{
    public class DrifterTraceTimer : IDisposable
    {
        private readonly DateTime startTime;
        private readonly string name;

        public DrifterTraceTimer()
        {
            var st = new StackTrace(frame: new StackFrame(skipFrames: 1));
            name = st.GetFrame(index: 0).GetMethod().Name;
            startTime = DateTime.Now;
        }

        public void Dispose()
        {
            var thread = System.Threading.Thread.CurrentThread;
            var endTime = DateTime.Now;

            var result = new DrifterTracer.ProfileResult(name, thread.ManagedThreadId, startTime, endTime);
            DrifterTracer.Instance.WriteProfile(result);
        }
    } 
}
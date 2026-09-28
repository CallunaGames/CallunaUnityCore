#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Calluna
{
    /// <summary>
    /// Editor-only aid for <see cref="Observable{T}"/> notifying only on actual changes. When enabled
    /// (menu Calluna > Diagnostics), it logs every place that sets an observable to its current value
    /// while listeners are subscribed - which notified them before. Each call site is logged once per
    /// play session, together with the listeners no longer being called, so code relying on the old
    /// behavior can be found and reviewed.
    /// </summary>
    public static class ObservableDiagnostics
    {
        public static bool LogUnchangedValues { get; set; }

        private static readonly Assembly _coreAssembly = typeof(Observable).Assembly;
        private static readonly HashSet<string> _reportedSites = new HashSet<string>();

        /// <summary>Logs every call site again. Called automatically when entering play mode.</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetReportedSites() => _reportedSites.Clear();

        internal static void ReportUnchangedValue(Type valueType, Delegate onChanged, Delegate onChangedWithValues)
        {
            StackTrace trace = new StackTrace(1, true);
            // The first frame outside this package - e.g. the class setting an AccumulatingValue part,
            // rather than AccumulatingValue itself.
            StackFrame caller = trace.GetFrames()?
                .FirstOrDefault(frame => frame.GetMethod()?.DeclaringType?.Assembly != _coreAssembly);
            string site = caller == null
                ? "unknown"
                : $"{OwnerOf(caller.GetMethod())}.{caller.GetMethod().Name} " +
                  $"({Path.GetFileName(caller.GetFileName())}:{caller.GetFileLineNumber()})";
            if (!_reportedSites.Add(site))
                return;

            string listeners = string.Join(", ", Describe(onChanged).Concat(Describe(onChangedWithValues)));
            Debug.LogWarning($"[ObservableDiagnostics] Observable<{valueType.Name}> set to its current value at " +
                             $"{site} - no longer notifies: {listeners}\n{trace}");
        }

        private static IEnumerable<string> Describe(Delegate listeners) =>
            listeners == null
                ? Enumerable.Empty<string>()
                : listeners.GetInvocationList().Select(listener => $"{OwnerOf(listener.Method)}.{listener.Method.Name}");

        // Lambdas live in compiler-generated nested classes - name the class that wrote them instead.
        private static string OwnerOf(MethodBase method)
        {
            Type type = method?.DeclaringType;
            while (type != null && type.IsDefined(typeof(CompilerGeneratedAttribute), false) && type.DeclaringType != null)
                type = type.DeclaringType;
            return type?.Name ?? "?";
        }
    }
}
#endif

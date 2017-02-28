using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RedmineSlackIntegration.DataUtils
{
    public class ReflectionProvider
    {
        public static string GetCallerNamespace(StackFrame stackFrame)
        {
            var stackFrameMethod = stackFrame.GetMethod();
            return stackFrameMethod?.DeclaringType?.Namespace;
        }

        public static List<string> GetCallerNameSpacePartsList(StackFrame stackFrame)
        {
            var ns = GetCallerNamespace(stackFrame);
            return ns?.Split('.').ToList();
        }
    }
}

using System.Diagnostics;

namespace RedmineSlackIntegration.DataUtils
{
    public class ReflectionProvider
    {
        public static string GetCallerNamespace(StackFrame stackFrame)
        {
            var stackFrameMethod = stackFrame.GetMethod();
            return stackFrameMethod?.DeclaringType?.Namespace;
        }
    }
}

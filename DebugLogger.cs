using MelonLoader;

namespace GetToModLib
{
    public static class DebugLogger
    {
        public static void Log(string message)
        {
            if (ModConfig.VerboseLogging?.Value ?? false)
                MelonLogger.Msg(message);
        }

        public static void LogWarning(string message)
        {
            if (ModConfig.VerboseLogging?.Value ?? false)
                MelonLogger.Warning(message);
        }

        public static void LogError(string message)
        {
            // Always log errors
            MelonLogger.Error(message);
        }

        public static void LogDeveloper(string message)
        {
            if (ModConfig.DeveloperMode?.Value ?? false)
                MelonLogger.Msg($"[DEV] {message}");
        }
    }
}
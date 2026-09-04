using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetToModLib
{
    public static class ModConfig
    {
        private static MelonPreferences_Category _category;

        // General
        public static MelonPreferences_Entry<bool> EnableMod;

        // Debug
        public static MelonPreferences_Entry<bool> DeveloperMode;
        public static MelonPreferences_Entry<bool> VerboseLogging;

        public static void Initialize()
        {
            _category = MelonPreferences.CreateCategory("Get To Mod Lib");

            // General
            EnableMod = _category.CreateEntry("EnableMod", true, "Enable Mod", "Master toggle for the mod");

            // Debug
            DeveloperMode = _category.CreateEntry("DeveloperMode", false, "Developer Mode", "Enable developer features");
            VerboseLogging = _category.CreateEntry("VerboseLogging", false, "Verbose Logging", "Enable detailed debug logging");

            _category.SaveToFile();
        }

        // Helper to check if mod is enabled
        public static bool IsEnabled => EnableMod?.Value ?? true;
    }
}

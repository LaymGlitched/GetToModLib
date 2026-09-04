using GetToModLib.UI;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(GetToModLib.Core), "Get To Mod Lib", "1.0.0", "itslaymxd", null)]
[assembly: MelonGame("Isto", "Get To Work")]

namespace GetToModLib
{
    public class Core : MelonMod
    {
        public List<GetToMod> LoadedMods = new List<GetToMod>();

        public static Core Instance { get; private set; }

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            Instance = this;

            GameObject modManager = new GameObject("Mod Manager");
            modManager.AddComponent<ModUIManager>();
            GameObject.DontDestroyOnLoad(modManager);


            // detect new mods and add them to the list
        }
    }
}
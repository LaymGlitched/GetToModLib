using MelonLoader;

[assembly: MelonInfo(typeof(GetToModLib.Core), "Get To Mod Lib", "1.0.0", "itslaymxd", null)]
[assembly: MelonGame("Isto", "Get To Work")]

namespace GetToModLib
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }
}
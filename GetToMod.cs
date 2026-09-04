using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetToModLib
{
    public class GetToMod
    {
        /// <summary>
        /// The name of the mod.
        /// </summary>
        public string name = "Mod Name";

        /// <summary>
        /// The author of the mod.
        /// </summary>
        public string author = "Author Name";

        /// <summary>
        /// The linked MelonMod instance for this mod. This is used to access the mod's functionality and data.
        /// </summary>
        public MelonMod linkedMelonMod;

        /// <summary>
        /// The version of the mod.
        /// </summary>
        public Version version = new Version(1, 0, 0);

        /// <summary>
        /// A 3-letter word that adds a bit of flair to the mod's name. This is optional and can be left empty if not desired.
        /// This appears in the mod list. In the base game, this is used next to the Doinkler Portfolio as 'NEW!'.
        /// </summary>
        public string flair = "NEW!";
    }
}

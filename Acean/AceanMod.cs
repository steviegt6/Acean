using System;
using Acean;
using MelonLoader;

[assembly: MelonInfo(typeof(AceanMod), "Acean", "0.1.0", "Tomat")]
[assembly: MelonGame("Dani", "Crab Game")]

namespace Acean
{
    public class AceanMod : MelonMod
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();

            MelonLogger.Msg(ConsoleColor.Yellow, "Application started.");
        }
    }
}

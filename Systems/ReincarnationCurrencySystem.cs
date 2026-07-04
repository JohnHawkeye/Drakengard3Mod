using Terraria.GameContent.UI;
using Terraria.ModLoader;
using Drakengard3Mod.Currencies;


namespace Drakengard3Mod.Systems
{
    public class ReincarnationCurrencySystem : ModSystem
    {
        public static int CurrencyID { get; internal set; }

        public override void Load()
        {
            CurrencyID = CustomCurrencyManager.RegisterCurrency(
                new ReincarnationCurrency()
                );
        }

        public override void Unload()
        {
            CurrencyID = 0;
        }
    }
}
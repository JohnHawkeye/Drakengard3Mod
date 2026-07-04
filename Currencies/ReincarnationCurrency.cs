using Terraria.GameContent.UI;
using Terraria.ModLoader;
using Drakengard3Mod.Items;

namespace Drakengard3Mod.Currencies
{
    public class ReincarnationCurrency : CustomCurrencySingleCoin
    {
        public ReincarnationCurrency()
            : base(ModContent.ItemType<ReincarnationSoul>(), 999999L)
        {
            CurrencyTextKey = "Mods.Drakengard3Mod.Currency.ReincarnationSoul";
        }
    }
}
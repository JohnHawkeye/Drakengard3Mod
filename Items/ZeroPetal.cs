using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Items
{
    public class ZeroPetal : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
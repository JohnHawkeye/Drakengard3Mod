using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class OneCosABoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            // 日本語化はLocalizationファイルを使う場合は不要
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;

            Item.value = Item.sellPrice(copper: 10);
            Item.rare = ItemRarityID.Purple;

            Item.vanity = true;
        }
    }
}
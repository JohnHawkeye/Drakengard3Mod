using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class OneCosBCoat : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;

            Item.value = Item.sellPrice(copper: 10);
            Item.rare = ItemRarityID.Purple;

            Item.vanity = true; 
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
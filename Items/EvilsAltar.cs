using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Items
{
    public class EvilsAltar : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 999;

            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.consumable = true;

            Item.value = Item.buyPrice(silver: 50);

            Item.createTile = ModContent.TileType<Tiles.EvilsAltarTile>();
        }

        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ItemID.RottenChunk, 10)
                .AddIngredient(ItemID.EbonstoneBlock, 10)
                .AddTile(TileID.Furnaces)
                .Register();

            Recipe.Create(Type)
                .AddIngredient(ItemID.Vertebrae, 10)
                .AddIngredient(ItemID.CrimstoneBlock, 10)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
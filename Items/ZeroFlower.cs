using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.States;

namespace Drakengard3Mod.Items
{
    public class ZeroFlower : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;

            Item.maxStack = 20;
            Item.consumable = true;

            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.UseSound = SoundID.Roar;

            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool CanUseItem(Player player)
        {
            // ZeroBossが既に存在するなら使用不可
            return !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.ZeroBoss>());
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 playerPos = player.Center;
                int direction = Main.rand.NextBool() ? -1 : 1;
                int distanceBlocks = 26;
                int distancePixels = distanceBlocks * 16;
                int spawnX = (int)playerPos.X + direction * distancePixels;
                int spawnY = (int)playerPos.Y - 384;

                NPC.NewNPC(player.GetSource_ItemUse(Item),
                    spawnX, spawnY,
                    ModContent.NPCType<NPCs.Bosses.ZeroBoss>(),
                    Target: player.whoAmI);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ZeroPetal>(), 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
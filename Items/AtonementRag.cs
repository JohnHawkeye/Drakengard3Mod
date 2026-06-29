using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Items
{
    public class AtonementRag : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useTime = 15;
            Item.useAnimation = 15;

            Item.useStyle = ItemUseStyleID.Swing;

            Item.UseSound = SoundID.Item1;

            Item.autoReuse = true;


            Item.value = Item.buyPrice(copper: 10);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool? UseItem(Player player)
        {
            CleanPaint(player);

            return true;
        }

        private void CleanPaint(Player player)
        {
            int centerX = (int)(player.Center.X / 16);
            int centerY = (int)(player.Center.Y / 16);

            int radius = 10;

            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;

                    WorldGen.paintTile(x, y, 0);
                    WorldGen.paintWall(x, y, 0);
                }
            }
        }
    }
}
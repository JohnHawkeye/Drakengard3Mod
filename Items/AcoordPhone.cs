using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Drakengard3Mod.NPCs;

namespace Drakengard3Mod.Items
{
    public class AcoordPhone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useTurn = true;
            Item.autoReuse = false;

            Item.UseSound = SoundID.Item6;

            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 5);

            Item.consumable = false;
        }

        public override bool? UseItem(Player player)
        {
            // 既にAcoordが存在するか確認
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == ModContent.NPCType<Acoord>())
                {
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Main.NewText("あら、私はここにいますよ。");
                    }

                    return true;
                }
            }

            // プレイヤーの少し右側に召喚
            Vector2 spawnPos = player.Center + new Vector2(64f, 0f);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC(
                    player.GetSource_ItemUse(Item),
                    (int)spawnPos.X,
                    (int)spawnPos.Y,
                    ModContent.NPCType<Acoord>());
            }

            return true;
        }
    }
}
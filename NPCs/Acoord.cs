using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.NPCs
{
    [AutoloadHead]
    public class Acoord : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 23;

            NPCID.Sets.ExtraFramesCount[Type] = 0;
            NPCID.Sets.AttackFrameCount[Type] = 1;
            NPCID.Sets.DangerDetectRange[Type] = 500;
            NPCID.Sets.AttackType[Type] = 1;
            NPCID.Sets.AttackTime[Type] = 30;
            NPCID.Sets.AttackAverageChance[Type] = 10;
            NPCID.Sets.HatOffsetY[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 48;

            NPC.aiStyle = NPCAIStyleID.Passive; // Town NPC

            NPC.damage = 0;
            NPC.defense = 25;
            NPC.lifeMax = 250;

            NPC.friendly = true;
            NPC.townNPC = true;

            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;
        }

        // 無条件で住人になる
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return false;
        }
        // 名前
        public override string GetChat()
        {
            return "こんにちは！アコール商店をお呼びですか？";
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "ショップ";
            button2 = "帰ってください";
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = "Shop";
            }
            else
            {
                Main.NewText("アコールはどこか別の次元へ旅立った。");

                NPC.active = false;
            }
        }

        public override void AddShops()
        {
            NPCShop shop = new NPCShop(Type);

            // ここに販売アイテムを追加
            shop.Add(ItemID.Torch);
            shop.Add(ItemID.Bomb);
            shop.Add(ItemID.Rope);
            shop.Add(ItemID.HealingPotion);
            shop.Add(ItemID.ManaPotion);

            shop.Register();
        }
    }
}
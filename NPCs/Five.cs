using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.NPCs
{
    [AutoloadHead]
    public class Five : ModNPC
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
            return true;
        }

        // 名前
        public override string GetChat()
        {
            return "元気？私はいつも元気だよ！";
        }
    }
}
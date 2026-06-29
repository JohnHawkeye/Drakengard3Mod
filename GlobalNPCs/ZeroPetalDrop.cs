using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.GlobalNPCs
{
    public class ZeroPetalDrop : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.boss)
            {
                npcLoot.Add(
                    ItemDropRule.Common(
                        ModContent.ItemType<Items.ZeroPetal>(),
                        1,      // 必ずドロップ
                        1,      // 最小1個
                        3));    // 最大3個
            }
        }
    }
}
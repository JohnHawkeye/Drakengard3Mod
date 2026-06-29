using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

namespace Drakengard3Mod.GlobalNPCs
{
    public class ReincarnationSoulDrop : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (npc.friendly || npc.townNPC)
                return;

            int amount;
            
            if (npc.boss)
            {
                amount = Main.rand.Next(25, 36); // 25～35
            }
            else
            {
                amount = 1;
            }

            Item.NewItem(
                npc.GetSource_Loot(),
                npc.Hitbox,
                ModContent.ItemType<Items.ReincarnationSoul>(),
                amount
            );
        }
    }
}
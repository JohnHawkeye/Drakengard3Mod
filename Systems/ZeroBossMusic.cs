using Terraria;
using Terraria.ModLoader;

namespace Drakengard3Mod.Systems
{
    public class ZeroBossMusic : ModSceneEffect
    {
        public override int Music =>
            MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/kuroihana");

        public override SceneEffectPriority Priority =>
            SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            return NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.ZeroBoss>());
        }
    }
}
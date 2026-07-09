using Terraria;
using Terraria.ModLoader;
using Drakengard3Mod.Players;

namespace Drakengard3Mod.Systems
{
    public class MusicSystem : ModSceneEffect
    {
        public override int Music
        {
            get
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/kuroihana");
            }
        }

        public override SceneEffectPriority Priority
            => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            return player.GetModPlayer<DODPlayer>().UtautaiMode;
        }
    }
}
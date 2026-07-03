using Terraria;
using Terraria.ModLoader;

namespace Drakengard3Mod.Players
{
    public class OnePlayer : ModPlayer
    {
        // ワン姉さんが召喚中か
        public bool oneMinion;

        public override void ResetEffects()
        {
            oneMinion = false;
        }

        public override void UpdateDead()
        {
            oneMinion = false;
        }

        public override void PostUpdate()
        {
            if (oneMinion)
            {
                Player.findTreasure = true;
                Player.dangerSense = true;
            }
        }
    }
}
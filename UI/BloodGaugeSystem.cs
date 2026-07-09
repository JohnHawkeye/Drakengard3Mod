using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Drakengard3Mod.Players;

namespace Drakengard3Mod.UI
{
    public class BloodGaugeSystem : ModSystem
    {
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (player == null || !player.active)
                return;

            DODPlayer modPlayer = player.GetModPlayer<DODPlayer>();

            string text = $"Blood : {modPlayer.BloodGauge}/{DODPlayer.MaxBloodGauge}";

            Color color = modPlayer.UtautaiMode
                ? Color.Red
                : Color.White;

            Utils.DrawBorderString(
                spriteBatch,
                text,
                new Vector2(30, 30),
                color);
        }
    }
}
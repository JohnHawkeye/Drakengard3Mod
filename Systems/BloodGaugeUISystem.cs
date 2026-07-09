using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Drakengard3Mod.Players;

namespace Drakengard3Mod.Systems
{
    public class BloodGaugeUISystem : ModSystem
    {
        private static Asset<Texture2D> frameTexture;
        private static Asset<Texture2D> barTexture;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                frameTexture = ModContent.Request<Texture2D>("Drakengard3Mod/Assets/UI/BloodGaugeFrame");
                barTexture = ModContent.Request<Texture2D>("Drakengard3Mod/Assets/UI/BloodGaugeBar");
            }
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (!player.active || player.dead)
                return;

            DODPlayer modPlayer = player.GetModPlayer<DODPlayer>();

            // 左上座標
            Vector2 pos = new Vector2(1200, 20);

            // 枠
            spriteBatch.Draw(
                frameTexture.Value,
                pos,
                Color.White);

            // ゲージ割合
            float percent = (float)modPlayer.BloodGauge / DODPlayer.MaxBloodGauge;
            percent = MathHelper.Clamp(percent, 0f, 1f);

            Texture2D bar = barTexture.Value;

            int drawWidth = (int)(bar.Width * percent);

            if (drawWidth > 0)
            {
                Rectangle source = new Rectangle(
                    0,
                    0,
                    drawWidth,
                    bar.Height);

                Vector2 barPos = pos + new Vector2(10, 10);

                Color color = Color.White;

                // ウタウタイ中は点滅
                if (modPlayer.UtautaiMode)
                {
                    float pulse = (float)((System.Math.Sin(Main.GlobalTimeWrappedHourly * 8f) + 1f) * 0.5f);
                    color = Color.Lerp(Color.Red, Color.White, pulse);
                }

                spriteBatch.Draw(
                    bar,
                    barPos,
                    source,
                    color);
            }

            // 数値表示
            Utils.DrawBorderString(
                spriteBatch,
                $"{modPlayer.BloodGauge}/{DODPlayer.MaxBloodGauge}",
                pos + new Vector2(125, 7),
                Color.White);

        }
    }
}
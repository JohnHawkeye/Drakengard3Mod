using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drakengard3Mod.Projectiles
{
    public class SoulMissile : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.penetrate = 1;

            Projectile.timeLeft = 300;
        }


        public override void AI()
        {
            NPC target = null;
            float distance = 500f;

            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy())
                {
                    float d = Vector2.Distance(
                            Projectile.Center,
                            npc.Center
                    );

                    if (d < distance)
                    {
                        distance = d;
                        target = npc;
                    }
                }
            }


            if (target != null)
            {
                Vector2 direction =
                    target.Center - Projectile.Center;

                direction.Normalize();

                Projectile.velocity =
                    Vector2.Lerp(
                        Projectile.velocity,
                        direction * 14f,
                        0.08f
                    );

                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            Dust dust = Dust.NewDustDirect(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.BlueTorch);
            dust.noGravity = true;
            dust.scale = 1.2f;
            dust.velocity *= 0.2f;

            Lighting.AddLight(
                Projectile.Center,
                0.2f,   // 赤
                0.5f,   // 緑
                1.2f    // 青
                );

            Projectile.rotation = Projectile.velocity.ToRotation();

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture =
                Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            Vector2 origin = texture.Size() / 2f;

            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                Vector2 drawPos =
                    Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;

                Color color = Color.Cyan * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length) * 0.6f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    color,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0);
            }

            return true;
        }
    }
}
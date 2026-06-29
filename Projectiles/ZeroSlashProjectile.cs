using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace Drakengard3Mod.Projectiles
{
    public class ZeroSlashProjectile : ModProjectile
    {
        private const float SwingStart = -90f;
        private const float SwingEnd = 90f;
        private const float SwingSpeed = 15f;

        private float angle;

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 12;

            Projectile.hostile = true;
            Projectile.friendly = false;

            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                angle = MathHelper.ToRadians(SwingStart);
            }

            if (Projectile.ai[0] < 0 || Projectile.ai[0] >= Main.maxNPCs)
            {
                Projectile.Kill();
                return;
            }

            NPC npc = Main.npc[(int)Projectile.ai[0]];

            if (!npc.active)
            {
                Projectile.Kill();
                return;
            }

            angle += MathHelper.ToRadians(SwingSpeed);

            if (angle >= MathHelper.ToRadians(SwingEnd))
            {
                Projectile.Kill();
                return;
            }

            float finalAngle = angle;

            if (npc.direction == -1)
                finalAngle = MathHelper.Pi - angle;

            Projectile.rotation = finalAngle;

            Vector2 handOffset = new Vector2(16f * npc.direction, -12f);

            Projectile.Center =
                npc.Center +
                handOffset +
                finalAngle.ToRotationVector2() * 24f;

            Projectile.spriteDirection = npc.direction;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            SpriteEffects effects =
                Projectile.spriteDirection == 1 ?
                SpriteEffects.None :
                SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                new Vector2(4, texture.Height / 2),
                1f,
                effects,
                0);

            return false;
        }
    }
}
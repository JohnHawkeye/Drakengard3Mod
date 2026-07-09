using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Players
{
    public class DODPlayer : ModPlayer
    {
        // 最大ブラッドゲージ
        public const int MaxBloodGauge = 100;

        // 現在のブラッドゲージ
        public int BloodGauge = 0;

        // ウタウタイモード中か
        public bool UtautaiMode = false;

        // 残り時間（フレーム）
        //public int UtautaiTimer = 0;
        private int bloodDecayTimer = 0;

        public override void ResetEffects()
        {
            if (UtautaiMode)
            {
                Player.armorEffectDrawShadow = true;
                Player.armorEffectDrawOutlines = true;
            }
        }

        public override void PostUpdate()
        {
            if (UtautaiMode)
            {
                if (Player.dead)
                {
                    if (UtautaiMode || BloodGauge > 0)
                    {
                        EndUtautaiMode();
                    }

                    return;
                }

                Player.armorEffectDrawShadow = true;
                Player.armorEffectDrawOutlines = true;

                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.GemRuby);

                    Main.dust[dust].velocity *= 0.2f;
                    Main.dust[dust].scale = 1.6f;
                    Main.dust[dust].noGravity = true;
                }
                // 赤いライト
                Lighting.AddLight(Player.Center, 1.0f, 0.1f, 0.1f);

                bloodDecayTimer++;
                if (bloodDecayTimer >= 12)
                {
                    bloodDecayTimer = 0;
                    BloodGauge--;
                    if (BloodGauge <= 0)
                    {
                        BloodGauge = 0;
                        EndUtautaiMode();
                    }
                }
            }
        }

        /// <summary>
        /// ブラッドゲージを増やす
        /// </summary>
        public void AddBlood(int amount)
        {
            BloodGauge += amount;

            if (BloodGauge > MaxBloodGauge)
                BloodGauge = MaxBloodGauge;

            if (!UtautaiMode && BloodGauge >= MaxBloodGauge)
            {
                ActivateUtautaiMode();
            }
        }

        /// <summary>
        /// ウタウタイモード開始
        /// </summary>
        private void ActivateUtautaiMode()
        {
            UtautaiMode = true;
            bloodDecayTimer = 0;
        }

        /// <summary>
        /// ウタウタイモード終了
        /// </summary>
        private void EndUtautaiMode()
        {
            UtautaiMode = false;
            BloodGauge = 0;
            bloodDecayTimer = 0;
        }

        //能力アップ
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (UtautaiMode)
            {
                damage *= 2f;
            }
        }
        public override void PostUpdateRunSpeeds()
        {
            if (UtautaiMode)
            {
                Player.moveSpeed *= 1.5f;
                Player.maxRunSpeed *= 1.5f;

                Player.accRunSpeed *= 1.5f;
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (UtautaiMode)
            {
                Player.wingTimeMax = (int)(Player.wingTimeMax * 1.5f);
            }
        }
    }
}
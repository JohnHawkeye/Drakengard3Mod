using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Drakengard3Mod.Items;
using Drakengard3Mod.Items.Armor;
using Drakengard3Mod.Currencies;
using Drakengard3Mod.Systems;
using Drakengard3Mod.Items.Weapons;
using Drakengard3Mod.Items.Summons;
using Microsoft.Xna.Framework;
using Drakengard3Mod.Buffs;
using Drakengard3Mod.Projectiles;
//test
namespace Drakengard3Mod.NPCs
{
    [AutoloadHead]
    public class One : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;

            NPCID.Sets.ExtraFramesCount[Type] = 0;
            NPCID.Sets.AttackFrameCount[Type] = 1;
            NPCID.Sets.DangerDetectRange[Type] = 500;
            NPCID.Sets.AttackType[Type] = 1;
            NPCID.Sets.AttackTime[Type] = 30;
            NPCID.Sets.AttackAverageChance[Type] = 10;
            NPCID.Sets.HatOffsetY[Type] = 4;

        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;

            NPC.width = 20;
            NPC.height = 20;

            NPC.aiStyle = NPCAIStyleID.Passive;

            NPC.defense = 35;
            NPC.lifeMax = 300;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;

        }
        public override void AI()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.active || player.dead)
                    continue;

                // プレイヤーとの距離
                float xDist = Math.Abs(player.Center.X - NPC.Center.X);
                float yDist = Math.Abs(player.Center.Y - NPC.Center.Y);

                if (xDist < 48f && yDist < 24f)
                {
                    // ラブラブなハートを出す
                    if (Main.rand.NextBool(40))
                    {
                        Vector2 pos = NPC.Center +
                            new Vector2(
                                Main.rand.NextFloat(-8f, 8f),
                                -24f);

                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            pos,
                            new Vector2(
                                Main.rand.NextFloat(-1f, 1f),
                                -0.5f),
                            ModContent.ProjectileType<LoveHeart>(),
                            0,
                            0f,
                            Main.myPlayer);
                    }

                    // バフ付与
                    player.AddBuff(ModContent.BuffType<OneLoveBuff>(), 2);
                }
            }
        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            int soulType = ModContent.ItemType<ReincarnationSoul>();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player != null && player.active && player.HasItem(soulType))
                {
                    return true;
                }
            }
            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "ワン姉さん"
            };
        }

        public override string GetChat()
        {
            return Main.rand.Next(3) switch
            {
                0 => "ゼロを倒すために君にはもっと強くなってもらわなくてはならない。",
                1 => "この世界のボスを倒して強くなるんだ。",
                2 => "夜になったら帰ってくるんだ。深い意味は…ない。",
                _ => "あまり…遠くに行かないでくれ…。"
            };
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "交換ショップ";
            button2 = "ショップについて";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = "Shop";
            }
            else
            {

                Main.npcChatText =
                    "君がモンスターを倒すと輪廻の魂を手に入れられるはずだ。\nそれと見合った特別なアイテムと交換してあげよう。";
            }
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type);
            //costume
            npcShop.Add(new Item(ModContent.ItemType<OneCosAWig>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<OneCosACoat>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<OneCosABoots>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<OneCosBWig>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<OneCosBCoat>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<OneCosBBoots>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });

            //weapons
            npcShop.Add(new Item(ModContent.ItemType<OneChakramItem>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<SoulCalibur>())
            {
                shopCustomPrice = 100,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<SoulPunisher>())
            {
                shopCustomPrice = 100,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<SoulStaff>())
            {
                shopCustomPrice = 100,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });

            //summons
            npcShop.Add(new Item(ModContent.ItemType<OneEarring>())
            {
                shopCustomPrice = 100,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<GabriellaEgg>())
            {
                shopCustomPrice = 500,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });

            //Pots
            npcShop.Add(new Item(ModContent.ItemType<AngelProtectionPotion>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });

            //zakka
            npcShop.Add(new Item(ModContent.ItemType<AtonementRag>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<AcoordPhone>())
            {
                shopCustomPrice = 50,
                shopSpecialCurrency = ReincarnationCurrencySystem.CurrencyID
            });
            npcShop.Register();
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 15;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 5;
            randExtraCooldown = 10;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<Projectiles.OneChakram>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(
            ref float multiplier,
            ref float gravityCorrection,
            ref float randomOffset)
        {
            multiplier = 7f;
        }
    }
}
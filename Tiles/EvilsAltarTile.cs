using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;

namespace Drakengard3Mod.Tiles
{
    public class EvilsAltarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);

            TileObjectData.newTile.Origin = new Point16(0, 2);

            TileObjectData.addTile(Type);
            
            RegisterItemDrop(ModContent.ItemType<Items.EvilsAltar>());

            DustType = DustID.Corruption;

            AddMapEntry(new Color(120, 40, 120), CreateMapEntryName());

            AdjTiles = new int[]
            {
                TileID.DemonAltar
            };
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

    }
}
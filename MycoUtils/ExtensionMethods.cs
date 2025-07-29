using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Terraria;

namespace Mycology.MycoUtils
{
    public static class ExtensionMethods
    {
        public static bool IsPlayerGrounded(this Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                int tileX = (int)((player.position.X + (player.width * i / 2f)) / 16f);
                int tileY = (int)((player.position.Y + player.height + 1) / 16f);
                Tile tile = Framing.GetTileSafely(tileX, tileY);
                if (tile != null && tile.HasTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

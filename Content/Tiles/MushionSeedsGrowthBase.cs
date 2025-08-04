using Mycology.Content.Items.Accessories;
using Mycology.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using ReLogic.Content;

namespace Mycology.Content.Tiles
{
    public abstract class MushionSeedsGrowthBase : ModTileEntity
    {
        public int phase = 0;
        public float growth = 0;
        public int growRate = 3600;
        public float colonyGlowModifier = 0;
        public int owner;
        public int colonyID = 0;
        public Color glowColor;
        protected Color baseColor;
        public string ownerName;
        protected abstract Color DefaultBaseColor { get; }
        protected abstract int ProjectileID { get; }
        public MushionSeedsGrowthBase()
        {
            growRate = Main.rand.Next(3000, 4200);
            baseColor = DefaultBaseColor;
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == ModContent.TileType<MushionSeedsTileBase>();
        }
        public static bool HasMud(int i, int j)
        {
            int tilledMud = ModContent.TileType<TilledMud>();
            Tile[] tiles =
            [
                Framing.GetTileSafely(i, j + 1),
                Framing.GetTileSafely(i + 1, j + 1),
                Framing.GetTileSafely(i, j + 2),
                Framing.GetTileSafely(i + 1, j + 2)
            ];
            int mudTiles = tiles.Where(tile => tile.HasTile && (tile.TileType == tilledMud)).Count();
            return mudTiles == 4;
        }
        public bool HasHeadroom(int i, int j) //call from the bottom left corner
        {
            Tile[] tiles =
            [
                Framing.GetTileSafely(i, j - 1),
                Framing.GetTileSafely(i + 1, j - 1),
                Framing.GetTileSafely(i, j - 2),
                Framing.GetTileSafely(i + 1, j - 2)
            ];
            int airTiles = tiles.Where(tile => !tile.HasTile).Count();
            return airTiles == 4;
        }
        private bool grown = false;

        private bool clickDebounce = false;
        public override void Update()
        {
            phase = (int)Math.Floor(growth);

            MushionColony colony = MushionColonySystem.GetOrSetColony(ID, Position.X, Position.Y);
            if (colonyID == ID) colony.GlowColor = glowColor;
            if (colonyID == 0)
            {
                colonyID = colony.Id;
            }

            if (!MushionColonySystem.IsValidForGrowing(Position.X, Position.Y) && growth < 3.999f)
            {
                growth = Math.Max(growth - 40f / growRate, 0);
                return;
            }

            growth = Math.Min(growth + 5f / growRate, 3.999f);
            Player plr = Main.player.FirstOrDefault(p => p.name == ownerName);

            if (growth >= 3.999f && !grown)
            {
                if (plr == null) return;
                CombatText.NewText(plr.Hitbox, Color.White, "Your Mushion has grown!", false, false);
                grown = true;
            }
            if (Main.MouseWorld.Distance(Position.ToWorldCoordinates(16, 16)) < 20 && Main.mouseRight && !clickDebounce)
            {
                if (plr == null) return;
                int Mushions = Main.projectile.Where(proj => proj.active && proj.owner == plr.whoAmI && MycologyPlayer.projectileTypes.Contains(proj.type)).Count();

                int max = plr.GetModPlayer<MycologyPlayer>().MaxMushions;

                if (Mushions < max)
                {
                    Projectile.NewProjectile(plr.GetSource_FromThis(), Position.ToWorldCoordinates(16, 16), new Vector2(0, -5), ProjectileID, 1, 0, plr.whoAmI);
                    if (Main.rand.NextBool(10))
                    {
                        Point worldPos = Position.ToWorldCoordinates(0, 0).ToPoint();
                        Item.NewItem(Item.GetSource_NaturalSpawn(),
                            new Rectangle(worldPos.X, worldPos.Y, 32, 32),
                            new Item(ModContent.ItemType<Sporepack>(), 1, -1));
                    }
                    WorldGen.KillTile(Position.X, Position.Y, false, false, true);
                } else
                {
                    CombatText.NewText(plr.Hitbox, Color.White, "You can only have up to " + max + " mushions at one time!", false, false);
                }
                clickDebounce = true;
            }
            if (!Main.mouseRight) clickDebounce = false;

            NetMessage.SendData(MessageID.TileEntitySharing, number: ID); //been told this is important for tileEntities 

            glowColor = baseColor;
            float amount = MathHelper.Lerp(0.25f, 0.75f, growth / 4);
            amount = MushionSeedsTileBase.GlowAlpha(amount) * 0.5f;
            glowColor *= colony.GlowModifier * amount;
        }
        public override void SaveData(TagCompound tag)
        {
            tag[nameof(growth)] = growth;
            tag[nameof(growRate)] = growRate;
            tag[nameof(owner)] = owner;
            tag[nameof(ownerName)] = ownerName;
            tag[nameof(grown)] = grown;
        }
        public override void LoadData(TagCompound tag)
        {
            growth = tag.GetFloat(nameof(growth));
            growRate = tag.GetInt(nameof(growRate));
            owner = tag.GetInt(nameof(owner));
            ownerName = tag.GetString(nameof(ownerName));
            grown = tag.GetBool(nameof(grown));
        }
        public override void NetSend(BinaryWriter writer) => writer.Write(growth);
        public override void NetReceive(BinaryReader reader) => growth = reader.ReadSingle();
    }
    public abstract class MushionSeedsTileBase : ModTile
    {
        private static Texture2D glowTexture;
        public override void Load()
        {
            On_Player.PlaceThing_Tiles_PlaceIt += StorePlayer;
            glowTexture = ModContent.Request<Texture2D>(Texture + "_glow", AssetRequestMode.ImmediateLoad).Value;
        }
        public override void Unload()
        {
            On_Player.PlaceThing_Tiles_PlaceIt -= StorePlayer;
        }
        protected string OwnerName;
        private TileObject StorePlayer(On_Player.orig_PlaceThing_Tiles_PlaceIt orig, Player self, bool newObjectType, TileObject data, int tileToCreate)
        {
            if (data.type == Type) OwnerName = self.name;
            return orig(self, newObjectType, data, tileToCreate);
        }

        public abstract override string Texture { get; }
        public override bool CanPlace(int i, int j) => MushionSeedsGrowthBase.HasMud(i, j);
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileHammeringIfOnTopOfIt[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);

            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = GetTileEntityInstance().Generic_HookPostPlaceMyPlayer;

            TileObjectData.addTile(Type);
        }
        public override bool CanDrop(int i, int j)
        {
            return false;
        }
        protected abstract MushionSeedsGrowthBase GetTileEntityInstance();
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            GetTileEntityInstance().Kill(i, j);
        }
        public static Point GetTopLeft(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            int left = i - (tile.TileFrameX / 18) % 2;
            int top = j - (tile.TileFrameY / 18) % 2;
            return new Point(left, top);
        }
        #region drawing
        public static float GlowAlpha(float modifier)
        {
            int tickCycle = 500;
            float alpha = 0.5f + (float)Math.Sin(Main.timeForVisualEffects / tickCycle * MathHelper.TwoPi) / 2; //make the sine 0-1

            return MathHelper.Lerp(modifier / 2, modifier, alpha);
        }
        protected Color glowColor;
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY += 4;

            int phase = GetPhaseFromCoordinates(i, j);
            GetFrameYOffset(ref tileFrameY, 34, phase);
        }
        public void GetFrameYOffset(ref short frameY, int frameHeight, int phase)
        {
            frameY += (short)(phase * frameHeight);
        }
        public int GetPhaseFromCoordinates(int x, int y)
        {
            var topleft = GetTopLeft(x, y);
            if (TileEntity.TryGet(topleft.X, topleft.Y, out MushionSeedsGrowthBase entity))
            {
                glowColor = entity.glowColor; //just doing this here for convenience
                if (entity.ownerName is "" or null) entity.ownerName = OwnerName;
                return entity.phase;
            }
            return 0;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            ModTile modTile = TileLoader.GetTile(tile.TileType);

            int glowFrameX = tile.TileFrameX / 18;
            glowFrameX *= 26;
            int frameYint = tile.TileFrameY;
            short glowFrameY = (short)((short)(frameYint / 18) * 26);

            int phase = GetPhaseFromCoordinates(i, j);
            GetFrameYOffset(ref glowFrameY, 42, phase);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;
            drawPos.Y += 4;

            Vector2 glowPos = drawPos;

            Rectangle glowSource = new(glowFrameX, glowFrameY, 26, 26);
            if (glowFrameX == 0) glowPos.X -= 8;
            if (tile.TileFrameY == 0) glowPos.Y -= 8;

            glowColor.A *= 0;
            spriteBatch.Draw(glowTexture, glowPos, glowSource, glowColor, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);

            int frameX = tile.TileFrameX;
            short frameY = tile.TileFrameY;
            GetFrameYOffset(ref frameY, 34, phase);
            Rectangle source = new(frameX, frameY, 16, 16);
            Color color = Lighting.GetColor(i, j);
            color.MultiplyRGB(new Color(2, 2, 2));

            Texture2D tex = TextureAssets.Tile[tile.TileType].Value;
            spriteBatch.Draw(tex, drawPos, source, color, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            return true;
        }
        /*
        private void DrawGlow(On_Main.orig_DrawTiles orig, Main self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            int minX = Main.screenPosition.ToPoint().X / 16 - 1;
            int maxX = minX + Main.screenWidth / 16 + 1;
            int minY = Main.screenPosition.ToPoint().Y / 16 - 1;
            int maxY = minY + Main.screenHeight / 16 + 1;
            int Type = ModContent.TileType<MushionSeedsTileBase>();


            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!tile.HasTile) continue;

                    ModTile modTile = TileLoader.GetTile(tile.TileType);
                    if (modTile is not MushionSeedsTileBase mTile) continue;

                    int glowFrameX = tile.TileFrameX / 18;
                    glowFrameX *= 26;
                    int frameYint = tile.TileFrameY;
                    short glowFrameY = (short)((short)(frameYint / 18) * 26);

                    int phase = GetPhaseFromCoordinates(x, y);
                    GetFrameYOffset(ref glowFrameY, 42, phase);
                    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                    Vector2 drawPos = new Vector2(x * 16, y * 16) - Main.screenPosition + zero;
                    drawPos.Y += 4;

                    Vector2 glowPos = drawPos;

                    Rectangle glowSource = new(glowFrameX, glowFrameY, 26, 26);
                    if (glowFrameX == 0) glowPos.X -= 8;
                    if (tile.TileFrameY == 0) glowPos.Y -= 8;

                    Texture2D glowTex = TextureAssets.GlowMask[mTile.Type].Value;
                    Main.spriteBatch.Draw(glowTex, glowPos, glowSource, glowColor, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!tile.HasTile) continue;

                    ModTile modTile = TileLoader.GetTile(tile.TileType);
                    if (modTile is not MushionSeedsTileBase mTile) continue;

                    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                    Vector2 drawPos = new Vector2(x * 16, y * 16) - Main.screenPosition + zero;
                    drawPos.Y += 4;

                    int frameX = tile.TileFrameX;
                    short frameY = tile.TileFrameY;
                    int phase = GetPhaseFromCoordinates(x, y);
                    GetFrameYOffset(ref frameY, 34, phase);
                    Rectangle source = new(frameX, frameY, 16, 16);
                    Color color = Lighting.GetColor(x, y);
                    color.MultiplyRGB(new Color(2,2,2));

                    Texture2D tex = TextureAssets.Tile[mTile.Type].Value;
                    Main.spriteBatch.Draw(tex, drawPos, source, color, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
                }
            }
            orig(self, solidLayer, forRenderTargets, intoRenderTargets, waterStyleOverride);
        }
        */
        #endregion
    }
    public class MushionColony(int id)
    {
        public int Id { get; } = id;
        public HashSet<TileEntity> Members { get; } = [];
        public void AddMember(TileEntity member) => Members.Add(member);
        public void RemoveMember(TileEntity member) => Members.Remove(member);
        public bool Contains(TileEntity member) => Members.Contains(member);
        public int Size => Members.Count;
        public Color GlowColor { get; set; }
        public float GlowModifier { get; set; } = 0;
        public int Leader => Members.OrderBy(m => ((MushionSeedsGrowthBase)m).growth).FirstOrDefault()?.ID ?? -1; //get highest growth as 'leader'
        public void Update()
        {
            GlowModifier = MathHelper.Lerp(GlowModifier, Members.Count < 3 ? 0 : 0.9f, 0.15f);
            if (Members.Count == 0) return;

            Vector3 colorSum = Vector3.Zero;

            foreach (TileEntity member in Members)
            {
                if (member is MushionSeedsGrowthBase mushion)
                {
                    mushion.colonyID = Id;
                    colorSum += mushion.glowColor.ToVector3();
                }
            }
            Vector3 normalizedColor = colorSum / Members.Count;
            GlowColor = new Color(normalizedColor);
        }
    }
    public class MushionColonySystem : ModSystem
    {
        Texture2D tileGlow;
        public override void Load()
        {
            tileGlow = ModContent.Request<Texture2D>("Mycology/Assets/Textures/glowTile").Value;
            On_Main.DrawTiles += GlowMud;
        }

        public static Dictionary<int, MushionColony> Colonies { get; private set; } = [];
        public static MushionColony ColonyFromID(int ID) => Colonies.TryGetValue(ID, out MushionColony colony) ? colony : null;
        public static MushionColony GetOrSetColony(int ID, int x, int y)
        {
            List<Point> chunk = ConnectedTiles(x, y + 2);
            List<TileEntity> entities = ColonyCount(chunk)
                .Where(e => e is MushionSeedsGrowthBase)
                .ToList();
            int? lowestColonyId = entities
                .Select(e => Colonies.TryGetValue(e.ID, out var c) ? c.Id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .DefaultIfEmpty(ID)
                .Min();
            int colonyId = lowestColonyId ?? ID;
            if (!Colonies.TryGetValue(colonyId, out MushionColony colony))
            {
                colony = new MushionColony(colonyId);
                Colonies.Add(colonyId, colony);
            }
            foreach (var entity in entities)
            {
                colony.AddMember(entity);
                if (entity is MushionSeedsGrowthBase mushion)
                {
                    mushion.colonyID = colonyId;
                }
            }

            return colony;
        }
        public override void PreUpdateWorld()
        {
            foreach (MushionColony colony in Colonies.Values)
            {
                    colony.Update();
            }
        }
        private static MushionColony GetColonyFromChunk(List<Point> chunk)
        {
            List<TileEntity> entities = ColonyCount(chunk);
            foreach (TileEntity entity in entities)
            {
                if (!Colonies.TryGetValue(entity.ID, out MushionColony value))
                {
                    value = GetOrSetColony(entity.ID, entity.Position.X, entity.Position.Y);
                }
                return value;
            }
            return null;
        }
        private void GlowMud(On_Main.orig_DrawTiles orig, Main self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
        {
            int minX = Main.screenPosition.ToPoint().X / 16 - 1;
            int maxX = minX + Main.screenWidth / 16 + 1;
            int minY = Main.screenPosition.ToPoint().Y / 16 - 1;
            int maxY = minY + Main.screenHeight / 16 + 1;
            int Type = ModContent.TileType<TilledMud>();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null);

            HashSet<Point> visited = [];

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    Point p = new(x, y);
                    if (visited.Contains(p)) continue;

                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.TileType != Type || !tile.HasTile) continue;

                    List<Point> chunk = ConnectedTiles(x, y);
                    foreach (var cTile in chunk) visited.Add(cTile);
                    if (!IsValidForGrowing(chunk[0].X, chunk[0].Y)) continue;

                    MushionColony colony = GetColonyFromChunk(chunk);

                    foreach (var cTile in chunk)
                    {
                        Tile above = Framing.GetTileSafely(cTile.X, cTile.Y - 1);
                        if (above.HasTile && Main.tileSolid[above.TileType]) continue;

                        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                        Vector2 drawPos = new Vector2(cTile.X * 16, cTile.Y * 16 - 12) - Main.screenPosition + zero;
                        Main.spriteBatch.Draw(tileGlow, drawPos, null, colony.GlowColor * colony.GlowModifier, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
                    }
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);

            orig(self, solidLayer, forRenderTargets, intoRenderTargets, waterStyleOverride);
        }

        private static readonly Point[] cardinals =
        [
            new(0, -1), 
            new(1, 0),
            new(0, 1),
            new(-1, 0)
        ];
        private static List<Point> ConnectedTiles(int i, int j)
        {
            Queue<Point> pointQueue = new();
            HashSet<Point> visited = [];
            List<Point> connected = [];

            int maxRange = 100;

            pointQueue.Enqueue(new Point(i, j));
            visited.Add(new Point(i, j));
            while (pointQueue.Count > 0 && connected.Count < maxRange)
            {
                Point p = pointQueue.Dequeue();
                connected.Add(p);
                foreach (var shift in cardinals)
                {
                    Point neighbor = p + shift;
                    if (!visited.Contains(neighbor))
                    {
                        Tile tile = Framing.GetTileSafely(neighbor.X, neighbor.Y);
                        if (tile.HasTile && tile.TileType == ModContent.TileType<TilledMud>())
                        {
                            visited.Add(neighbor);
                            pointQueue.Enqueue(neighbor);
                        }
                    }
                }
            }
            return connected;
        }
        private static List<TileEntity> ColonyCount(int i, int j)
        {
            List<Point> chunk = ConnectedTiles(i, j);
            List<TileEntity> tiles = [];
            foreach (var point in chunk)
            {
                if ((TileEntity.ByPosition.TryGetValue(new Point16(point.X, point.Y-2), out TileEntity entity)) && entity is MushionSeedsGrowthBase)
                {
                    tiles.Add(entity);
                }
            }
            if (chunk.Count < 20) tiles.Clear();
            return tiles;
        }
        private static List<TileEntity> ColonyCount(List<Point> chunk)
        {
            List<TileEntity> tiles = [];
            foreach (var point in chunk)
            {
                if ((TileEntity.ByPosition.TryGetValue(new Point16(point.X, point.Y - 2), out TileEntity entity)) && entity is MushionSeedsGrowthBase)
                {
                    tiles.Add(entity);
                }
            }
            if (chunk.Count < 20) tiles.Clear();
            return tiles;
        }
        public static bool IsValidForGrowing(int i, int j) =>
            (ColonyCount(i, j + 2).Count >= 3 && Lighting.Brightness(i, j) < 0.5f);
    }
}
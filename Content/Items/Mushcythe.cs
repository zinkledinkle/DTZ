using DTZ.Content.Tiles;
using DTZ.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DTZ.Content.Items
{
    public class Mushcythe : ModItem
    {
        public override void Load()
        {
            On_SmartCursorHelper.Step_StaffOfRegrowth += SmartCursorSupport;
        }
        public override void Unload()
        {
            On_SmartCursorHelper.Step_StaffOfRegrowth -= SmartCursorSupport;
        }
        private class SmartCursorUsageInfo
        {
            public Player player;
            public Item item;
            public Vector2 mouse;
            public Vector2 position;
            public Vector2 Center;
            public int screenTargetX;
            public int screenTargetY;
            public int reachableStartX;
            public int reachableEndX;
            public int reachableStartY;
            public int reachableEndY;
            public int paintLookup;
            public int paintCoatingLookup;
        }
        private void SmartCursorSupport(On_SmartCursorHelper.orig_Step_StaffOfRegrowth orig, object providedInfo, ref int focusedX, ref int focusedY)
        {
            SmartCursorUsageInfo info = Unsafe.As<SmartCursorUsageInfo>(providedInfo);
            orig(providedInfo, ref focusedX, ref focusedY);
            if (info.item.type != Type || focusedX != -1 || focusedY != -1) return;
            var targets = typeof(SmartCursorHelper).GetField("_targets", BindingFlags.NonPublic | BindingFlags.Static);
            var targetsList = targets?.GetValue(null) as List<Tuple<int, int>>; //convert back to initial list format
            targetsList?.Clear();

            for (int x = info.reachableStartX; x < info.reachableEndX; x++)
            {
                for (int y = info.reachableStartY; y < info.reachableEndY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    bool flag = !Main.tile[x - 1, y].HasTile || !Main.tile[x, y + 1].HasTile || !Main.tile[x + 1, y].HasTile || !Main.tile[x, y - 1].HasTile;
                    //bool flag2 = !Main.tile[x - 1, y - 1].HasTile || !Main.tile[x - 1, y + 1].HasTile || !Main.tile[x + 1, y + 1].HasTile || !Main.tile[x + 1, y - 1].HasTile;
                    //these flags are completely ripped from terraria source to make it consistent, genuinely no idea what they do. best guess is checking for air adjacent blocks rather than ones naturally out of reach
                    //yeah it seems that at the very least the second one is checking topmost/air adjacent tiles, which makes sense for the staff of regrowth but not here
                    bool isMud = tile.TileType is TileID.Mud or TileID.MushroomGrass;
                    if (isMud && (flag || (isMud)))
                    {
                        targetsList.Add(new Tuple<int, int>(x,y)); //add to candidates
                    }
                }
            }
            if (targetsList.Count > 0)
            {
                float num = -1f;
                Tuple<int, int> tuple = targetsList[0];
                foreach (Tuple<int, int> target in targetsList)
                {
                    float distance = Vector2.Distance(new Vector2(target.Item1, target.Item2) * 16 + Vector2.One * 8, info.mouse);
                    if (num == -1 || distance < num)
                    {
                        num = distance;
                        tuple = target;
                    }
                }
                if (Collision.InTileBounds(tuple.Item1, tuple.Item2, info.reachableStartX, info.reachableStartY, info.reachableEndX, info.reachableEndY))
                {
                    focusedX = tuple.Item1;
                    focusedY = tuple.Item2;
                }
            }

            /*                                                                
 : =: =: =: =: =: =: =.       :  :  .     .. =: =: =: =: =: =: =: 
 :.::.::.::.::.::.:     @@@@@@@@@@@@@@@@@@     .::.::.::.::.::.:: 
 :+ :+ :+ :+ :+ ..  #@@@#=:.          .-=#@@@+   .+ :+ :+ :+ :+ : 
 : +: +: +: +: .  @@%+:                   .:+%@@:  -: +: +: +: +: 
 :-.:-.:-.:-.   @@*-    ..::::::::::::::.     -*@@:  -.:-.:-.:-.: 
 ::.::.::.::  @@*-.  .::::::::::::::::::::::.   -+@@   ::.::.::.: 
 : =: =: =.  @%+:  .::::::::::::::::::::::::::.  .=%@  . =: =: =: 
 :+ :+ :=  -@*-. .::::.      .::::::.      .::::. .-*@. = :+ :+ : 
 : =: =: . @*=.         @@@@   ::::   @@@@         :=*@  =: =: =: 
 :::::::  @#=:   @@@@ @@=  +@@  ::  @@+  =@@ @@@@   .=#@  ::::::: 
 :+ := :  @--  @@= .#@*:===:.@= :: .@.:===:*@%: =@@  --@- := := : 
 : +: +: @%=- @@:  .-***+++-#@  ::  @%=+++***-.  .@@ -=#@ : +: +:  I LOVE REFLECTION
 :- :- : @+=-  @@@@@@@#**+=@@= .::: .@@=++*#@@@@@@@: :==@ :- :- : 
 ::::::: @==-:  @@@@%%@@%@@@   :--:   @@@%@@%%@@@@. .-=-@ ::::::: 
 : -: -: @+=--:   @@@@@@@@@   :----:.  @@@@@@@@@   .--=+@ : -: -: 
 :+ :+ : @@====-:     +@@   .:-------.   @@*     .:====%@ :+ :+ : 
 : +: +: .@-=====-:.       .::::---::::        .-=====-@  : +: +: 
 ::.::.:  @@=======---:..:--------------:..::--=-=====@@  ::.::.: 
 :+ :+ :=  @#======:       .:::-::::::.       :=====-#@   :+ :+ : 
 : +: +: =  @%=====:@@@@@@.            .%@@@@@:=====#@=  +: +: +: 
 := := :=    @@+===- @@#%@@@@@@@@@@@@@@@@%#@@ -===+@@   = := := : 
 :.-:.-:.-:   @@%==+: *@@*++*********+++*%@# :++=%@@  .:.-:.-:.-: 
 :.-:.-:.-:.-   @@#==-  *@@@%*++++++*%@@@#  -==#@@    -:.-:.-:.-: 
 :+ :+ :+ :+ :-   @@@*=-   :#@@@@@@@@#:   -=*@@@    :+ :+ :+ :+ : 
 : +: +: +: +: +.   *@@@@+-:          :-+@@@@@     +: +: +: +: +: 
 :-.:-.:-.:-.:-.:-.     @@@@@@@@@@@@@@@@@@     :.:-.:-.:-.:-.:-.: 
 :- :- :- :- :- :- :-                       - :- :- :- :- :- :- : 
                                                                  */
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 38;
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.knockBack = 0.1f;
            Item.crit = 0;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(silver: 10);
            Item.useTurn = true;
        }
        public override bool? UseItem(Player player)
        {
            Point16 tileCoords = Main.SmartCursorIsUsed ? new Point16(Main.SmartCursorX, Main.SmartCursorY) : Main.MouseWorld.ToTileCoordinates16();
            Tile tile = Framing.GetTileSafely(tileCoords);
            int type = tile.TileType;
            if (type is TileID.Mud or TileID.MushroomGrass)
            {
                tile.TileType = (ushort)ModContent.TileType<TilledMud>();
                SoundEngine.PlaySound(SoundID.Grass, Main.MouseWorld);
                for (int i = 0; i < Main.rand.Next(1, 4); i++) {
                    Vector2 vel = -Main.rand.NextVector2Unit(MathHelper.PiOver4, MathHelper.PiOver2) * 2;
                    int dustType = type == TileID.MushroomGrass ? DustID.MushroomSpray : DustID.Mud;
                    Dust.NewDust(tileCoords.ToWorldCoordinates(), 16, 1, dustType, vel.X, vel.Y);
                }
                NetMessage.SendTileSquare(-1, tileCoords.X, tileCoords.Y);

                for(int h = -1; h <= 1; h++)
                {
                    for(int j = -1; j <= 1; j++)
                    {
                        var neighborCoords = tileCoords + new Point16(h, j);
                        WorldGen.Reframe(neighborCoords.X, neighborCoords.Y);
                    }
                } 
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 15)
                .AddIngredient(ItemID.Sickle)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}

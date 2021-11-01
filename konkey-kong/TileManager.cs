using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace pakeman
{
    class TileManager
    {
        Tile[,] currentMap = new Tile[36, 27];
        Tile gate1;
        Tile gate2;

        public void Initialize(Player player, PickupManager pickup, EnemyManager enemy, Texture2D ghostTex, Texture2D pointTex, Texture2D powerup1Tex, Texture2D powerup2Tex)
        {
            int ghostscreated = 0;
            int gatesCreated = 0;
            int powerupAlternate = 1;
            foreach (Tile t in currentMap)
            {
                if (t.type == TileType.GhostSpawn)
                {
                    if (ghostscreated < 3)
                    {
                        Enemy enemyCreator = new Enemy(t.pos, t.size, ghostTex, t.posX, t.posY, ghostscreated);
                        enemy.enemies.Add(enemyCreator);
                        ghostscreated++;
                        t.type = TileType.Standard;
                    }
                }
                if (t.type == TileType.PacmanSpawn)
                {
                    player.pos = t.pos;
                    player.size = t.size;
                    player.tilePosX = t.posX;
                    player.tilePosY = t.posY;
                    player.respawnTile = t;
                    t.type = TileType.Standard;
                }
                if (t.type == TileType.Standard)
                {
                    Pickup pickupCreator = new Pickup(new Vector2(t.pos.X + 12, t.pos.Y + 12), pointTex, new Rectangle((int)(t.pos.X + 12), (int)(t.pos.Y + 12), 8, 8));
                    pickup.list.Add(pickupCreator);
                }
                if (t.type == TileType.PowerUpSpawn)
                {
                    if (powerupAlternate == 1)
                    {
                        Powerup pickupCreator = new Powerup(new Vector2(t.pos.X, t.pos.Y), powerup1Tex, new Rectangle((int)(t.pos.X), (int)(t.pos.Y), 32, 32), PickupType.PowerupEatWall);
                        pickupCreator.value = 100;
                        pickup.list.Add(pickupCreator);
                    }
                    else
                    {
                        Powerup pickupCreator = new Powerup(new Vector2(t.pos.X, t.pos.Y), powerup2Tex, new Rectangle((int)(t.pos.X), (int)(t.pos.Y), 32, 32), PickupType.PowerupEatGhost);
                        pickupCreator.value = 100;
                        pickup.list.Add(pickupCreator);
                    }
                    t.type = TileType.Standard;
                    powerupAlternate *= -1;
                }
                if (t.type == TileType.Gate)
                {
                    if (gatesCreated == 0) { gate1 = t; }
                    if (gatesCreated == 1) { gate2 = t; }
                    gatesCreated++;
                    t.type = TileType.Standard;
                }
            }



        }

        public Tile[] NeighborTiles(int posX, int posY)
        {
            Tile[] tiles = new Tile[11];
            tiles[0] = currentMap[posX, posY];
            tiles[1] = currentMap[posX, posY];
            tiles[2] = currentMap[posX, posY];
            tiles[3] = currentMap[posX, posY];
            tiles[4] = currentMap[posX, posY];
            tiles[5] = currentMap[posX, posY];
            tiles[6] = currentMap[posX, posY];
            tiles[7] = currentMap[posX, posY];
            tiles[8] = currentMap[posX, posY];
            tiles[9] = currentMap[posX, posY];
            tiles[10] = currentMap[posX, posY];
            tiles[11] = currentMap[posX, posY];

            return tiles;
        }


    }
}

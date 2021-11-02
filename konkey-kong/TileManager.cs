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
using static pakeman.Game1;

namespace pakeman
{
    public class TileManager
    {
        TextureManager textures;
        Player player;
        public Tile[,] currentMap = new Tile[36, 27];
        double gateTimer = 0;
        const double GATETIMER = 800;

        public TileManager(TextureManager textures, Player player)
        {
            this.textures = textures;
            this.player = player;
        }

        public void Initialize(PickupManager pickup, EnemyManager enemy)
        {
            int ghostscreated = 0;
            int powerupAlternate = 1;
            foreach (Tile t in currentMap)
            {
                if (t.type == TileType.GhostSpawn)
                {
                    if (ghostscreated < 3)
                    {
                        Enemy enemyCreator = new Enemy(t.pos, t.size, textures.ghost, t.posX, t.posY, ghostscreated);
                        enemyCreator.respawnTile = t;
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
                    Pickup pickupCreator = new Pickup(new Vector2(t.pos.X + 12, t.pos.Y + 12), textures.point, new Rectangle((int)(t.pos.X + 12), (int)(t.pos.Y + 12), 8, 8));
                    pickup.list.Add(pickupCreator);
                }
                if (t.type == TileType.PowerUpSpawn)
                {
                    if (powerupAlternate == 1)
                    {
                        Powerup pickupCreator = new Powerup(new Vector2(t.pos.X, t.pos.Y), textures.powerupWall, new Rectangle((int)(t.pos.X), (int)(t.pos.Y), 32, 32), PickupType.PowerupEatWall);
                        pickupCreator.value = 100;
                        pickup.list.Add(pickupCreator);
                    }
                    else
                    {
                        Powerup pickupCreator = new Powerup(new Vector2(t.pos.X, t.pos.Y), textures.powerupGhost, new Rectangle((int)(t.pos.X), (int)(t.pos.Y), 32, 32), PickupType.PowerupEatGhost);
                        pickupCreator.value = 100;
                        pickup.list.Add(pickupCreator);
                    }
                    t.type = TileType.Standard;
                    powerupAlternate *= -1;
                }
                if (t.type == TileType.Gate)
                {
                    t.type = TileType.Standard;
                }
            }

        }

        public Tile[] NeighborTiles(int posX, int posY)
        {
            Tile[] tiles = new Tile[12];
            tiles[0] = currentMap[posX-1, posY];
            tiles[1] = currentMap[posX+1, posY];
            tiles[2] = currentMap[posX, posY-1];
            tiles[3] = currentMap[posX, posY+1];
            tiles[4] = currentMap[posX-2, posY];
            tiles[5] = currentMap[posX+2, posY];
            tiles[6] = currentMap[posX, posY-2];
            tiles[7] = currentMap[posX, posY+2];
            tiles[8] = currentMap[posX-1, posY-1];
            tiles[9] = currentMap[posX+1, posY-1];
            tiles[10] = currentMap[posX-1, posY+1];
            tiles[11] = currentMap[posX+1, posY+1];

            return tiles;
        }

        public void Update(double time, GameState gameState)
        {
            
            foreach (Tile t in currentMap)
            {
                if (t.posX > 0 && t.posX < currentMap.GetLength(0) - 1 && t.posY > 0 && t.posY < currentMap.GetLength(1) - 1)
                {
                    t.DetermineWallTex(currentMap[t.posX - 1, t.posY - 1], currentMap[t.posX, t.posY - 1], currentMap[t.posX + 1, t.posY - 1], currentMap[t.posX - 1, t.posY], currentMap[t.posX + 1, t.posY], currentMap[t.posX - 1, t.posY + 1], currentMap[t.posX, t.posY + 1], currentMap[t.posX + 1, t.posY + 1]);
                }
            }
            if (gameState == GameState.InGame)
            {
                gateTimer -= time;
                CheckGate();
                if (player.state == EntityState.PowerupWall)
                {
                    if (currentMap[player.tilePosX, player.tilePosY].type == TileType.Wall && player.tilePosX > 2 & player.tilePosX < currentMap.GetLength(0) - 2)
                    {
                        currentMap[player.tilePosX, player.tilePosY].type = TileType.Standard;
                        currentMap[player.tilePosX, player.tilePosY].tex = textures.blank;
                        foreach (Tile t in currentMap)
                        {
                            if (t.posX > 0 && t.posX < currentMap.GetLength(0) - 1 && t.posY > 0 && t.posY < currentMap.GetLength(1) - 1)
                            {
                                t.DetermineWallTex(currentMap[t.posX - 1, t.posY - 1], currentMap[t.posX, t.posY - 1], currentMap[t.posX + 1, t.posY - 1], currentMap[t.posX - 1, t.posY], currentMap[t.posX + 1, t.posY], currentMap[t.posX - 1, t.posY + 1], currentMap[t.posX, t.posY + 1], currentMap[t.posX + 1, t.posY + 1]);
                            }
                        }
                    }
                }
            }
        }
        public void CheckGate()
        {
            if (player.tilePosX <= 2 && !player.isMoving && gateTimer < 0)
            {
                player.GateMove(currentMap[currentMap.GetLength(0) - 3, player.tilePosY], Direction.Left);
                gateTimer = GATETIMER;
            }
            if (player.tilePosX >= currentMap.GetLength(0) - 3 && !player.isMoving && gateTimer < 0)
            {
                player.GateMove(currentMap[2, player.tilePosY], Direction.Right);
                gateTimer = GATETIMER;
            }
            if (player.tilePosY <= 2 && !player.isMoving && gateTimer < 0)
            {
                player.GateMove(currentMap[player.tilePosX, currentMap.GetLength(1) - 3], Direction.Up);
                gateTimer = GATETIMER;
            }
            if (player.tilePosY >= currentMap.GetLength(1) - 3 && !player.isMoving && gateTimer < 0)
            {
                player.GateMove(currentMap[player.tilePosX, 2], Direction.Down);
                gateTimer = GATETIMER;
            }
        }
        public void LevelEdit(char button)
        {
            var mouse = Mouse.GetState();

            foreach (Tile t in currentMap)
            {
                if(button == 'l' && t.size.Contains(mouse.Position))
                {
                    switch (t.type)
                    {
                        case TileType.Wall:
                            t.type = TileType.Standard;
                            t.tex = textures.blank;
                            break;
                        case TileType.Standard:
                            t.type = TileType.Wall;
                            t.tex = textures.wallSheet;
                            break;
                    }
                }
                if (button == 'r' && t.size.Contains(mouse.Position))
                {
                    switch (t.type)
                    {
                        case TileType.Standard:
                            t.type = TileType.PowerUpSpawn;
                            t.tex = textures.blank;
                            break;

                        case TileType.PowerUpSpawn:
                            t.type = TileType.PacmanSpawn;
                            t.tex = textures.blank;
                        break;

                        case TileType.PacmanSpawn:
                            t.type = TileType.GhostSpawn;
                            t.tex = textures.blank;
                        break;

                        case TileType.GhostSpawn:
                            t.type = TileType.Standard;
                            t.tex = textures.blank;
                        break;
                    }

                }

            }



        }
        public void UpdateDraw(SpriteBatch spriteBatch, GameState gameState)
        {
            foreach (Tile t in currentMap)
            {
                t.Draw(spriteBatch);
            }
            if(gameState == GameState.LevelEditor)
            {
                foreach (Tile t in currentMap)
                {
                    if(t.type == (TileType.PowerUpSpawn))
                    {
                        spriteBatch.Draw(textures.powerupGhost, t.pos, new Rectangle(0,0,32,32), Color.White);
                    }
                    if (t.type == (TileType.GhostSpawn))
                    {
                        spriteBatch.Draw(textures.ghost, t.pos, new Rectangle(0, 0, 32, 32), Color.White);
                    }
                    if (t.type == (TileType.PacmanSpawn))
                    {
                        spriteBatch.Draw(textures.pakeman, t.pos, new Rectangle(0, 0, 32, 32), Color.White);
                    }
                }
            }
        }
    }
}

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
    class EnemyManager
    {
        public List<Enemy> enemies = new List<Enemy>();

        public void Update(double time, Player player, ScoreManager score)
        {
            foreach (Enemy e in enemies)
            {
                e.EntityMove();
                e.WhenScared(player.tilePosX, player.tilePosY);
                e.ArtificialIntelligence();
                e.Anim(time);

                Collision(e, player, score);
            }
        }

        public void UpdateDraw(SpriteBatch spriteBatch)
        {
            foreach (Enemy e in enemies)
            {
                e.Draw(spriteBatch);
            }
        }
        private void Collision(Enemy e, Player p, ScoreManager score)
        {
            if(e.size.Intersects(p.size) && e.state == EntityState.Default && p.state != EntityState.PowerupGhost)
            {
                p.Death();
                p.health--;

            }
            else if (e.size.Intersects(p.size) && e.state != EntityState.Death && p.state == EntityState.PowerupGhost)
            {
                e.state = EntityState.Death;
                score.Increment(200, e.pos);
            }
        }

    }
}

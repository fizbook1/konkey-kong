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
    class ScoreManager
    {
        public int score = 0;
        public List<PointString> list = new List<PointString>();
        public void Increment(int value, Vector2 pos)
        {
            score += value;
            PointString pointStringCreate = new PointString(value, pos);
            list.Add(pointStringCreate);

        }
        public void Update(double time)
        {
            int? killObject = null;
            foreach (PointString ps in list)
            {
                ps.PointStringAnim(time);
                if (ps.duration < 0)
                {
                    killObject = list.IndexOf(ps);
                }
            }
            if (killObject != null)
            {
                list.RemoveAt((int)killObject);
            }
        }

        public void UpdateDraw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            foreach (PointString ps in list)
            {
                ps.Draw(spriteBatch, spriteFont);
            }
        }
        
    }
}

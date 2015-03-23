using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Savant
{
    class Obstacle
    {
        public Vector2 position;

        public Obstacle(Rectangle Bounds)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            position = new Vector2(random.Next(Bounds.Left, Bounds.Right), random.Next(Bounds.Top, Bounds.Bottom));
        }

        public void Draw(SpriteBatch spritebatch, Texture2D texture)
        {
            Vector2 origin = new Vector2(25, 25);
            spritebatch.Draw(texture, position - origin, Color.Red);
        }
    }
}

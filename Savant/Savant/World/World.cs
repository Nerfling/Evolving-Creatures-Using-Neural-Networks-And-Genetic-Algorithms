using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Savant
{
    class World
    {
        public Rectangle Bounds;

        public Camera camera;

        public Texture2D TextureCreature;
        public Texture2D TexturePoint;
        public Texture2D TextureFood;
        public SpriteFont MyFont;

        public int NrOfCreatures = 100;
        public List<Creature> creature;

        public int NrOfFood = 200;
        public List<Food> food;

        public int NrOfObstacles = 0;
        public List<Obstacle> obstacle;

        public BasicShapes basicshapes;

        public int Ticks;

        public GeneticAlgorithm GA;

        public int NrOfDeaths;

        private double[] GraphValue;

        public bool DoDraw;

        public World(ContentManager content, Rectangle Bounds)
        {
            this.Bounds = Bounds;
            this.TextureCreature = content.Load<Texture2D>("Bug");
            this.TexturePoint = content.Load<Texture2D>("Point");
            this.TextureFood = content.Load<Texture2D>("Circle");
            this.MyFont = content.Load<SpriteFont>("MyFont");
            this.camera = new Camera(new Viewport(this.Bounds));
            this.basicshapes = new BasicShapes();
            creature = new List<Creature>();
            for (int i = 0; i < NrOfCreatures; i++)
            {
                Creature c = new Creature(Bounds);
                creature.Add(c);
            }
            food = new List<Food>();
            for (int i = 0; i < NrOfFood; i++)
            {
                Food f = new Food(Bounds);
                food.Add(f);
            }
            obstacle = new List<Obstacle>();
            for (int i = 0; i < NrOfObstacles; i++)
            {
                Obstacle o = new Obstacle(Bounds);
                obstacle.Add(o);
            }
            camera = new Camera(new Viewport(0, 0, 1600, 900));
            GA = new GeneticAlgorithm(60, 1);
            NrOfDeaths = 0;
            GraphValue = new double[32000];
            DoDraw = true;
        }

        public void Update()
        {
            camera.Update();

            NrOfDeaths = 0;
            foreach (Creature c in creature)
            {
                c.Update(food, obstacle);
                if (c.Life <= 0)
                    NrOfDeaths++;
            }

            Ticks++;
            if (Ticks == 10000 || NrOfDeaths == creature.Count())
            {
                Ticks = 0;
                GraphValue[GA.Generation] = GA.Evolve(creature, Bounds);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (Ticks % 10 == 0)
                {
                    Obstacle NewObstacle = new Obstacle(Bounds);
                    NewObstacle.position = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), camera.InverseTransform);
                    obstacle.Add(NewObstacle);
                }
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                obstacle.Clear();
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (DoDraw)
            {
                spritebatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
                foreach (Obstacle o in obstacle)
                    o.Draw(spritebatch, TextureFood);
                foreach (Food f in food)
                    f.Draw(spritebatch, TextureFood);
                foreach (Creature c in creature)
                    c.Draw(spritebatch, TextureCreature);

                basicshapes.DrawLine(spritebatch, new Vector2(Bounds.Left - 100, Bounds.Top - 100), new Vector2(Bounds.Right + 100, Bounds.Top - 100), Color.Black, TexturePoint, 4);
                basicshapes.DrawLine(spritebatch, new Vector2(Bounds.Right + 100, Bounds.Top - 100), new Vector2(Bounds.Right + 100, Bounds.Bottom + 100), Color.Black, TexturePoint, 4);
                basicshapes.DrawLine(spritebatch, new Vector2(Bounds.Right + 100, Bounds.Bottom + 100), new Vector2(Bounds.Left - 100, Bounds.Bottom + 100), Color.Black, TexturePoint, 4);
                basicshapes.DrawLine(spritebatch, new Vector2(Bounds.Left - 100, Bounds.Bottom + 100), new Vector2(Bounds.Left - 100, Bounds.Top - 100), Color.Black, TexturePoint, 4);

                spritebatch.End();
            }

            spritebatch.Begin();
            spritebatch.DrawString(MyFont, "Generation: " + GA.Generation, new Vector2(10, 10), Color.Black);
            spritebatch.DrawString(MyFont, "Tick: " + Ticks, new Vector2(10, 30), Color.Black);
            spritebatch.DrawString(MyFont, "Deaths: " + NrOfDeaths, new Vector2(10, 50), Color.Black);
            spritebatch.DrawString(MyFont, "Elitism: " + GA.ElitismChance + "%", new Vector2(10, 70), Color.Black);
            spritebatch.DrawString(MyFont, "Crossover: " + GA.CrossOverChance + "%", new Vector2(10, 90), Color.Black);
            spritebatch.DrawString(MyFont, "Mutation: " + GA.MutationChance + "%", new Vector2(10, 110), Color.Black);
            DrawGraph(spritebatch);
            spritebatch.End();
        }

        void DrawGraph(SpriteBatch spritebatch)
        {
            double GraphScale = 1.0;
            double MaxGraph = 0;
            double TotalGraph = 0;
            for (int i = 0; i < GA.Generation; i++)
            {
                TotalGraph += GraphValue[i];
                if (GraphValue[i] > MaxGraph)
                    MaxGraph = GraphValue[i];
            }

            double AverageGraph = TotalGraph / GA.Generation;

            if (MaxGraph > 100)
            {
                GraphScale = 100 / MaxGraph;
            }

            for (int i = 0; i < GA.Generation; i++)
            {
                if (i == 0)
                {
                    DrawLine(spritebatch, new Vector2((i * 2), 900), new Vector2((float)((i * 2) + 2), (float)(900 - (GraphValue[i] * GraphScale))), Color.Black, TexturePoint, 1);
                }
                else
                {
                    DrawLine(spritebatch, new Vector2((float)(i * 2), (float)(900 - (GraphValue[i - 1] * GraphScale))), new Vector2((float)((i * 2) + 2), (float)(900 - (GraphValue[i]) * GraphScale)), Color.Black, TexturePoint, 1);
                }
            }

            DrawLine(spritebatch, new Vector2(0, (float)(900 - (AverageGraph * GraphScale))), new Vector2(1600, (float)(900 - (AverageGraph * GraphScale))), Color.Blue, TexturePoint, 1);
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color, Texture2D texture, int size)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(texture,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    size), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
    }
}

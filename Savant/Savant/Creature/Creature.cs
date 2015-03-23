using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Savant
{
    class Creature
    {
        public Vector2 Position;
        public double Angle;
        public NeuralNetwork Brain;
        public int Frame;
        public Rectangle Bounds;
        public double Life;
        public double Fitness;
        public double ParentChance;

        public Creature(Rectangle Bounds)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            this.Bounds = Bounds;
            this.Position = new Vector2(random.Next(Bounds.Left, Bounds.Right), random.Next(Bounds.Top, Bounds.Bottom));
            this.Angle = random.Next(0, 360);
            this.Brain = new NeuralNetwork(0.0, 4, 250, 3);
            this.Frame = random.Next(0, 8);
            this.Life = 100;
        }

        public void Reset()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            //this.Position = new Vector2(random.Next(Bounds.Left, Bounds.Right), random.Next(Bounds.Top, Bounds.Bottom));
            this.Angle = random.Next(0, 360);
            this.Frame = random.Next(0, 8);
            this.Fitness = 0;
            this.Life = 100;
        }

        public void Update(List<Food> food, List<Obstacle> obstacle)
        {
            if (Life <= 0)
            {
                Fitness = 0;
                return;
            }

            Frame++;
            if (Frame > 7)
                Frame = 0;

            double[] input = null;
            double[] output = null;

            input = new double[4];

            Vector2 Origin = new Vector2(32, 32);
            Vector2 LeftSensor = ExtendedPoint(Position, (float)(Angle - 45 - 90), 100);
            Vector2 RightSensor = ExtendedPoint(Position, (float)(Angle + 45 - 90), 100);
            double ClosestFoodLeft = GetDistance(GetClosestFood(food, Position).position, LeftSensor);
            double ClosestFoodRight = GetDistance(GetClosestFood(food, Position).position, RightSensor);
            double ClosestObstacleLeft = GetDistance(GetClosestObstacle(obstacle, Position).position, LeftSensor);
            double ClosestObstacleRight = GetDistance(GetClosestObstacle(obstacle, Position).position , RightSensor);
            double CenterDistance = GetDistance(GetClosestFood(food, Position).position, Position);
            double CenterDistanceObstacle = GetDistance(GetClosestObstacle(obstacle, Position).position, Position);
            if (CenterDistance < 50)
            {
                Life += 30;
                Fitness += 10;
                Random random = new Random(Guid.NewGuid().GetHashCode());
                GetClosestFood(food, Position).position = new Vector2(random.Next(Bounds.Left, Bounds.Right), random.Next(Bounds.Top, Bounds.Bottom));
            }

            Life -= 0.025;

            if (CenterDistance < CenterDistanceObstacle)
            {
                if (ClosestFoodLeft > ClosestFoodRight)
                {
                    input[0] = 1;
                    input[1] = -1;
                }
                else
                {
                    input[0] = -1;
                    input[1] = 1;
                }

                input[2] = 0;
                input[3] = 0;
            }
            else
            {
                input[0] = 0;
                input[1] = 0;

                if (ClosestObstacleLeft > ClosestObstacleRight)
                {
                    input[2] = 1;
                    input[3] = -1;
                }
                else
                {
                    input[2] = -1;
                    input[3] = 1;
                }

            }

            /*if (CenterDistance > CenterDistanceObstacle)
            {
                input[4] = 1;
            }
            else
            {
                input[4] = -1;
            }*/

            /*input[0] = ClosestFoodLeft;
            input[1] = ClosestFoodRight;
            input[2] = ClosestObstacleLeft;
            input[3] = ClosestObstacleRight;*/

            output = Brain.Run(input);

            if (output[0] > output[1])
            {
                Angle += output[0] * 4;
            }
            else
            {
                Angle -= output[1] * 4;
            }
            double Speed = output[2] * 2;
            float Radians = (float)((Angle - 90) * Math.PI / 180);
            Vector2 OldPos = Position;
            Position.X += (float)(Math.Cos(Radians) * Speed);
            Position.Y += (float)(Math.Sin(Radians) * Speed);
            double ClosestObstacle = 32000000;
            foreach (Obstacle o in obstacle)
            {
                if (GetDistance(o.position - Origin, Position - Origin) < ClosestObstacle)
                    ClosestObstacle = GetDistance(o.position - Origin, Position - Origin);
            }
            if (ClosestObstacle < 50)
            {
                Life -= 1;
                Fitness -= 10;
                if (Fitness < 0)
                    Fitness = 0;
                Position = OldPos;
            }
            if (Position.X < Bounds.Left)
                Position.X = Bounds.Left;
            if (Position.X > Bounds.Right)
                Position.X = Bounds.Right;
            if (Position.Y < Bounds.Top)
                Position.Y = Bounds.Top;
            if (Position.Y > Bounds.Bottom)
                Position.Y = Bounds.Bottom;
        }

        public void Draw(SpriteBatch spritebatch, Texture2D texture)
        {
            if (Life <= 0)
                return;

            Vector2 Origin = new Vector2(16, 16);
            Rectangle SourceRectangle = new Rectangle(Frame * 32, 0, 32, 32);
            Rectangle DestinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, 64, 64);
            spritebatch.Draw(texture, DestinationRectangle, SourceRectangle, Color.White, (float)(Angle * Math.PI / 180), Origin, SpriteEffects.None, 0.0f);
        }

        public Vector2 ExtendedPoint(Vector2 center, float directionangle, int length)
        {
            float Radians = (float)(directionangle * Math.PI / 180);
            Vector2 resultposition;
            resultposition.X = (float)(center.X + (Math.Cos(Radians) * length));
            resultposition.Y = (float)(center.Y + (Math.Sin(Radians) * length));
            return resultposition;
        }

        public Food GetClosestFood(List<Food> food, Vector2 Start)
        {
            Food ClosestFood = new Food(Bounds);
            double Closest = 320000;
            foreach (Food f in food)
            {
                if (GetDistance(Start, f.position) < Closest)
                {
                    Closest = GetDistance(Start, f.position);
                    ClosestFood = f;
                }
            }
            return ClosestFood;
        }

        public Obstacle GetClosestObstacle(List<Obstacle> obstacle, Vector2 Start)
        {
            Obstacle ClosestObstacle = new Obstacle(Bounds);
            double Closest = 32000;
            foreach (Obstacle o in obstacle)
            {
                if (GetDistance(Start, o.position) < Closest)
                {
                    Closest = GetDistance(Start, o.position);
                    ClosestObstacle = o;
                }
            }
            return ClosestObstacle;
        }

        public double GetDistance(Vector2 Start, Vector2 End)
        {
            Vector2 Diff = Start - End;
            return Math.Sqrt(Diff.X * Diff.X + Diff.Y * Diff.Y);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Battlezeppelins.Models
{
    public class Zeppelin
    {
        [JsonProperty]
        public ZeppelinType type { get; private set; }
        [JsonProperty]
        public Point location { get; private set; }
        [JsonProperty]
        public bool rotDown { get; private set; }

        private Zeppelin() { }

        public Zeppelin(ZeppelinType type, Point location, bool rotDown) {
            this.type = type;
            this.location = location;
            this.rotDown = rotDown;
        }

        private List<Point> getPoints()
        {
            List<Point> points = new List<Point>();
            int x = this.location.x;
            int y = this.location.y;

            for (int i = 0; i < this.type.length; i++)
            {
                Point point = new Point(x, y);
                points.Add(point);
                if (this.rotDown) y++;
                else x++;
            }

            return points;
        }

        /// <summary>
        /// Is every point of Zeppelin filled with collisionPoints
        /// </summary>
        /// <param name="openPoints"></param>
        /// <returns></returns>
        public bool fullyCollides(List<Point> collisionPoints)
        {
            foreach (Point zeppelinPoint in this.getPoints())
            {
                bool pointCollides = false;
                foreach (OpenPoint collisionPoint in collisionPoints)
                {
                    if (zeppelinPoint.x == collisionPoint.x &&
                        zeppelinPoint.y == collisionPoint.y)
                    {
                        pointCollides = true;
                        break;
                    }
                }

                if (!pointCollides) return false;
            }

            return true;
        }

        public bool collides(Zeppelin zeppelin)
        {
            List<Point> points = zeppelin.getPoints();
            foreach (Point point in points)
            {
                if (collides(point)) return true;
            }

            return false;
        }

        public bool collides(Point point)
        {
            List<Point> points = this.getPoints();
            foreach (Point thisPoint in points)
            {
                if (thisPoint.Equals(point)) return true;
            }
            return false;
        }

        public int getWidth()
        {
            if (this.rotDown) return 1;
            else return this.type.length;
        }

        public int getHeight()
        {
            if (!this.rotDown) return 1;
            else return this.type.length;
        }
    }
}

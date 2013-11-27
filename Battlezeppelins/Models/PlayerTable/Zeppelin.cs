using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Battlezeppelins.Models
{
    public class Zeppelin
    {
        public ZeppelinType type { get; private set; }
        public int x { get; private set; }
        public int y { get; private set; }
        public bool rotDown { get; private set; }

        public Zeppelin(ZeppelinType type, int x, int y, bool rotDown) {
            this.type = type;
            this.x = x;
            this.y = y;
            this.rotDown = rotDown;
        }

        private List<Point> getPoints()
        {
            List<Point> points = new List<Point>();
            int x = this.x;
            int y = this.y;

            for (int i = 0; i < this.type.Length; i++)
            {
                if (this.rotDown) y--;
                else x++;
                Point point = new Point(x, y);
                points.Add(point);
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
            foreach (Point point in zeppelin.getPoints())
            {
                if (collides(point)) return true;
            }

            return false;
        }

        public bool collides(Point point)
        {
            foreach (Point thisPoint in this.getPoints())
            {
                if (thisPoint.x == point.x && thisPoint.y == point.y)
                {
                    return true;
                }
            }
            return false;
        }

        public int getWidth()
        {
            if (this.rotDown) return 1;
            else return this.type.Length;
        }

        public int getHeight()
        {
            if (!this.rotDown) return 1;
            else return this.type.Length;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Battlezeppelins.Models
{

    public class GameTable
    {
        public const int TABLE_ROWS = 10;
        public const int TABLE_COLS = 10;

        [JsonProperty]
        public Game.Role role { get; private set; }
        [JsonProperty]
        public List<OpenPoint> openPoints { get; private set; }
        [JsonProperty]
        public List<Zeppelin> zeppelins { get; private set; }

        private GameTable() { }

        public GameTable(Game.Role role)
        {
            this.role = role;

            this.openPoints = new List<OpenPoint>();
            this.zeppelins = new List<Zeppelin>();
        }

        public static GameTable deserialize(string serialized)
        {
            var settings = new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };
            GameTable deserialized = JsonConvert.DeserializeObject<GameTable>(serialized, settings);

            return deserialized;
        }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public bool AddZeppelin(Zeppelin newZeppelin)
        {
            foreach (Zeppelin zeppelin in this.zeppelins)
            {
                // No multiple zeppelins of the same type
                if (zeppelin.type == newZeppelin.type)
                    return false;

                // No colliding zeppelins
                if (zeppelin.collides(newZeppelin))
                    return false;
            }

            if (newZeppelin.x < 0 ||
                newZeppelin.x + newZeppelin.getWidth()-1 > TABLE_COLS - 1 ||
                newZeppelin.y < 0 ||
                newZeppelin.y + newZeppelin.getHeight()-1 > TABLE_ROWS - 1)
            {
                return false;
            }

            this.zeppelins.Add(newZeppelin);

            return true;
        }

        /// <summary>
        /// Remove all zeppelins that have not been found
        /// </summary>
        /// <param name="openPoints"></param>
        public void removeNonOpenZeppelins() {
            foreach (Zeppelin zeppelin in zeppelins) {
                List<Point> points = new List<Point>(this.openPoints);
                if (!(zeppelin.fullyCollides(points)))
                {
                    zeppelins.Remove(zeppelin);
                }
            }
        }
    }
}

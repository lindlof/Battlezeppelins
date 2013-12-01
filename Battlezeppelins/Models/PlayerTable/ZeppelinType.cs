using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Battlezeppelins.Models
{
    public class ZeppelinType
    {
        public static readonly ZeppelinType MOTHER = new ZeppelinType("Mother", 5);
        public static readonly ZeppelinType LEET = new ZeppelinType("Leet", 3);
        public static readonly ZeppelinType NIBBLET = new ZeppelinType("Nibblet", 2);

        public static IEnumerable<ZeppelinType> Values
        {
            get
            {
                yield return MOTHER;
                yield return LEET;
                yield return NIBBLET;
            }
        }

        public static ZeppelinType getByName(string name) {
            foreach (ZeppelinType type in Values)
            {
                if (type.Name == name) return type;
            }
            return null;
        }

        private readonly string name;
        private readonly int length;

        ZeppelinType(string name, int length)
        {
            this.name = name;
            this.length = length;
        }

        public string Name { get { return name; } }

        public int Length { get { return length; } }

        public override string ToString()
        {
            return name;
        }
    }
}
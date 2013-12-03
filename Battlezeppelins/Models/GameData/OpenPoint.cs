using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Battlezeppelins.Models
{
    public class OpenPoint : Point
    {
        [JsonProperty]
        public bool hit { get; private set; }

        private OpenPoint() : base(0, 0) { }

        public OpenPoint(int x, int y, bool hit) : base(x, y)
        {
            this.hit = hit;
        }

        public static OpenPoint deserialize(string serialized)
        {
            var settings = new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };
            OpenPoint deserialized = JsonConvert.DeserializeObject<OpenPoint>(serialized, settings);

            return deserialized;
        }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
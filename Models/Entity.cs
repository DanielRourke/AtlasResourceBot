using System;


namespace AtlasResourceBot.Modles
{
    public enum EntityType{
        Player,
        Crewmember,
        Item,
        Ship,
        Company    
    }
    public class Entity
    {

        public string name { get; set; }
        public int level { get; set; }
        public string company { get; set; }
        public string rank  { get; set; } 
        public EntityType type { get; set; }

    }
}
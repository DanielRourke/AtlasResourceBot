using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AtlasResourceBot.Modles
{
    public enum EntityType{
        Player,
        Crewmember,
        Item,
        Ship,
        Animal,
        Company,
        Bed,
        Unknown
    }
    public class Entity
    {
        public string name { get; set; }
        public int level { get; set; }
        public string company { get; set; }
        public string rank  { get; set; } 
        public EntityType type { get; set; }
        public char side { get; set; }    

        public override string ToString() => JsonSerializer.Serialize<Entity>(this);

        public Entity( Match match, string group)
        {
            side = group[group.Length -1];

            //Get name from corrispoding group name
            if(match.Groups.TryGetValue(group, out var n))
            {
                Console.WriteLine("Found Value");
                name = n.Value;
            }
            else
            {
                Console.WriteLine($"could not get value of {group}");
            }

            //TODO do exceptoin catching
            //Make type from substring
            type = (EntityType)Enum.Parse(typeof(EntityType), 
                        group.Substring(0,group.Length-1));
        


            //get level
            if(match.Groups.TryGetValue("Level"+side, out var outLevel))
            {
                Console.WriteLine("Found Level");
                //TODO add eception
                if(int.TryParse(outLevel.Value, out int i))
                {
                    level = i;
                }
            }
            else
            {
                Console.WriteLine("could not get value level");
            }


            //get company
            if(match.Groups.TryGetValue("Company"+side, out var outCompany))
            {
                Console.WriteLine("Found Value");
                rank = outCompany.Value;
            }
            else
            {
                Console.WriteLine("could not get Company rank");
            }


            //get rank
            if (match.Groups.TryGetValue("Rank" + side, out var outRank))
            {
                Console.WriteLine("Found Rank");
                rank = outRank.Value;
            }
            else
            {
                Console.WriteLine("could not get value rank");
            }

        }
    }
}
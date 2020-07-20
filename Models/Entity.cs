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
        Company    
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
                Console.WriteLine("Found Value");
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

            //get rank
            if(match.Groups.TryGetValue("Rank"+side, out var outRank))
            {
                Console.WriteLine("Found Value");
                rank = outRank.Value;
            }
            else
            {
                Console.WriteLine("could not get value rank");
            }

            //get company
            if(match.Groups.TryGetValue("Company"+side, out var outCompany))
            {
                Console.WriteLine("Found Value");
                rank = outCompany.Value;
            }
            else
            {
                Console.WriteLine("could not get value rank");
            }



           


             //TODO fix this heaping pile into something more dynamic playerA verb ...list of (entities) .add..
                    if(words[0].Equals("Item"))
                    {
                         Console.WriteLine("success item");
                        entityA.name = result.Groups["item"].Value;
                        entityA.type = EntityType.Item;
                    }
                    else if (words[0].Equals("Ship"))
                    {
                        Console.WriteLine("success sss");
                        entityA.name = result.Groups["ship"].Value;
                        entityA.type = EntityType.Ship;
                    }
                    else if (words[0].Equals("Bed"))
                    {
                        Console.WriteLine("success bb");
                        entityA.name = result.Groups["bed"].Value;
                        entityA.type = EntityType.Ship;
                    }
                    else if (words[0].Equals("Company"))
                    {
                        Console.WriteLine("success item");
                        entityA.name = result.Groups["company"].Value;
                        entityA.type = EntityType.Company;
                    }
                    else if (words[0].Equals("Player"))
                    {
                        Console.WriteLine("success pppp");
                        entityA.name = result.Groups["victim"].Value;
                        entityA.type = EntityType.Player;
                        entityA.level =  int.Parse(result.Groups["victimlevel"].Value);
                        entityA.rank = result.Groups["victimrank"].Value;
                        entityA.company = result.Groups["victimcompany"].Value;
                    }
                    else if (words[0].Equals("Crewmember"))
                    {

                        

                        Console.WriteLine("success ccccc");
                        entityA = new Entity();
                        //entityA.type = EntityType.Crewmember;
                        Console.WriteLine(result.Groups.Count);

                        for (int i = 0; i < result.Groups.Count; i++)
                        {
                            Console.WriteLine(result.Groups[i]);
                        }

                        Console.WriteLine("successx");
                        if(result.Groups.TryGetValue("victim", out var o))
                        {
                            Console.WriteLine("Found Value");
                            entityA.name = o.Value;
                        }
                        else
                        {
                            Console.WriteLine("could not get value");
                        }

                       
                        Console.WriteLine("success cccccsssss");

                        
                     
                        entityA.level =  int.Parse(result.Groups["victimlevel"].Value);
                        Console.WriteLine("success ccccssssssssc");
                        entityA.rank = result.Groups["victimrank"].Value;
                        Console.WriteLine("success ccccssssssssssssc");
                        entityA.company = result.Groups["victimcompany"].Value;
                    }
                    else
                    {
                        entityA = null;
                    }
 
                    // if(entityA.type.Equals(EntityType.Player) || entityA.type.Equals(EntityType.Crewmember))
                    // {

                    // }
                    
                    Console.WriteLine("success 3");

                    entityB = null;


                    if(words.Length > 1)
                    {
                        if(words[2].Equals("us"))
                        {
                            entityB.name = "Grease Gang";//TODO FIX hardcoding
                            entityB.type = EntityType.Company;
                        }
                        else if (words[2].Equals("new"))
                        {
                            entityB.name = result.Groups["new"].Value;
                            entityB.type = EntityType.Crewmember;
                        }
                        else if (words[2].Equals("Player"))
                        {
                            entityB.name = result.Groups["attacker"].Value;
                            entityB.type = EntityType.Player;
                        }
                        else if (words[2].Equals("Ship"))
                        {
                            entityB.name = result.Groups["ship"].Value;
                            entityB.type = EntityType.Ship;
                        }

                            
                        if(entityB.type.Equals(EntityType.Player) || entityB.type.Equals(EntityType.Crewmember))
                        {
                            entityB.level =  int.Parse(result.Groups["attackerlevel"].Value);
                            //entityB.rank = result.Groups["rank"].Value;
                            entityB.company = result.Groups["attackercompany"].Value;
                        }
                    }




        }
    }
}
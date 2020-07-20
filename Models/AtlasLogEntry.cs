using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Text.Json;

namespace AtlasResourceBot.Modles
{
    public class AtlasLogEntry
    {
        public DateTime RecivedTime { get; }
        public int GameDay { get; }
        public DateTime GameTime { get; }


        public Entity entityA{ get; }
        public string verb { get; }
        public Entity entityB { get; }

        public char gridReferenceLetter { get; }
        public int gridReferenceNumber { get; }
        public double longitude  { get ;}
        public double latitude { get; }
        
        public override string ToString() => JsonSerializer.Serialize<AtlasLogEntry>(this);
        //dont know how to make this into simlar to macro in c#
        static private string rgx(string s) => $@"(?<{s}>([^()' ]*\s)*[^()' ]*)";

        public AtlasLogEntry(string entry)
        {
            
           // Set log pattern matching types
           //TODO move to func ? 
            Dictionary<string, Regex> patterns = createPatterns();
            Boolean foundMatch = false;
            Console.WriteLine($"Testing {entry}");


            foreach (var pattern in patterns)
            {
                Match result = pattern.Value.Match(entry);
                Console.WriteLine($"{pattern} \n");

                if (result.Success)
                {

                    Console.WriteLine("success");
                    foundMatch = true;
                    //set Recieced Time to Current time
                    RecivedTime = DateTime.Now;

                    //set Type to of log to match dictonary key

                    string[] words = pattern.Key.Split(' ');

                    Console.WriteLine(words.Length);

                    Console.WriteLine("success 1");
                    verb = words[1];
                    Console.WriteLine("success 2");
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




                    //Grab game day
                    //TODO add error reporting
                    if(result.Groups["day"].Success)
                    {
                        if(int.TryParse(result.Groups["day"].Value, out int day))
                        {
                            GameDay = day;
                        }
                    
                    }

                    //Grab game time
                    //TODO add error reporting
                    if (result.Groups["hour"].Success && result.Groups["minute"].Success &&
                            result.Groups["second"].Success)
                    {
                        string time = 
                            result.Groups["hour"].Value + 
                            result.Groups["minute"].Value +
                            result.Groups["second"].Value;

                        if(DateTime.TryParseExact(time, "HH:mm:ss",CultureInfo.InvariantCulture,
                                DateTimeStyles.NoCurrentDateDefault, out DateTime datetime))
                        {
                            GameTime = datetime;
                        }
                    }

                    //Add long and lat if avaible
                    //TODO add error reporting
                    if(result.Groups["long"].Success)
                    {
                        double output = 0;
                        if(double.TryParse(result.Groups["long"].Value, out output))
                        {
                            longitude = output;
                        } 
                    }

                    if(result.Groups["lat"].Success)
                    {
                        double output = 0;
                        if(double.TryParse(result.Groups["lat"].Value, out output))
                        {
                            latitude = output;
                        } 
                    }

                    //take grid
                    if(result.Groups["grid"].Success)
                    {
                        //grab first letter
                        gridReferenceLetter = result.Groups["grid"].Value[0];

                        //take last two char as a number
                        string str = result.Groups["grid"].Value.Substring(1);
                        if(int.TryParse(str, out int num))
                        {
                            gridReferenceNumber = num;
                        }
                    }
                }

            }

            if(!foundMatch)
            {
                Console.WriteLine("Entry not found" + entry);
            }

        }

        private Dictionary<string, Regex> createPatterns()
        {
           Dictionary<string, Regex> patterns = new Dictionary<string, Regex>();

            const string Time = @"Day (?<day>\d{1,4}), (?<time>\d{2}:\d{2}:\d{2}):";
            const string Grid = @"(?<grid>[A-Z]\d)";
            const string Coords = @"\[Long: (?<long>(-|)\d*\.\d*) / Lat: (?<lat>(-|)\d*\.\d*)\]";
            const string Detail = @"(?<detail>\(([^()' ]*)*(\s[^()' ]*)*\))*";
            const string LevelA = @"Lvl (?<victimlvl>\d{1,3})" ;
            const string LevelB = @"Lvl (?<attackerlvl>\d{1,3})" ;

            Console.WriteLine("3");
             Console.WriteLine($@"^{Time} Your '{rgx("ItemA") + Detail}' was destroyed!$");  
            patterns.Add("ItemA destroyed us",
                new Regex($@"^{Time} Your '{rgx("ItemA") + Detail}' was destroyed!$"));  
                Console.WriteLine("4");     
            patterns.Add("Item auto-decay us",
                new Regex(pattern: $@"{Time} Your '{rgx("ItemA")+Detail}' was auto-decay destroyed at {Grid} {Coords}"));
            patterns.Add("Player Killed us",
                new Regex(pattern: $@"{Time} Your Company killed {rgx("PlayerA")} - {LevelA} \({rgx("victimcompany")} - {rgx("victimrank")}\)!"));
            patterns.Add("Crewmember Killed us",
                new Regex(pattern: $@"{Time} Your Company killed {rgx("CrewmemberA")} - {LevelA} \(Crewmember\) \({rgx("company")}\)!"));
                //TODO killed Rodrigo Bill - Lvl 28 (Shoreline Mafia)!
            patterns.Add("Crewmember Retired us",
                new Regex(pattern: $@"{Time} Crew member {rgx("CrewmemberA")} - {LevelA} was killed!"));
            patterns.Add("Crewmember Killed Player",
                new Regex(pattern: $@"{Time} Crew member {rgx("CrewmemberA")} - {LevelA} was killed by {rgx("PlayerB")} - {LevelB} \({rgx("companyB")}\)!"));
            patterns.Add("Bed Removed us",
                new Regex(pattern: $@"{Time} Bed {rgx("BedA")} was removed from the Company!"));
            patterns.Add("Bed RenamedTo New",
                new Regex(pattern: $@"{Time} Bed {rgx("BedA")} was renamed to {rgx("ShipB")}!"));
            patterns.Add("Ship RenamedTo New",
                new Regex(pattern: $@"{Time} Ship {rgx("ShipA")} was renamed to {rgx("ShipB")}!"));
            patterns.Add("Ship beingStolen Company", //TODO sort that out
                new Regex(pattern: $@"{Time} {rgx("CompanyB")} is stealing your {rgx("ShipA")}! ({Grid})"));
            patterns.Add("Ship Interrupted",
                new Regex(pattern: $@"{Time} Your claim of {rgx("ShipA")} has been interrupted! ({Grid})"));

                //Day 355, 10:46:07: joejoe little brother - Lvl 26 destroyed their 'Ship Resources Box (Locked)  (R A T S)')!
	//Line 177: Day 351, 18:10:32: Crew member joejoe little brother - Lvl 23 was killed by a Yeti - Lvl 4!
    //Day 414, 03:51:15: Crew member Turpentine Crow - Lvl 66 was killed by Warrior of the Damned - Lvl 29!
            return patterns;  
        }
    }

}
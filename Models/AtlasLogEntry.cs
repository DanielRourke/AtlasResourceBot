using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;

namespace AtlasResourceBot.Modles
{
    public class AtlasLogEntry
    {
        public DateTime RecivedTime { get; }
        public int GameDay { get; }
        public DateTime GameTime { get; }
        public char gridReferenceLetter { get; }
        public int gridReferenceNumber { get; }
        public double longitude  { get ;}
        public double latitude { get; }

        
        public string verb { get; }
        public Entity entityA{ get; }
        public Entity entityB { get; }

        //dont know how to make this into simlar to macro in c#
        private string rgx(string s) => $@"(?<{s}>([^()' ]*\s)*[^()' ]*)";

        public AtlasLogEntry(string entry)
        {

           // Set log pattern matching types
           //TODO move to func ? 
            Dictionary<string, Regex> patterns = createPatterns();
                

            foreach (var pattern in patterns)
            {
                Match result = pattern.Value.Match(entry);

                if (result.Success)
                {
                    //set Recieced Time to Current time
                    RecivedTime = DateTime.Now;

                    //set Type to of log to match dictonary key

                    string[] words = pattern.Key.Split(' ');

                    verb = words[1];

                    //TODO fix this heaping pile into something more dynamic playerA verb ...list of (entities) .add..
                    if(words[0].Equals("Item"))
                    {
                        entityA.name = result.Groups["item"].Value;
                        entityA.type = EntityType.Item;
                    }
                    else if (words[0].Equals("Ship"))
                    {
                        entityA.name = result.Groups["ship"].Value;
                        entityA.type = EntityType.Ship;
                    }
                    else if (words[0].Equals("Bed"))
                    {
                        entityA.name = result.Groups["bed"].Value;
                        entityA.type = EntityType.Ship;
                    }
                    else if (words[0].Equals("Company"))
                    {
                        entityA.name = result.Groups["company"].Value;
                        entityA.type = EntityType.Ship;
                    }
                    else if (words[0].Equals("Player"))
                    {
                        entityA.name = result.Groups["victim"].Value;
                        entityA.type = EntityType.Player;
                    }
                    else if (words[0].Equals("Crewmember"))
                    {
                        entityA.name = result.Groups["victim"].Value;
                        entityA.type = EntityType.Crewmember;
                    }
 
                    if(entityA.type.Equals(EntityType.Player) || entityA.type.Equals(EntityType.Crewmember))
                    {
                        entityA.level =  int.Parse(result.Groups["victimlevel"].Value);
                        entityA.rank = result.Groups["victimrank"].Value;
                        entityA.company = result.Groups["victimcompany"].Value;
                    }
                    


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
                    else if (words.Length < 2)
                    {
                        //unknown
                        entityB = null;
                    }



                    if(entityB.type.Equals(EntityType.Player) || entityB.type.Equals(EntityType.Crewmember))
                    {
                        entityB.level =  int.Parse(result.Groups["attackerlevel"].Value);
                        //entityB.rank = result.Groups["rank"].Value;
                        entityB.company = result.Groups["attackercompany"].Value;
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

        }

        private Dictionary<string, Regex> createPatterns()
        {
           Dictionary<string, Regex> patterns = new Dictionary<string, Regex>();

            const string rgxTime = @"Day (?<day>\d{1,4}), (?<time>\d{2}):\d{2):\d{2}):";
            const string rgxGrid = @"(?<grid>[A-Z]\d)";
            const string rgxCoords = @"\[Long: (?<long>(-|)\d*\.\d*) / Lat: (?<lat>(-|)\d*\.\d*)\]";
            const string rgxDetail = @"(?<detail>\(([^()' ]*)*(\s[^()' ]*)*\))*";
            const string rgxVictimLevel = @"Lvl (?<victimlvl>\d{1,3})" ;
            const string rgxAttackerLevel = @"Lvl (?<attackerlvl>\d{1,3})" ;

            patterns.Add("Item destroyed us",
                new Regex($@"{rgxTime} Your '{rgx("item") + rgxDetail}' was destroyed!"));        
            patterns.Add("Item auto-decay us",
                new Regex(pattern: $@"{rgxTime} Your '{rgx("item")+rgxDetail}' was auto-decay destroyed at {rgxGrid} {rgxCoords}"));
            patterns.Add("Player Killed us",
                new Regex(pattern: $@"{rgxTime} Your Company killed {rgx("victim")} - {rgxVictimLevel} \({rgx("victimcompany")} - {rgx("victimrank")}\)!"));
            patterns.Add("Crewmember Killed us",
                new Regex(pattern: $@"{rgxTime} Your Company killed {rgx("victim")} - {rgxVictimLevel} \(Crewmember\) \({rgx("company")}\)!"));
                //TODO killed Rodrigo Bill - Lvl 28 (Shoreline Mafia)!
            patterns.Add("Crewmember Retired us",
                new Regex(pattern: $@"{rgxTime} Crew member {rgx("victim")} - {rgxVictimLevel} was killed!"));
            patterns.Add("Crewmember Killed Player",
                new Regex(pattern: $@"{rgxTime} Crew member {rgx("victim")} - {rgxVictimLevel} was killed by {rgx("attacker")} - {rgxAttackerLevel} \({rgx("attackercompany")}\)!"));
            patterns.Add("Bed Removed us",
                new Regex(pattern: $@"{rgxTime} Bed {rgx("bed")} was removed from the Company!"));
            patterns.Add("Bed RenamedTo New",
                new Regex(pattern: $@"{rgxTime} Bed {rgx("bed")} was renamed to {rgx("new")}!"));
            patterns.Add("Ship RenamedTo New",
                new Regex(pattern: $@"{rgxTime} Ship {rgx("ship")} was renamed to {rgx("new")}!"));
            patterns.Add("Company Stealing Ship", //TODO sort that out
                new Regex(pattern: $@"{rgxTime} {rgx("attackercompany")} is stealing your {rgx("ship")}! ({rgxGrid})"));
            patterns.Add("Ship Interrupted",
                new Regex(pattern: $@"{rgxTime} Your claim of {rgx("ship")} has been interrupted! ({rgxGrid})"));

                //Day 355, 10:46:07: joejoe little brother - Lvl 26 destroyed their 'Ship Resources Box (Locked)  (R A T S)')!

            return patterns;  
        }
    }

}
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
        public string action { get; }
        public Entity entityB { get; }

        public char gridReferenceLetter { get; }
        public int gridReferenceNumber { get; }
        public double longitude  { get ;}
        public double latitude { get; }
        
        public override string ToString() => JsonSerializer.Serialize<AtlasLogEntry>(this);
        //dont know how to make this into simlar to macro in c#
        
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
                //Console.WriteLine($"{pattern} \n");

                if (result.Success)
                {

                    Console.WriteLine("success");
                   
                    foundMatch = true;
                    //set Recieced Time to Current time
                    RecivedTime = DateTime.Now;

                    //set Type to of log to match dictonary key
                    string[] words = pattern.Key.Split(' ');

                    //TODO error checking
                    entityA = new Entity(result, words[0]);

                    action = words[1];

                    //add entity if exists
                    entityB = words.Length > 2 ? new Entity(result, words[2]) : null;

                 
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
                    if (result.Groups.TryGetValue("time", out var outTime))
                    {   

                        if(DateTime.TryParseExact(outTime.Value, "HH:mm:ss",CultureInfo.InvariantCulture,
                                DateTimeStyles.NoCurrentDateDefault, out DateTime outDatetime))
                        {
                            GameTime = outDatetime;
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
        //Regex Helper function
        static private string rgx(string s) => $@"(?<{s}>([^()' ]*\s)*[^()' ]*)";

        private Dictionary<string, Regex> createPatterns()
        {
           Dictionary<string, Regex> patterns = new Dictionary<string, Regex>();

            const string time = @"Day (?<day>\d{1,4}), (?<time>\d{2}:\d{2}:\d{2}):";
            const string grid = @"(?<grid>[A-Z]\d)";
            const string coords = @"\[Long: (?<long>(-|)\d*\.\d*) / Lat: (?<lat>(-|)\d*\.\d*)\]";
            const string detail = @"(?<detail>\(([^()' ]*)*(\s[^()' ]*)*\))*";
            const string levelA = @"Lvl (?<victimlvl>\d{1,3})" ;
            const string levelB = @"Lvl (?<attackerlvl>\d{1,3})" ;

            patterns.Add("ItemA destroyed",
                new Regex($@"^{time} Your '{rgx("ItemA") + detail}' was destroyed!$"));   
            patterns.Add("ItemA auto-decay",
                new Regex(pattern: $@"{time} Your '{rgx("ItemA")+detail}' was auto-decay destroyed at {grid} {coords}"));
            patterns.Add("PlayerA Killed",
                new Regex(pattern: $@"{time} Your Company killed {rgx("PlayerA")} - {levelA} \({rgx("CompanyA")} - {rgx("RankA")}\)!"));
            patterns.Add("CrewmemberA Killed",
                new Regex(pattern: $@"{time} Your Company killed {rgx("CrewmemberA")} - {levelA} \(Crewmember\) \({rgx("CompanyA")}\)!"));
                //TODO killed Rodrigo Bill - Lvl 28 (Shoreline Mafia)!
            patterns.Add("CrewmemberA Retired",
                new Regex(pattern: $@"{time} Crew member {rgx("CrewmemberA")} - {levelA} was killed!"));
            patterns.Add("CrewmemberA Killed PlayerB",
                new Regex(pattern: $@"{time} Crew member {rgx("CrewmemberA")} - {levelA} was killed by {rgx("PlayerB")} - {levelB} \({rgx("CompanyB")}\)!"));
            patterns.Add("CrewmemberA Killed AnimalB",
                new Regex(pattern: $@"{time} Crew member {rgx("CrewmemberA")} - {levelA} was killed by {rgx("AnimalB")} - {levelB}!"));
            patterns.Add("BedA Removed",
                new Regex(pattern: $@"{time} Bed {rgx("BedA")} was removed from the Company!"));
            patterns.Add("BedA RenamedTo BedB",
                new Regex(pattern: $@"{time} Bed {rgx("BedA")} was renamed to {rgx("BedB")}!"));
            patterns.Add("ShipA RenamedTo ShipB",
                new Regex(pattern: $@"{time} Ship {rgx("ShipA")} was renamed to {rgx("ShipB")}!"));
            patterns.Add("ShipA beingStolen CompanyB", //TODO sort that out
                new Regex(pattern: $@"{time} {rgx("CompanyB")} is stealing your {rgx("ShipA")}! ({grid})"));
            patterns.Add("ShipA Interrupted",
                new Regex(pattern: $@"{time} Your claim of {rgx("ShipA")} has been interrupted! ({grid})"));

                //Day 355, 10:46:07: joejoe little brother - Lvl 26 destroyed their 'Ship Resources Box (Locked)  (R A T S)')!
	//Line 177: Day 351, 18:10:32: Crew member joejoe little brother - Lvl 23 was killed by a Yeti - Lvl 4!
    //Day 414, 03:51:15: Crew member Turpentine Crow - Lvl 66 was killed by Warrior of the Damned - Lvl 29!
            return patterns;  
        }
    }

}
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

        public string PatternKey { get; }

        public Entity EntityA { get; }
        public string Action { get; }
        public Entity EntityB { get; }

        public char GridReferenceLetter { get; }
        public int GridReferenceNumber { get; }
        public double Longitude { get; }
        public double Latitude { get; }

        public override string ToString() => JsonSerializer.Serialize<AtlasLogEntry>(this);
        //dont know how to make this into simlar to macro in c#

        public AtlasLogEntry(string entry)
        {

            // Set log pattern matching types
            //TODO move to func ? 
            Dictionary<string, Regex> patterns = CreatePatterns();
            Boolean foundMatch = false;
            Console.WriteLine($"Testing {entry}");


            foreach (var pattern in patterns)
            {
                Match result = pattern.Value.Match(entry);
                //Console.WriteLine($"{pattern} \n");

                PatternKey = pattern.Key.ToString();

                if (result.Success)
                {

                    Console.WriteLine("success");

                    foundMatch = true;
                    //set Recieced Time to Current time
                    RecivedTime = DateTime.Now;

                    //set Type to of log to match dictonary key
                    string[] words = pattern.Key.Split(' ');

                    //TODO error checking
                    EntityA = new Entity(result, words[0]);

                    Action = words[1];

                    //add entity if exists
                    EntityB = words.Length > 2 ? new Entity(result, words[2]) : null;


                    //Grab game day
                    //TODO add error reporting
                    if (result.Groups["day"].Success)
                    {
                        if (int.TryParse(result.Groups["day"].Value, out int day))
                        {
                            GameDay = day;
                        }

                    }

                    //Grab game time
                    //TODO add error reporting
                    if (result.Groups.TryGetValue("time", out var outTime))
                    {

                        if (DateTime.TryParseExact(outTime.Value, "HH:mm:ss", CultureInfo.InvariantCulture,
                                DateTimeStyles.NoCurrentDateDefault, out DateTime outDatetime))
                        {
                            GameTime = outDatetime;
                        }
                    }

                    //Add long and lat if avaible
                    //TODO add error reporting
                    if (result.Groups["long"].Success)
                    {
                        double output = 0;
                        if (double.TryParse(result.Groups["long"].Value, out output))
                        {
                            Longitude = output;
                        }
                    }

                    if (result.Groups["lat"].Success)
                    {
                        double output = 0;
                        if (double.TryParse(result.Groups["lat"].Value, out output))
                        {
                            Latitude = output;
                        }
                    }

                    //take grid
                    if (result.Groups["grid"].Success)
                    {
                        //grab first letter
                        GridReferenceLetter = result.Groups["grid"].Value[0];

                        //take last two char as a number
                        string str = result.Groups["grid"].Value.Substring(1);
                        if (int.TryParse(str, out int num))
                        {
                            GridReferenceNumber = num;
                        }
                    }
                    //if succes 
                    break;
                }
            }

            if (!foundMatch)
            {
                Console.WriteLine("Entry not found \n");
            }

        }


        private Dictionary<string, Regex> CreatePatterns()
        {

            Dictionary<string, Regex> patterns = new Dictionary<string, Regex>();

            const string time = @"Day (?<day>\d{1,4}), (?<time>\d{2}:\d{2}:\d{2}):";
            const string grid = @"(?<grid>[A-Z]\d)";
            const string coords = @"\[Long: (?<long>(-|)\d*\.\d*) / Lat: (?<lat>(-|)\d*\.\d*)\]";
            const string detail = @"(?<detail>\(([^()' ]*)*(\s[^()' ]*)*\))*";
            const string levelA = @"Lvl (?<LevelA>\d{1,3})";
            const string levelB = @"Lvl (?<LevelB>\d{1,3})";

            static string animal(string s) => $@"(?<Animal{s}>(Bee|Bear|Cobra|Yeti))";
            static string rgx(string s) => $@"(?<{s}>([^()' ]*\s)*[^()' ]*)";


            // Day 7904, 21:52:19: Your 'Raft' was destroyed!
            //Day 3539, 07:20:32: Your 'Wood Ramp' was destroyed!
            patterns.Add("ItemA destroyed",
                new Regex($@"^{time} Your '{rgx("ItemA") + detail}' was destroyed!$"));

            //Day 3614, 16:29:29: Your 'Stone Floor' was auto-decay destroyed at C9 [Long: -50.61 / Lat: -56.58]!
            patterns.Add("ItemA auto-decay",
                new Regex(pattern: $@"{time} Your '{rgx("ItemA") + detail}' was auto-decay destroyed at {grid} {coords}"));

            //Day 3614, 21:27:32: Your Company killed Five 20 - Lvl 120(Somalia - SVIP)!
            //Day 3614, 17:06:40: Your Company killed Sensei Splinter - Lvl 54 (Arkings - Pirate)!
            //TODO add derive company A from message
            patterns.Add("CompanyA Killed PlayerB",
                new Regex(pattern: $@"{time} Your Company killed {rgx("PlayerB")} - {levelA} \({rgx("CompanyB")} - {rgx("RankB")}\)!"));

            //Day 3541, 19:40:35: Your Company killed Pretty Joe Nine Toes - Lvl 6 (Crewmember) (CrowCove)!
            //Day 3539, 11:40:39: Your Company killed Calico John the Mighty - Lvl 3 (Crewmember)
            patterns.Add("CompanyA Killed CrewmemberB",
                new Regex(pattern: $@"{time} Your Company killed {rgx("CrewmemberB")} - {levelB} \(Crewmember\) (\({rgx("CompanyB")}\))?!"));

            //Day 3541, 18:58:47: Crew member l3ong //\a$t3r - Lvl 55 was killed!
            //Day 3614, 17:06:40: Crew member Sensei Splinter - Lvl 54 was killed!
            patterns.Add("UknownA Killed CrewmemberB",
                new Regex(pattern: $@"{time} Crew member {rgx("CrewmemberB")} - {levelB} was killed!"));

            //Day 3614, 21:29:03: Crew member Sensei Splinter - Lvl 54 was killed by a Bee - Lvl 1!
            //Day 351, 18:10:32: Crew member joejoe little brother - Lvl 23 was killed by a Yeti - Lvl 4!
            patterns.Add("AnimalA Killed CrewmemberB",
                new Regex(pattern: $@"{time} Crew member {rgx("CrewmemberB")} - {levelB} was killed by a {animal("A")} - {levelA}"));

            //Day 3593, 14:06:54: Crew member Noxis Ratman - Lvl 36 was killed by Sensei Splinter - Lvl 54 (Arkings - Pirate)!;
            patterns.Add("PlayerB Killed CrewmemberA",
                    new Regex(pattern: $@"{time} Crew member {rgx("CrewmemberA")} - {levelA} was killed by {rgx("PlayerB")} - {levelB} \({rgx("CompanyB")} (- {rgx("RankB")})?\)!"));


            //Day 7974, 20:14:46: Your 'Bed' was destroyed!
            //Day 7788, 07:19:34: Kashmir_Z Z was removed from the Company!

            //Day 7974, 20:14:50: Bed 1381415434 was removed from the Company!
            // Day 7904, 21:52:26: Bed - 668241229 was removed from the Company!
            patterns.Add("BedA Removed",
                new Regex(pattern: $@"{time} Bed {rgx("BedA")} was removed from the Company!"));

            //Day 7904, 21:52:26: Ship 2021978378 was removed from the Company!
            //Day 7838, 09:05:52: Ship 1016976508 was removed from the Company!
            patterns.Add("ShipA Removed",
            new Regex(pattern: $@"{time} Ship {rgx("ShipA")} was removed from the Company!"));

            //Day 7868, 16:04:39: Bed nuts was renamed to NUTS2!
            // Day 7842, 02:21:31: Bed Bed was renamed to nuts!
            patterns.Add("BedA RenamedTo BedB",
                new Regex(pattern: $@"{time} Bed {rgx("BedA")} was renamed to {rgx("BedB")}!"));

            //Day 7825, 07:17:25: Ship Raft was renamed to Word!
            //Day 7799, 02:12:06: Ship Ramshackle Sloop was renamed to Word!
            patterns.Add("ShipA RenamedTo ShipB",
                new Regex(pattern: $@"{time} Ship {rgx("ShipA")} was renamed to {rgx("ShipB")}!"));

            //Day 7798, 01:43:13: Company renamed to Last of the pathfinders!


            //UP TO HERE
            patterns.Add("CompanyA stealing ShipB", //TODO sort that out
                new Regex(pattern: $@"{time} {rgx("CompanyA")} is stealing your {rgx("ShipB")}! ({grid})"));

            //Day 3522, 13:49:47: Your claim of S.S Cook has been interrupted! (D5)
            patterns.Add("ShipA Interrupted",
                new Regex(pattern: $@"{time} Your claim of {rgx("ShipA")} has been interrupted! ({grid})"));

            //TODO
            //Day 7974, 19:02:29: Your Mama - Lvl 23 (Bear) was killed by Vlad The Impaler - Lvl 94 (Skorms Crack) at M4 [Long: 66.44 / Lat: 50.46]!
            //Day 7917, 17:00:57: Your pooh bear - Lvl 92 (Bear) was killed by a Shark - Lvl 9 at L12 - Southeast Temperate Freeport [Long: 53.49 / Lat: -57.55]!
            //Day 7858, 01:13:22: Your Calico Bob the Wanderer - Lvl 1 (Crewmember) was killed by a Manta ray - Lvl 3 at M9 - Southeast Tropical Freeport [Long: 66.47 / Lat: -17.25]!
            //Day 355, 10:46:07: joejoe little brother - Lvl 26 destroyed their 'Ship Resources Box (Locked)  (R A T S)')!
            //Day 414, 03:51:15: Crew member Turpentine Crow - Lvl 66 was killed by Warrior of the Damned - Lvl 29!

            return patterns;

            //TODO killed Rodrigo Bill - Lvl 28 (Shoreline Mafia)!

        }
    }
}
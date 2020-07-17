using System;
using System.Text.RegularExpressions;

namespace AtlasResourceBot.Modles
{
    public class AtlasLogEntry
    {
        public DateTime RecivedTime { get; }
        public int GameDay { get; }
        public DateTime GameTime { get; }
        public string attacker { get; }
        public string victim { get; }
        public char gridReferenceLetter { get; }
        public int gridReferenceNumber { get; }
        public float Longitude  { get ;}
        public float Latitude { get; }
        public string Type { get; }

        public AtlasLogEntry(string entry)
        {


            const string gametime = @"Day (?<day>\d{1,4}), (?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2}):";
            const string detail = @"(?<detail>\(([^()' ]*)*(\s[^()' ]*)*\))*";
            const string ship = @"Ship (?<ship>([^()' ]*\s)*[^()' ]*)" + detail;
            const string item = @"(?<item>([^()' ]*\s)*[^()' ]*)" + detail;
            const string company = @"(?<company>([^())\s]*\s)*[^())\s]*)";
            const string rank = @"(?<rank>([^())\s]*\s)*[^())\s]*)";
            const string victim =  @"(?<victim>([^())\s]*\s)*[^())\s]*)";
            const string attacker =  @"(?<attacker>([^())\s]*\s)*[^())\s]*)";
            const string level = @"Lvl (?<level>\d{1,3})" ;
            const string grid = @"(?<grid>[A-Z]\d)";
            const string coords = @"\[Long: (?<long>(-|)\d*\.\d*) / Lat: (?<lat>(-|)\d*\.\d*)\]";


            const string bed = "(?<bed>([^()' ]*\s)*[^()' ]*)";
           
            

            "{gametime} Your '{item}' was destroyed!"

            "{gametime} Your '{item}' was auto-decay destroyed at {grid} {coords}"

            "{gametime} Your Company killed {victim} - {level} \({company} - {rank}\)!"
            "{gametime} Your Company killed {victim} - {level} \(Crewmember\) \({company}\)!"

            "{gametime} Crew member {victim} - {level} was killed!"
            "{gametime} Crew member {victim} - {level} was killed by {attacker} - {level} \({company}\)!"

            "{gametime} Bed {bed} was removed from the Company!"
            "{gametime} Bed {bed} was renamed to {rename}!"

            "{gametime} Ship {ship} was renamed to Pop Goes Your Planks!"
            "{gametime} {company} is stealing your {ship}! ({grid})"
            "{gametime} Your claim of {ship} has been interrupted! ({grid})"




    /*
           (Day + int + Time + :)
            Day (?<day>\d{1,}), (?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2}):

            Your '(?<item>(\w*\s)*\(\w*\))' was destroyed!
            Your 'Wood Square (Bed)' was destroyed!
            Your 'Crafting (Bed)' was destroyed!
            Your '{item}' was destroyed!

            Your Company killed (?<company>([^())\s]*\s)*[^())\s]*) - Lvl (?<level>\d{1,3}) \((?<company>([^())\s]*\s)*[^())\s]*) - (?<rank>([^())\s]*\s)*[^())\s]*)\)!
            Your Company killed Riga MorTuss v2 - Lvl 57 (WAKANDA - Supa Soldier)!
            Your Company killed {victim} - {level} \({company} - {rank}\)!

            Your Company killed (\w*\s)*\w* - Lvl (?<level>\d{1,3}) \(Crewmember\) \((?<company>([^())\s]*\s)*[^())\s]*)\)!
            Your Company killed Old Joe Junior - Lvl 9 (Crewmember) (==U-M==)!
            Your Company killed {victim} - {level} \(Crewmember\) \({company}\)!

            Crew member (?<victim>([^())\s]*\s)*[^())\s]*) - Lvl (?<level>\d{1,3}) was killed!
            Crew member {victim} - {level} was killed!

            Crew member (?<victim>([^())\s]*\s)*[^())\s]*) - Lvl (?<level>\d{1,3}) was killed by (?<attacker>([^())\s]*\s)*[^())\s]*) - Lvl (?<level>\d{1,3}) \((?<company>([^())\s]*\s)*[^())\s]*)\)!
            Crew member {victim} - {level} was killed by {attacker} - {level} \({company}\)! 
            Crew member Finn BucketBOI - Lvl 44 was killed by ��� shire ��� - Lvl 117 (Slick Daddy Club ??? - Two-Timer)!

            Bed -1129720323 was removed from the Company!
            Bed {bed} was removed from the Company!

            Bed Bed was renamed to 2ed har!
            Bed {bed} was renamed to 2ed har!

            Your '{item}' was auto-decay destroyed at {coords}



    */

        /*
            prefabs
            words (\w*\s)*\w
            gametime Day (?<day>\d{1,}), (?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2}):
            
            item = name + detail
            company "(?<company>([^())\s]*\s)*[^())\s]*)"
            rank = "(?<rank>([^())\s]*\s)*[^())\s]*)"
            victim =  "(?<victim>([^())\s]*\s)*[^())\s]*)"
            attacker =  "(?<attacker>([^())\s]*\s)*[^())\s]*)"
            level = "Lvl (?<level>\d{1,3})" 
            bed = "(?<bed>([^()' ]*\s)*[^()' ]*)"
            name = (?<name>([^()' ]*\s)*[^()' ]*)
            detail = (?<detail>\(([^()' ]*)*(\s[^()' ]*)*\))*
            grid = (?<grid>[A-Z]\d(- Lawless Region)*)
            coords = \[Long: (?<long>(-|)\d*\.\d*) / Lat: (?<lat>(-|)\d*\.\d*)\]

            Your Company destroyed Heyyyyyyyyyy (Ramshackle Sloop) (Obsidian)!
        */


            /*
            Your 'Wood Square (Bed)' was destroyed!
            Your 'Crafting (Bed)' was destroyed!
            Your Company killed Riga MorTuss v2 - Lvl 57 (WAKANDA - Supa Soldier)!
            Your Bawdy Sue Senior - Lvl 1 (Crewmember) was killed by ��� shire ��� - Lvl 117 (Slick Daddy Club ??? - Two-Timer) at F2 [Long: 1.31 / Lat: 77.59]!
            Bed -1129720323 was removed from the Company!
            Bed Bed was renamed to 2ed har!
            Crew member Finn BucketBOI - Lvl 44 was killed by ��� shire ��� - Lvl 117 (Slick Daddy Club ??? - Two-Timer)!
            Crew member Chile Relleno - Lvl 24 was killed!
            */


            /*
            Your 'Wood Square Ceiling' was destroyed!
            Your 'Crafting (Bed)' was destroyed!

            Your Company killed Riga MorTuss v2 - Lvl 57 (WAKANDA - Supa Soldier)!
            Your Bawdy Sue Senior - Lvl 1 (Crewmember) was killed by ��� shire ��� - Lvl 117 (Slick Daddy Club ??? - Two-Timer) at F2 [Long: 1.31 / Lat: 77.59]!

            'Item type'
            'Item type cotaining bed'
            Company killed Name - lvl + int (Guild - Rank)
            Name - lvl + int (Guild - Rank) at location [long: + float / lat: + float]


            Bed Bed was renamed to 2ed har!
            */


            //Item Destroyed
           // "^Your '(\w*\s)*\w*' was destroyed!$"
            //Day 148, 02:56:55: Your 'Wood Square Ceiling' was destroyed!
            
            //guildy demolished Item
            //Day 148, 14:16:06: Mr welsh Sheep demolished a 'Cannon' at B11 [Long: -75.19 / Lat: -88.94]!

            //Bed removed
            //Day 147, 17:52:14: Bed -1129720323 was removed from the Company!
            
            //Bed Destroyed
            //Day 147, 17:53:08: Your 'Crafting (Bed)' was destroyed!

            //bed Renamed
            //Day 147, 17:17:17: Bed Bed was renamed to 2ed har!

            //bed added

            //company Killed Someone
            //Day 148, 10:10:21: Your Company killed Riga MorTuss v2 - Lvl 57 (WAKANDA - Supa Soldier)!

            //guildy Murded Someone
            //Day 148, 10:18:25: Crew member Finn BucketBOI - Lvl 44 was killed by ��� shire ��� - Lvl 117 (Slick Daddy Club ??? - Two-Timer)!

            //Crew member was Murdered By
            //Your Bawdy Sue Senior - Lvl 1 (Crewmember) was killed by ���
            // shire ��� - Lvl 117 (Slick Daddy Club ??? - Two-Timer) at F2 [Long: 1.31 / Lat: 77.59]!

            //Crew member killed by unknown
            //Day 148, 15:11:08: Crew member Chile Relleno - Lvl 24 was killed!

            //crew animal death

            ///tame death

            //Ship destroyede

            //

            //Day \d{1,}, \d{2}:\d{2}:\d{2}: Your '(\w*\s)*\w*' was destroyed!

          //  Day 358, 17:39:47: Your 'Wooden Chair' was destroyed!
            //Day \d{1,}, \d{2}:\d{2}\d{2}: Your '(\w*\s)*\w*' was destroyed!
        }
    }
}
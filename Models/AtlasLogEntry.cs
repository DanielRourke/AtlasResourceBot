using System;

namespace AtlasResourceBot.Modles
{
    public class AtlasLogEntry
    {
        public int GameDay { get; }
        public DateTime GameTime { get; }
        public string Assailant { get; }
        public string Casualty { get; }
        public char gridReferenceLetter { get; }
        public int gridReferenceNumber { get; }
        public float Longitude  { get ;}
        public float Latitude { get; }
        public string Type { get; }

        public AtlasLogEntry(string entry)
        {
            //Item Destroyed
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

            m

            //Ship destroyede

            //
        }
    }
}
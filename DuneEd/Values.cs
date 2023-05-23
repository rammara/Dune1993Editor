namespace DuneEd
{
    public static class Values
    {
        /*
         * This is a 'magic sequence indicating the beginning of the first sietch in the file.
         */
        public static readonly byte[] SietchesStartPattern =
            { 0x02, 0x01, 0x15, 0x19, 0xFC, 0xFF, 0xEB, 0x5C }; // Carthag first pattern

        public const int SIETCHES_COUNT = 70;

        public enum GameObjectTypes
        {
            Sietch, Troop
        }

        public static readonly Dictionary<byte, string>
            FirstNames = new()
            {
                { 0x1, "Arrakeen" },
                { 0x2, "Carthag" },
                { 0x3, "Tuono" },
                { 0x4, "Habbanya" },
                { 0x5, "Oxtyn" },
                { 0x6, "Tsympo" },
                { 0x7, "Bledan" },
                { 0x8, "Ergsun" },
                { 0x9, "Haga" },
                { 0xA, "Cielago" },
                { 0xB, "Sihaya" },
                { 0xC, "Celimyn" }
            };

        public static readonly Dictionary<byte, string>
            LastNames = new()
            {
                { 0x1, " (Atreides)" },
                { 0x2, " (Harkonnen)" },
                { 0x3, "-Tabr" },
                { 0x4, "-Timin" },
                { 0x5, "-Tuek" },
                { 0x6, "-Harg" },
                { 0x7, "-Clam" },
                { 0x8, "-Tsymyn" },
                { 0x9, "-Siet" },
                { 0xA, "-Pyons" },
                { 0xB, "-Pyort" }
            };

        public static readonly Dictionary<byte, string>
            TroopLocations = new()
            {
                { 0x01, "South of the siecth" },
                { 0x02, "South East of the siecth" },
                { 0x03, "South West of the siecth" },
                { 0x04, "East of the siecth" },
                { 0x05, "West of the siecth" },
                { 0x06, "North East of the siecth" },
                { 0x07, "North West of the siecth" },
                { 0x08, "North of the siecth" }
            };

        [Flags]
        public enum SietchStatus
        {
            Visible = 0x00,
            VegetationPresent = 0x01,
            InBattle = 0x02,
            UnknownA = 0x04,
            UnknownB = 0x08,
            InventoryVisible = 0x10,
            WindtrapPresent = 0x20,
            AreaProspected = 0x40,
            NotDiscovered = 0x80,
            UndiscoveredWindtrap = 0xA0,
            TuonoTabrInitial = 0xE0
        } // enum SietchStatus

        [Flags]
        public enum Equipment : byte
        {
            Unknown =           0x00,
            None =              0x01,
            Bulbs =             0x02,
            Atomics =           0x04,
            WeirdingModules =   0x08,
            LaserGuns =         0x10,
            KrysKnives =        0x20,
            Ornithopter =       0x40,
            SpiceHarvester =    0x80
        }

        public static string GetLocation(byte locationvalue)
        {
            if (TroopLocations.ContainsKey(locationvalue)) return TroopLocations[locationvalue];
            return "Elsewhere";
        } // GetLocation

        public static string GetOccupationString(byte jobvalue)
        {
            if (Occupations.ContainsKey(jobvalue)) return Occupations[jobvalue];
            else if (jobvalue >= 0x40 && jobvalue <= 0x7f) return "Moving to another place";
            else if (jobvalue >= 0x80 && jobvalue <= 0x9f) return "Not yet hired.";
            else if (jobvalue > 0xa0) return "Enslaved by Harkonnen";
            else return "Unknown (Data corrputed)";
        } // GetOccupationString

        public static bool IsOccupationOffsite(byte occupationByte)
        {
            return
                (occupationByte == 0x03) ||
                (occupationByte == 0x05) ||
                (occupationByte == 0x06) ||
                (occupationByte == 0x07) ||
                (occupationByte == 0x0B) ||
                (occupationByte == 0x0F) ||
                (occupationByte == 0x15) ||
                (occupationByte == 0x16) ||
                (occupationByte == 0x17) ||
                (occupationByte == 0x1B) ||
                (occupationByte == 0x1F) ||
                (occupationByte == 0x23) ||
                (occupationByte >= 0x40 && occupationByte <= 0x7f);
        }

        public static readonly Dictionary<byte, string> 
            Occupations = new()
        {
            { 0x00, "Spice mining" },
            { 0x01, "Spice prospecting" },
            { 0x02, "Waiting orders" },
            { 0x03, "Spice miners going to search equipment" },
            { 0x04, "Military training" },
            { 0x05, "Espionage" },
            { 0x06, "Attacking" },
            { 0x07, "Militaries going to search equipment" },
            { 0x08, "Irrigation & Tree care" },
            { 0x09, "Wind-trap assembly" },
            { 0x0A, "Bulb growing" },
            { 0x0B, "Ecologists going to search equipment" },
            { 0x0C, "Blue harkonnen spice mining" },
            { 0x0D, "Blue harkonnen prospecting" },
            { 0x0E, "Blue harkonnen waiting orders" },
            { 0x0F, "Blue harkonnen spice miners going to search equipment" },
            { 0x10, "Finished job : Spice mining" },
            { 0x11, "Finished job : Spice prospecting" },
            { 0x12, "Finished job : Waiting orders" },
            { 0x13, "Finished job : Spice miners going to search equipment" },
            { 0x14, "Finished job : Military training" },
            { 0x15, "Finished job : Espionage" },
            { 0x16, "Finished job : Attacking" },
            { 0x17, "Finished job : Militaries going to search equipment" },
            { 0x18, "Finished job : Irrigation & Tree care" },
            { 0x19, "Finished job : Wind-trap assembly" },
            { 0x1A, "Finished job : Bulb growing" },
            { 0x1B, "Finished job : Ecologists going to search equipment" },
            { 0x1C, "Finished job : Blue harkonnen spice mining" },
            { 0x1D, "Finished job : Blue harkonnen prospecting" },
            { 0x1E, "Finished job : Blue harkonnen waiting orders" },
            { 0x1F, "Finished job : Blue harkonnen spice miners going to search equipment" },
            { 0x20, "Spice mining - No more orders to give" },
            { 0x21, "Troop prospecting captured - No more orders to give" },
            { 0x22, "Troop apologize for been captured" },
            { 0x23, "Spice miners going to search a spice harvester - No more orders to give" }
        };
            /* ...nearly like[0x00]->[0x0F] upper list jobs
            [0x40]->[0x7F] = Moving to another place
            [0x80]->[0x9F] = Not yet hired
            [0xA0]->[0xFF] = Complain about slaving by harkonnen*/
    } // class Values
} // namespace

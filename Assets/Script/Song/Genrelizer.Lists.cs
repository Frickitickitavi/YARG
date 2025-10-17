﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YARG.Song
{
    public static partial class Genrelizer
    {
        // Official
        private const string ALTERNATIVE = "alternative";
        private const string AMBIENT_DRONE = "ambient/drone";
        private const string BALLAD = "ballad";
        private const string BLUES = "blues";
        private const string CHILDRENS_MUSIC = "children's music";
        private const string CHIPTUNE = "chiptune";
        private const string CLASSIC_ROCK = "classic rock";
        private const string CLASSICAL = "classical";
        private const string COUNTRY = "country";
        private const string DANCE = "dance";
        private const string DEATH_BLACK_METAL = "death/black metal";
        private const string DISCO = "disco";
        private const string DJENT = "djent";
        private const string DNB_BREAKBEAT_JUNGLE = "dnb/breakbeat/jungle";
        private const string DOOM_METAL = "doom metal";
        private const string DUBSTEP = "dubstep";
        private const string ELECTRONIC = "electronic";
        private const string ELECTRONIC_ROCK = "electronic rock";
        private const string EMO = "emo";
        private const string FOLK = "folk";
        private const string FUSION = "fusion";
        private const string GLAM = "glam";
        private const string GLITCH = "glitch";
        private const string GRINDCORE = "grindcore";
        private const string GROOVE_METAL = "groove metal";
        private const string GRUNGE = "grunge";
        private const string HARD_ROCK = "hard rock";
        private const string HARDCORE_EDM = "hardcore edm";
        private const string HEAVY_METAL = "heavy metal";
        private const string HIP_HOP_RAP = "hip-hop/rap";
        private const string HOLIDAY = "holiday";
        private const string HOUSE = "house";
        private const string IDM = "idm";
        private const string INDIE_ROCK = "indie rock";
        private const string INDUSTRIAL = "industrial";
        private const string INSPIRATIONAL = "inspirational";
        private const string J_POP = "j-pop";
        private const string J_ROCK = "j-rock";
        private const string JAZZ = "jazz";
        private const string K_POP = "k-pop";
        private const string LATIN = "latin";
        private const string MASHUP = "mashup";
        private const string MATH_ROCK = "math rock";
        private const string MELODIC_POWER_METAL = "melodic/power metal";
        private const string METALCORE = "metalcore";
        private const string NOISE = "noise";
        private const string NEW_WAVE = "new wave";
        private const string NOVELTY = "novelty";
        private const string NU_METAL = "nu-metal";
        private const string ORCHESTRAL = "orchestral";
        private const string POP = "pop";
        private const string POP_PUNK = "pop punk";
        private const string POP_ROCK = "pop-rock";
        private const string POST_HARDCORE = "post-hardcore";
        private const string PROGRESSIVE = "progressive";
        private const string PSYCHEDELIC = "psychedelic";
        private const string PUNK = "punk";
        private const string RNB_SOUL_FUNK = "r&b/soul/funk";
        private const string REGGAE_SKA = "reggae/ska";
        private const string ROCK = "rock";
        private const string ROCK_AND_ROLL = "rock & roll";
        private const string SOUNDTRACK = "soundtrack";
        private const string SOUTHERN_ROCK = "southern rock";
        private const string SURF_ROCK = "surf rock";
        private const string SYNTHPOP_ELECTROPOP = "synthpop/electropop";
        private const string TECHNO = "techno";
        private const string THRASH_SPEED_METAL = "thrash/speed metal";
        private const string TRANCE = "trance";
        private const string TRAP = "trap";
        private const string URBAN = "urban";
        private const string WORLD = "world";
        private const string OTHER = "other";

        // From Magma, but not official
        private const string METAL = "metal";
        private const string POP_DANCE_ELECTRONIC = "pop/dance/electronic";
        private const string PROG = "prog";



        // Mapping from official genre name to localization key
        public static Dictionary<string, string> GENRE_LOCALIZATION_KEYS = new(StringComparer.OrdinalIgnoreCase)
        {
            { ALTERNATIVE, "Alternative"},
            { AMBIENT_DRONE, "AmbientDrone"},
            { BALLAD, "Ballad"},
            { BLUES, "Blues"},
            { CHILDRENS_MUSIC, "ChildrensMusic"},
            { CHIPTUNE, "Chiptune"},
            { CLASSIC_ROCK, "ClassicRock"},
            { CLASSICAL, "Classical"},
            { COUNTRY, "Country"},
            { DANCE, "Dance"},
            { DEATH_BLACK_METAL, "DeathBlackMetal"},
            { DISCO, "Disco"},
            { DJENT, "Djent"},
            { DNB_BREAKBEAT_JUNGLE, "DnbBreakbeatJungle"},
            { DOOM_METAL, "DoomMetal"},
            { DUBSTEP, "Dubstep"},
            { ELECTRONIC, "Electronic"},
            { ELECTRONIC_ROCK, "ElectronicRock"},
            { EMO, "Emo"},
            { FOLK, "Folk"},
            { FUSION, "Fusion"},
            { GLAM, "Glam"},
            { GLITCH, "Glitch"},
            { GRINDCORE, "Grindcore"},
            { GROOVE_METAL, "GrooveMetal" },
            { GRUNGE, "Grunge"},
            { HARD_ROCK, "HardRock" },
            { HARDCORE_EDM, "HardcoreEDM"},
            { HEAVY_METAL, "HeavyMetal"},
            { HIP_HOP_RAP, "HipHopRap"},
            { HOLIDAY, "Holiday"},
            { HOUSE, "House"},
            { IDM, "IDM"},
            { INDIE_ROCK, "IndieRock"},
            { INDUSTRIAL, "Industrial"},
            { INSPIRATIONAL, "Inspirational"},
            { J_POP, "JPop"},
            { J_ROCK, "JRock"},
            { JAZZ, "Jazz"},
            { K_POP, "KPop"},
            { LATIN, "Latin"},
            { MASHUP, "Mashup"},
            { MATH_ROCK, "MathRock"},
            { MELODIC_POWER_METAL, "MelodicPowerMetal"},
            { METALCORE, "Metalcore"},
            { NOISE, "Noise"},
            { NEW_WAVE, "NewWave"},
            { NOVELTY, "Novelty"},
            { NU_METAL, "NuMetal"},
            { ORCHESTRAL, "Orchestral"},
            { POP, "Pop"},
            { POP_PUNK, "PopPunk"},
            { POP_ROCK, "PopRock"},
            { POST_HARDCORE, "PostHardcore"},
            { PROGRESSIVE, "Progressive"},
            { PSYCHEDELIC, "Psychedelic" },
            { PUNK, "Punk"},
            { RNB_SOUL_FUNK, "RnbSoulFunk"},
            { REGGAE_SKA, "ReggaeSka"},
            { ROCK, "Rock"},
            { ROCK_AND_ROLL, "RockAndRoll"},
            { SOUNDTRACK, "Soundtrack"},
            { SOUTHERN_ROCK, "SouthernRock"},
            { SURF_ROCK, "SurfRock"},
            { SYNTHPOP_ELECTROPOP, "SynthpopElectropop"},
            { TECHNO, "Techno"},
            { THRASH_SPEED_METAL, "ThrashSpeedMetal"},
            { TRANCE, "Trance"},
            { TRAP, "Trap"},
            { URBAN, "Urban"},
            { WORLD, "World"},
            { OTHER, "Other"}
        };

        public static Dictionary<(string genre, string subgenre), (string genre, string subgenre)> MAGMA_MAPPINGS = new(new TupleStringComparer())
        {
            { (ALTERNATIVE, "college"), (ALTERNATIVE, "college rock") },
            { (ALTERNATIVE, "other"), (ALTERNATIVE, null) },

            { (BLUES, "acoustic"), (BLUES, "acoustic blues") },
            { (BLUES, "chicago"), (BLUES, "chicago blues") },
            { (BLUES, "classic"), (BLUES, "classic blues") },
            { (BLUES, "contemporary"), (BLUES, "contemporary blues") },
            { (BLUES, "country"), (BLUES, "country blues") },
            { (BLUES, "delta"), (BLUES, "delta blues") },
            { (BLUES, "electric"), (BLUES, "electric blues") },
            { (BLUES, "other"), (BLUES, null) },

            { (COUNTRY, "alternative"), (COUNTRY, "alternative country") },
            { (COUNTRY, "contemporary"), (COUNTRY, "contemporary country") },
            { (COUNTRY, "outlaw"), (COUNTRY, "outlaw country") },
            { (COUNTRY, "traditional folk"), (FOLK, "traditional folk") },
            { (COUNTRY, "other"), (COUNTRY, null) },

            { (GLAM, "other"), (GLAM, null) },

            { (HIP_HOP_RAP, "gangsta"), (HIP_HOP_RAP, "gangsta rap") },
            { (HIP_HOP_RAP, "other"), (HIP_HOP_RAP, null) },

            { (INDIE_ROCK, "math rock"), (MATH_ROCK, null) },
            { (INDIE_ROCK, "noise"), (NOISE, "noise rock") },
            { (INDIE_ROCK, "other"), (INDIE_ROCK, null) },

            { (JAZZ, "contemporary"), (JAZZ, "contemporary jazz") },
            { (JAZZ, "experimental"), (JAZZ, "experimental jazz") },
            { (JAZZ, "smooth"), (JAZZ, "smooth jazz") },
            { (JAZZ, "other"), (JAZZ, null) },

            { (METAL, "alternative"), (HEAVY_METAL, "alternative metal") },
            { (METAL, "black"), (DEATH_BLACK_METAL, "black metal") },
            { (METAL, "core"), (METALCORE, null) },
            { (METAL, "death"), (DEATH_BLACK_METAL, "death metal") },
            { (METAL, "hair"), (HEAVY_METAL, "hair metal") },
            { (METAL, "industrial"), (INDUSTRIAL, "industrial metal") },
            { (METAL, "metal"), (HEAVY_METAL, null) },
            { (METAL, "power"), (MELODIC_POWER_METAL, "power metal") },
            { (METAL, "prog"), (HEAVY_METAL, "progressive metal") },
            { (METAL, "speed"), (THRASH_SPEED_METAL, "speed metal") },
            { (METAL, "thrash"), (THRASH_SPEED_METAL, "thrash metal") },
            { (METAL, "other"), (HEAVY_METAL, null) },

            { (NEW_WAVE, "synthpop"), (SYNTHPOP_ELECTROPOP, "synthpop") },
            { (NEW_WAVE, "other"), (NEW_WAVE, null) },

            { (POP_DANCE_ELECTRONIC, "ambient"), (AMBIENT_DRONE, "ambient") },
            { (POP_DANCE_ELECTRONIC, "breakbeat"), (DNB_BREAKBEAT_JUNGLE, "breakbeat") },
            { (POP_DANCE_ELECTRONIC, "chiptune"), (CHIPTUNE, null) },
            { (POP_DANCE_ELECTRONIC, "dance"), (DANCE, null) },
            { (POP_DANCE_ELECTRONIC, "downtempo"), (ELECTRONIC, "downtempo") },
            { (POP_DANCE_ELECTRONIC, "dub"), (DUBSTEP, null) },
            { (POP_DANCE_ELECTRONIC, "drum and bass"), (DNB_BREAKBEAT_JUNGLE, "drum and bass") },
            { (POP_DANCE_ELECTRONIC, "electronica"), (ELECTRONIC, "electronica") },
            { (POP_DANCE_ELECTRONIC, "garage"), (ELECTRONIC, "garage") },
            { (POP_DANCE_ELECTRONIC, "hardcore dance"), (HARDCORE_EDM, "hardcore dance") },
            { (POP_DANCE_ELECTRONIC, "house"), (HOUSE, null) },
            { (POP_DANCE_ELECTRONIC, "industrial"), (INDUSTRIAL, null) },
            { (POP_DANCE_ELECTRONIC, "techno"), (TECHNO, "") },
            { (POP_DANCE_ELECTRONIC, "trance"), (TRANCE, "") },
            { (POP_DANCE_ELECTRONIC, "other"), (ELECTRONIC, null) },

            { (POP_ROCK, "contemporary"), (POP_ROCK, "contemporary pop-rock") },
            { (POP_ROCK, "disco"), (DISCO, null) },
            { (POP_ROCK, "motown"), (RNB_SOUL_FUNK, "motown") },
            { (POP_ROCK, "pop"), (POP, "pop-rock") },
            { (POP_ROCK, "rhythm and blues"), (RNB_SOUL_FUNK, "rhythm and blues") },
            { (POP_ROCK, "soul"), (RNB_SOUL_FUNK, "soul") },
            { (POP_ROCK, "teen"), (POP, "teen pop") },
            { (POP_ROCK, "other"), (POP_ROCK, null) },

            { (PROG, "prog rock"), (PROGRESSIVE, null) },

            { (PUNK, "alternative"), (PUNK, "alternative punk rock") },
            { (PUNK, "classic"), (PUNK, "classic punk rock") },
            { (PUNK, "garage"), (PUNK, "garage punk") },
            { (PUNK, "hardcore"), (PUNK, "hardcore punk") },
            { (PUNK, "pop"), (POP_PUNK, null) },
            { (PUNK, "other"), (PUNK, null) },

            { (RNB_SOUL_FUNK, "disco"), (DISCO, null) },
            { (RNB_SOUL_FUNK, "other"), (RNB_SOUL_FUNK, null) },

            { (REGGAE_SKA, "other"), (REGGAE_SKA, null) },

            { (ROCK, "arena"), (ROCK, "arena rock") },
            { (ROCK, "blues"), (ROCK, "blues rock") },
            { (ROCK, "folk rock"), (FOLK, "folk rock") },
            { (ROCK, "funk"), (RNB_SOUL_FUNK, "funk") },
            { (ROCK, "garage"), (ROCK, "garage rock") },
            { (ROCK, "psychedelic"), (ROCK, "psychedelic rock") },
            { (ROCK, "reggae"), (REGGAE_SKA, "reggae") },
            { (ROCK, "rockabilly"), (ROCK_AND_ROLL, "rockabilly") },
            { (ROCK, "rock and roll"), (ROCK_AND_ROLL, null) },
            { (ROCK, "ska"), (REGGAE_SKA, "ska") },
            { (ROCK, "surf"), (SURF_ROCK, null) },
            { (ROCK, "other"), (ROCK, null) },

            { (URBAN, "alternative rap"), (HIP_HOP_RAP, "alternative rap") },
            { (URBAN, "downtempo"), (ELECTRONIC, "downtempo") },
            { (URBAN, "drum and bass"), (DNB_BREAKBEAT_JUNGLE, "drum and bass") },
            { (URBAN, "dub"), (DUBSTEP, null) },
            { (URBAN, "electronica"), (ELECTRONIC, "electronica") },
            { (URBAN, "gangsta"), (HIP_HOP_RAP, "gangsta rap") },
            { (URBAN, "garage"), (ELECTRONIC, "garage") },
            { (URBAN, "hardcore dance"), (HARDCORE_EDM, "hardcore dance") },
            { (URBAN, "hardcore rap"), (HIP_HOP_RAP, "hardcore rap") },
            { (URBAN, "hip hop"), (HIP_HOP_RAP, "hip-hop") },
            { (URBAN, "industrial"), (INDUSTRIAL, null) },
            { (URBAN, "old school hip hop"), (HIP_HOP_RAP, "oldschool hip-hop") },
            { (URBAN, "rap"), (HIP_HOP_RAP, "rap") },
            { (URBAN, "trip hop"), (HIP_HOP_RAP, "trip hop") },
            { (URBAN, "underground rap"), (HIP_HOP_RAP, "underground rap") },
            { (URBAN, "other"), (URBAN, null) },

            { (OTHER, "ambient"), (AMBIENT_DRONE, "ambient") },
            { (OTHER, "breakbeat"), (DNB_BREAKBEAT_JUNGLE, "breakbeat") },
            { (OTHER, "chiptune"), (CHIPTUNE, null) },
            { (OTHER, "classical"), (CLASSICAL, null) },
            { (OTHER, "contemporary folk"), (FOLK, "contemporary folk") },
            { (OTHER, "dance"), (DANCE, null) },
            { (OTHER, "electronica"), (ELECTRONIC, "electronica") },
            { (OTHER, "house"), (HOUSE, null) },
            { (OTHER, "techno"), (TECHNO, null) },
            { (OTHER, "trance"), (TRANCE, null) },
        };

        private class TupleStringComparer : IEqualityComparer<(string genre, string subgenre)>
        {
            public bool Equals((string genre, string subgenre) x, (string genre, string subgenre) y)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(x.Item1, y.Item1) && StringComparer.OrdinalIgnoreCase.Equals(x.Item2, y.Item2);
            }

            public int GetHashCode((string genre, string subgenre) obj)
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item1) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item2);
            }
        }
    }
}

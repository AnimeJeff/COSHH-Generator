

using System;
using System.Collections.Generic;
using System.Linq;

namespace COSHH_Generator
{

    class Substance
    {
        public string Name = "";
        public string MassVolume = "";
        public SafetyData SafetyData = new SafetyData();
        public Substance(string name, string massVolume = "N/A")
        {
            Name = name;
            MassVolume = massVolume;
        }
    }

    class SafetyData
    {
        private Hazard hazards = Hazard.NONE;
        public bool Eyes => hazards.HasFlag(Hazard.EYES);
        public bool Skin => hazards.HasFlag(Hazard.SKIN);
        public bool Inhalation => hazards.HasFlag(Hazard.INHALATION);
        public bool Ingestion => hazards.HasFlag(Hazard.INGESTION);
        public bool ConsultSpill => hazards.HasFlag(Hazard.CONSULT_SPILL);
        public bool Goggles => true;//hazards.HasFlag(Hazard.EYES);
        public bool LabCoat => true;// hazards.HasFlag(Hazard.SKIN);
        public bool Gloves => true;// hazards.HasFlag(Hazard.SKIN);
        public bool Fumehood => hazards.HasFlag(Hazard.INHALATION);
        public bool NoNakedFlames => hazards.HasFlag(Hazard.FLAMMABLE);
        public bool UseWaterBath => hazards.HasFlag(Hazard.USE_WATER_BATH);
        public bool NotUseWhenPregnant => hazards.HasFlag(Hazard.NOT_USE_WHEN_PREGNANT);
        public bool NotNearWater => hazards.HasFlag(Hazard.NOT_NEAR_WATER);
        public bool Dropwise => hazards.HasFlag(Hazard.DROPWISE);
        public bool NotExposeToAir => hazards.HasFlag(Hazard.NOT_EXPOSE_TO_AIR);
        public bool Flammable => hazards.HasFlag(Hazard.FLAMMABLE);
        public bool Explosive => hazards.HasFlag(Hazard.EXPLOSIVE);
        public bool ThermalRunaway => hazards.HasFlag(Hazard.THERMAL_RUNAWAY);
        public bool GasRelease => hazards.HasFlag(Hazard.GAS_RELEASE);
        public bool Malodorous => hazards.HasFlag(Hazard.MALODOROUS);
        public bool GasUnderPressure => hazards.HasFlag(Hazard.GAS_UNDER_PRESSURE);

        private List<string> hazardCodes = new List<string>();
        public List<string> HazardCodes 
        {
            get => hazardCodes;
            set => ParseHazardCodes(value);
        }
        private List<string> precautionaryCodes = new List<string>();
        public List<string> PrecautionaryCodes
        {
            get => precautionaryCodes;
            set => ParsePrecautionaryCodes(value);
        }

        public List<string> HazardStatements
        {
            get => hazardCodes.Count == 0 ? new List<string>() { "N/A" } : hazardCodes.Select(code =>  $"{code} – {HazardCodeDict[code]};" ).ToList();
        }
        public List<string> PrecautionaryStatements
        {
            get => precautionaryCodes.Select(code => $"{code} – {PrecautionaryCodeDict[code]};").ToList();
        }

        public SafetyData() { }
        public SafetyData(List<string> hazardCodes, List<string> precautionaryCodes)
        {
            ParseHazardCodes(hazardCodes);
            ParsePrecautionaryCodes(precautionaryCodes);
        }

        private void ParseHazardCodes(List<string> hazardCodes)
        {
            this.hazardCodes = hazardCodes;
            foreach (string hazardCode in hazardCodes)
            {
                var hazard = Hazard.NONE;
                if (!HazardCodeMap.TryGetValue(hazardCode, out hazard))
                {
                    //Console.WriteLine("Cannot find code " + hazardCode);
                    continue;
                }
                hazards |= hazard;
            }
        }

        private void ParsePrecautionaryCodes(List<string> precautionaryCodes)
        {
            this.precautionaryCodes = precautionaryCodes;
            foreach (string precautionaryCode in precautionaryCodes)
            {
                var hazard = Hazard.NONE;
                if (!PrecautionaryCodeMap.TryGetValue(precautionaryCode, out hazard))
                {
                    //Console.WriteLine("Cannot find code " + precautionaryCode);
                    continue;
                };
                hazards |= hazard;

            }
        }

        [Flags]
        private enum Hazard
        {
            NONE = 0b_0000_0000_0000_0000_0000_0000,
            EYES = 0b_0000_0000_0000_0000_0000_0001,
            SKIN = 0b_0000_0000_0000_0000_0000_0010,
            INHALATION = 0b_0000_0000_0000_0000_0000_0100,
            INGESTION = 0b_0000_0000_0000_0000_0000_1000,
            CONSULT_SPILL = 0b_0000_0000_0000_0000_0001_0000,
            GOGGLES = 0b_0000_0000_0000_0000_0010_0000,
            LAB_COAT = 0b_0000_0000_0000_0000_0100_0000,
            GLOVES = 0b_0000_0000_0000_0000_1000_0000,
            FUMEHOOD = 0b_0000_0000_0000_0001_0000_0000,
            NO_NAKED_FLAMES = 0b_0000_0000_0000_0010_0000_0000,
            USE_WATER_BATH = 0b_0000_0000_0000_0100_0000_0000,
            NOT_USE_WHEN_PREGNANT = 0b_0000_0000_0000_1000_0000_0000,
            NOT_NEAR_WATER = 0b_0000_0000_0001_0000_0000_0000,
            DROPWISE = 0b_0000_0000_0010_0000_0000_0000,
            NOT_EXPOSE_TO_AIR = 0b_0000_0000_0100_0000_0000_0000,
            FLAMMABLE = 0b_0000_0000_1000_0000_0000_0000,
            EXPLOSIVE = 0b_0000_0001_0000_0000_0000_0000,
            THERMAL_RUNAWAY = 0b_0000_0010_0000_0000_0000_0000,
            GAS_RELEASE = 0b_0000_0100_0000_0000_0000_0000,
            MALODOROUS = 0b_0000_1000_0000_0000_0000_0000,
            GAS_UNDER_PRESSURE = 0b_0001_0000_0000_0000_0000_0000,
        }

        private Dictionary<string, Hazard> HazardCodeMap = new Dictionary<string, Hazard>
        {
            {"H200",Hazard.EXPLOSIVE },                    // Unstable explosive
            {"H201",Hazard.EXPLOSIVE },                    // Explosive: mass explosion hazard
            {"H202",Hazard.EXPLOSIVE },                    // Explosive: severe projection hazard
            {"H203",Hazard.EXPLOSIVE },                    // Explosive: fire, blast or projection hazard
            {"H204",Hazard.EXPLOSIVE },                    // Fire or projection hazard
            {"H205",Hazard.EXPLOSIVE | Hazard.FLAMMABLE},  // May mass explode in fire
            {"H206",Hazard.FLAMMABLE },                    // Fire, blast or projection hazard: increased risk of explosion if desensitizing agent is reduced
            {"H207",Hazard.FLAMMABLE },                    // Fire or projection hazard; increased risk of explosion if desensitizing agent is reduced
            {"H208",Hazard.FLAMMABLE },                    // Fire hazard; increased risk of explosion if desensitizing agent is reduced
            {"H209",Hazard.EXPLOSIVE },                    // Explosive
            {"H210",Hazard.NONE },                         // Very sensitive
            {"H211",Hazard.NONE },                         // May be sensitive
            {"H220",Hazard.FLAMMABLE },                    // Extremely flammable gas
            {"H221",Hazard.EXPLOSIVE },                    // Flammable gas
            {"H222",Hazard.FLAMMABLE },                    // Extremely flammable material
            {"H223",Hazard.FLAMMABLE },                    // Flammable material
            {"H224",Hazard.FLAMMABLE },                    // Extremely flammable liquid and vapour
            {"H225",Hazard.FLAMMABLE | Hazard.FUMEHOOD },                    // Highly flammable liquid and vapour
            {"H226",Hazard.FLAMMABLE },                    // Flammable liquid and vapour
            {"H227",Hazard.FLAMMABLE },                    // Combustible liquid
            {"H228",Hazard.FLAMMABLE },                    // Flammable solid
            {"H229",Hazard.NONE },                         // Pressurized container: may burst if heated
            {"H230",Hazard.EXPLOSIVE },                    // May react explosively even in the absence of air
            {"H231",Hazard.EXPLOSIVE },                    // May react explosively even in the absence of air at elevated pressure and/or temperature
            {"H240",Hazard.EXPLOSIVE },                    // Heating may cause an explosion
            {"H241",Hazard.EXPLOSIVE | Hazard.FLAMMABLE }, // Heating may cause a fire or explosion
            {"H242",Hazard.FLAMMABLE },                                                // Heating may cause a fire
            {"H250",Hazard.FLAMMABLE | Hazard.NOT_EXPOSE_TO_AIR },                     // Catches fire spontaneously if exposed to air
            {"H251",Hazard.FLAMMABLE },                                                // Self-heating: may catch fire
            {"H252",Hazard.FLAMMABLE },                                                // Self-heating in large quantities: may catch fire
            {"H260",Hazard.FLAMMABLE | Hazard.NOT_NEAR_WATER | Hazard.GAS_RELEASE },   // In contact with water releases flammable gases which may ignite spontaneously
            {"H261",Hazard.FLAMMABLE | Hazard.NOT_NEAR_WATER | Hazard.GAS_RELEASE },   // In contact with water releases flammable gas
            {"H270",Hazard.FLAMMABLE },                                                // May cause or intensify fire: oxidizer
            {"H271",Hazard.EXPLOSIVE | Hazard.FLAMMABLE },                             // May cause fire or explosion: strong oxidizer
            {"H272",Hazard.FLAMMABLE },                                                // May intensify fire: oxidizer
            {"H280",Hazard.GAS_UNDER_PRESSURE | Hazard.EXPLOSIVE },                    // Contains gas under pressure; may explode if heated
            {"H281",Hazard.GAS_UNDER_PRESSURE },                                       // Contains refrigerated gas; may cause cryogenic burns or injury
            {"H282",Hazard.GAS_UNDER_PRESSURE | Hazard.EXPLOSIVE | Hazard.FLAMMABLE }, // Extremely flammable chemical under pressure: May explode if heated
            {"H283",Hazard.GAS_UNDER_PRESSURE | Hazard.EXPLOSIVE | Hazard.FLAMMABLE }, // Flammable chemical under pressure: May explode if heated
            {"H284",Hazard.GAS_UNDER_PRESSURE | Hazard.EXPLOSIVE },                    // Chemical under pressure: May explode if heated
            {"H290",Hazard.CONSULT_SPILL },                                            // May be corrosive to metals
            {"H300",Hazard.INGESTION },                                                // Fatal if swallowed
            
            {"H300+H310"       , Hazard.SKIN | Hazard.INGESTION},                      // Fatal if swallowed or in contact with skin
            {"H300+H310+H330"  , Hazard.SKIN | Hazard.INGESTION | Hazard.INHALATION},  // Fatal if swallowed, in contact with skin or if inhaled
            {"H300+H330"       , Hazard.INGESTION | Hazard.INHALATION},                // Fatal if swallowed or if inhaled
            {"H301"            , Hazard.INGESTION},                                    // Toxic if swallowed
            {"H301+H311"       , Hazard.SKIN | Hazard.INGESTION},                      // Toxic if swallowed or in contact with skin
            {"H301+H311+H331"  , Hazard.SKIN | Hazard.INGESTION | Hazard.INHALATION},  // Toxic if swallowed, in contact with skin or if inhaled
            {"H301+H331"       , Hazard.INGESTION | Hazard.INHALATION},                // Toxic if swallowed or if inhaled
            {"H302"            , Hazard.INGESTION},                                    // Harmful if swallowed
            {"H302+H312"       , Hazard.SKIN | Hazard.INGESTION},                      // Harmful if swallowed or in contact with skin
            {"H302+H312+H332"  , Hazard.SKIN | Hazard.INGESTION | Hazard.INHALATION},  // Harmful if swallowed, in contact with skin or if inhaled
            {"H302+H332"       , Hazard.INGESTION | Hazard.INHALATION},                // Harmful if swallowed or inhaled
            {"H303"            , Hazard.INGESTION},                                    // May be harmful if swallowed
            {"H303+H313"       , Hazard.SKIN | Hazard.INGESTION},                      // May be harmful if swallowed or in contact with skin
            {"H303+H313+H333"  , Hazard.SKIN | Hazard.INGESTION | Hazard.INHALATION},  // May be harmful if swallowed, in contact with skin or if inhaled
            {"H303+H333"       , Hazard.INGESTION | Hazard.INHALATION},                // May be harmful if swallowed or if inhaled
            {"H304"            , Hazard.INGESTION | Hazard.INHALATION},                // May be fatal if swallowed and enters airways
            {"H305"            , Hazard.INGESTION | Hazard.INHALATION},                // May be harmful if swallowed and enters airways
            {"H310"            , Hazard.SKIN},                                         // Fatal in contact with skin
            {"H310+H330"       , Hazard.SKIN | Hazard.INHALATION},                     // Fatal in contact with skin or if inhaled
            {"H311"            , Hazard.SKIN},                                         // Toxic in contact with skin
            {"H311+H331"       , Hazard.SKIN | Hazard.INHALATION},                     // Toxic in contact with skin or if inhaled
            {"H312"            , Hazard.SKIN },                    // Harmful in contact with skin
            {"H312+H332"       , Hazard.SKIN | Hazard.INHALATION}, // Harmful in contact with skin or if inhaled
            {"H313"            , Hazard.SKIN},                     // May be harmful in contact with skin
            {"H313+H333"       , Hazard.SKIN | Hazard.INHALATION}, // May be harmful in contact with skin or if inhaled
            {"H314"            , Hazard.SKIN | Hazard.EYES},       // Causes severe skin burns and eye damage
            {"H315"            , Hazard.SKIN},                     // Causes skin irritation
            {"H315+H320"       , Hazard.SKIN | Hazard.EYES},       // Causes skin and eye irritation
            {"H316"            , Hazard.SKIN},                     // Causes mild skin irritation
            {"H317"            , Hazard.SKIN},                     // May cause an allergic skin reaction
            {"H318"            , Hazard.EYES},                     // Causes serious eye damage
            {"H319"            , Hazard.EYES},                     // Causes serious eye irritation
            {"H320"            , Hazard.EYES},                     // Causes eye irritation
            {"H330"            , Hazard.INHALATION},               // Fatal if inhaled
            {"H331"            , Hazard.INHALATION},               // Toxic if inhaled
            {"H332"            , Hazard.INHALATION},               // Harmful if inhaled
            {"H333"            , Hazard.INHALATION},               // May be harmful if inhaled
            {"H334"            , Hazard.INHALATION},               // May cause allergy or asthma symptoms or breathing difficulties if inhaled
            {"H335"            , Hazard.INHALATION},               // May cause respiratory irritation
            {"H336"            , Hazard.INHALATION},               // May cause drowsiness or dizziness //UNSURE
            {"H340"            , Hazard.NONE},                     // May cause genetic defects
            {"H341"            , Hazard.NONE},                     // Suspected of causing genetic defects
            {"H350"            , Hazard.NONE},                     // May cause cancer
            {"H350i"           , Hazard.INHALATION},               // May cause cancer by inhalation
            {"H351"            , Hazard.NONE},                     // Suspected of causing cancer
            {"H360"            , Hazard.NOT_USE_WHEN_PREGNANT},    // May damage fertility or the unborn child
            {"H360D"           , Hazard.NOT_USE_WHEN_PREGNANT},    // May damage the unborn child
            {"H360Df"          , Hazard.NOT_USE_WHEN_PREGNANT},    // May damage the unborn child. Suspected of damaging fertility.
            {"H360F"           , Hazard.NONE},                     // May damage fertility
            {"H360FD"          , Hazard.NOT_USE_WHEN_PREGNANT},    // May damage fertility. May damage the unborn child.
            {"H360Fd"          , Hazard.NOT_USE_WHEN_PREGNANT},    // May damage fertility. Suspected of damaging the unborn child.
            {"H361"            , Hazard.NOT_USE_WHEN_PREGNANT},    // Suspected of damaging fertility or the unborn child
            {"H361d"           , Hazard.NOT_USE_WHEN_PREGNANT},    // Suspected of damaging the unborn child
            {"H361f"           , Hazard.NONE},                     // Suspected of damaging fertility
            {"H361fd"          , Hazard.NOT_USE_WHEN_PREGNANT },   // Suspected of damaging fertility. Suspected of damaging the unborn child.
            {"H362"            , Hazard.NONE },                    // May cause harm to breast-fed children
            {"H370"            , Hazard.NONE },                    // Causes damage to organs
            {"H371"            , Hazard.NONE },                    // May cause damage to organs
            {"H372"            , Hazard.NONE },                    // Causes damage to organs through prolonged or repeated exposure
            {"H373"            , Hazard.NONE },                    // May cause damage to organs through prolonged or repeated exposure
            {"H400"            , Hazard.NONE },                    // Very toxic to aquatic life
            {"H401"            , Hazard.NONE },                    // Toxic to aquatic life
            {"H402"            , Hazard.NONE },                    // Harmful to aquatic life
            {"H410"            , Hazard.NONE },                    // Very toxic to aquatic life with long lasting effects
            {"H411"            , Hazard.NONE },                    // Toxic to aquatic life with long lasting effects
            {"H412"            , Hazard.NONE },                    // Harmful to aquatic life with long lasting effects
            {"H413"            , Hazard.NONE },                    // May cause long lasting harmful effects to aquatic life
            {"H420"            , Hazard.NONE },                    // Harms public health and the environment by destroying ozone in the upper atmosphere
            {"H441"            , Hazard.NONE },                    // Very toxic to terrestrial invertebrates
            {"EUH006"          , Hazard.EXPLOSIVE | Hazard.NOT_EXPOSE_TO_AIR }, // Explosive with or without contact with air, deleted in the fourth adaptation to technical progress of CLP.
            {"EUH014"          , Hazard.NOT_NEAR_WATER },          // Reacts violently with water
            {"EUH018"          , Hazard.FLAMMABLE | Hazard.EXPLOSIVE | Hazard.GAS_RELEASE }, // In use may form flammable/explosive vapour-air mixture
            {"EUH019"          , Hazard.EXPLOSIVE },               // May form explosive peroxides
            {"EUH044"          , Hazard.EXPLOSIVE },               // Risk of explosion if heated under confinement
            {"EUH029"          , Hazard.INHALATION | Hazard.NOT_NEAR_WATER }, // Contact with water liberates toxic gas
            {"EUH031"          , Hazard.INHALATION },              // Contact with acids liberates toxic gas
            {"EUH032"          , Hazard.INHALATION },              // Contact with acids liberates very toxic gas
            {"EUH066"          , Hazard.SKIN },                    // Repeated exposure may cause skin dryness or cracking
            {"EUH070"          , Hazard.EYES },                    // Toxic by eye contact
            {"EUH071"          , Hazard.INHALATION },              // Corrosive to the respiratory tract
};

        private Dictionary<string, Hazard> PrecautionaryCodeMap = new Dictionary<string, Hazard>
        {
            {"P201"          , Hazard.NONE }, // Obtain special instructions before use.
            {"P202"          , Hazard.NONE }, // Do not handle until all safety precautions have been read and understood.
            {"P210"          , Hazard.FLAMMABLE }, // Keep away from heat, hot surfaces, sparks, open flames and other ignition sources. No smoking.
            {"P211"          , Hazard.FLAMMABLE }, // Do not spray on an open flame or other ignition source.
            {"P220"          , Hazard.FLAMMABLE }, // Keep/Store away from clothing/.../combustible materials.
            {"P221"          , Hazard.FLAMMABLE }, // Take any precaution to avoid mixing with combustibles.
            {"P222"          , Hazard.NOT_EXPOSE_TO_AIR }, // Do not allow contact with air.
            {"P223"          , Hazard.NOT_NEAR_WATER }, // Do not allow contact with water.
            {"P230"          , Hazard.NOT_EXPOSE_TO_AIR }, // Keep wetted with ...
            {"P231"          , Hazard.NOT_EXPOSE_TO_AIR }, // Handle and store contents under inert gas/...
            {"P231+P232"     , Hazard.NOT_EXPOSE_TO_AIR }, // Handle and store contents under inert gas. Protect from moisture
            {"P232"          , Hazard.NOT_NEAR_WATER }, // Protect from moisture.
            {"P233"          , Hazard.NOT_EXPOSE_TO_AIR }, // Keep container tightly closed.
            {"P234"          , Hazard.NONE }, // Keep only in original container/packaging.
            {"P235"          , Hazard.NONE }, // Keep cool.
            {"P235+P410"     , Hazard.NONE }, // Keep cool. Protect from sunlight.
            {"P240"          , Hazard.NONE }, // Ground and bond container and receiving equipment.
            {"P241"          , Hazard.EXPLOSIVE }, // Use explosion-proof electrical/ventilating/light/.../equipment.
            {"P242"          , Hazard.NONE }, // Use only non-sparking tools.
            {"P243"          , Hazard.NONE }, // Take action to prevent static discharges.
            {"P244"          , Hazard.NONE }, // Keep valves and fittings free from grease and oil
            {"P250"          , Hazard.NONE }, // Do not subject to grinding/shock/.../friction.
            {"P251"          , Hazard.NONE }, // Pressurized container – Do not pierce or burn, even after use.
            {"P260"          , Hazard.INHALATION }, // Do not breathe dust/fume/gas/mist/vapours/spray.
            {"P261"          , Hazard.INHALATION }, // Avoid breathing dust/fume/gas/mist/vapours/spray.
            {"P262"          , Hazard.SKIN | Hazard.EYES }, // Do not get in eyes, on skin, or on clothing.
            {"P263"          , Hazard.NOT_USE_WHEN_PREGNANT }, // Avoid contact during pregnancy and while nursing.
            {"P264"          , Hazard.SKIN }, // Wash ... thoroughly after handling.
            {"P270"          , Hazard.INGESTION }, // Do not eat, drink or smoke when using this product.
            {"P271"          , Hazard.NONE }, // Use only outdoors or in a well-ventilated area.
            {"P272"          , Hazard.NONE }, // Contaminated work clothing should not be allowed out of the workplace.
            {"P273"          , Hazard.NONE }, // Avoid release to the environment.
            {"P280"          , Hazard.EYES | Hazard.SKIN | Hazard.LAB_COAT }, // Wear protective gloves/protective clothing/eye protection/face protection.
            {"P281"          , Hazard.EYES | Hazard.SKIN | Hazard.LAB_COAT }, // Use personal protective equipment as required.
            {"P282"          , Hazard.EYES | Hazard.SKIN | Hazard.LAB_COAT }, // Wear cold insulating gloves and either face shield or eye protection.
            {"P283"          , Hazard.FLAMMABLE },// Wear fire resistant or flame retardant clothing.
            {"P284"          , Hazard.INHALATION },// Wear respiratory protection.
            {"P285"          , Hazard.INHALATION }, // In case of inadequate ventilation wear respiratory protection.
            {"P301"          , Hazard.INGESTION}, // IF SWALLOWED:
            {"P301+P310"     , Hazard.INGESTION}, // IF SWALLOWED: Immediately call a POISON CENTER or doctor/physician.
            {"P301+P312"     , Hazard.INGESTION}, // IF SWALLOWED: Call a POISON CENTER or doctor/physician if you feel unwell.
            {"P301+P330+P331", Hazard.INGESTION}, // IF SWALLOWED: Rinse mouth. Do NOT induce vomiting.
            {"P302"          , Hazard.SKIN}, // IF ON SKIN:
            {"P302+P334"     , Hazard.SKIN}, // IF ON SKIN: Immerse in cool water or wrap in wet bandages.
            {"P302+P350"     , Hazard.SKIN}, // IF ON SKIN: Gently wash with soap and water.
            {"P302+P352"     , Hazard.SKIN}, // IF ON SKIN: Wash with soap and water.
            {"P303"          , Hazard.SKIN}, // IF ON SKIN (or hair):
            {"P303+P361+P353", Hazard.SKIN}, // IF ON SKIN (or hair): Remove/Take off immediately all contaminated clothing. Rinse skin with water [or shower].
            {"P304"          , Hazard.INHALATION}, // IF INHALED:
            {"P304+P312"     , Hazard.INHALATION}, // IF INHALED: Call a POISON CENTER or doctor/physician if you feel unwell.
            {"P304+P340"     , Hazard.INHALATION}, // IF INHALED: Remove victim to fresh air and keep at rest in a position comfortable for breathing.
            {"P304+P341"     , Hazard.INHALATION}, // IF INHALED: If breathing is difficult, remove victim to fresh air and keep at rest in a position comfortable for breathing.
            {"P305"          , Hazard.EYES}, // IF IN EYES:
            {"P305+P351+P338", Hazard.EYES}, // IF IN EYES: Rinse continuously with water for several minutes. Remove contact lenses if present and easy to do. Continue rinsing.
            {"P306"          , Hazard.SKIN}, // IF ON CLOTHING:
            {"P306+P360"     , Hazard.SKIN}, // IF ON CLOTHING: Rinse immediately contaminated clothing and skin with plenty of water before removing clothes.
            {"P307"          , Hazard.NONE}, // IF exposed:
            {"P307+P311"     , Hazard.NONE}, // IF exposed: Call a POISON CENTER or doctor/physician.
            {"P308"          , Hazard.NONE}, // IF exposed or concerned:
            {"P308+P311"     , Hazard.NONE}, // IF exposed or concerned: Call a POISON CENTER or doctor/physician.
            {"P308+P313"     , Hazard.SKIN|Hazard.EYES|Hazard.INHALATION|Hazard.INGESTION}, // IF exposed or concerned: Get medical advice/attention.
            {"P309"          , Hazard.NONE}, // IF exposed or you feel unwell:
            {"P309+P311"     , Hazard.NONE}, // IF exposed or you feel unwell: Call a POISON CENTER or doctor/physician.
            {"P310"          , Hazard.NONE}, // Immediately call a POISON CENTER or doctor/physician.
            {"P311"          , Hazard.NONE}, // Call a POISON CENTER or doctor/physician.
            {"P312"          , Hazard.INGESTION}, // Call a POISON CENTER or doctor/physician if you feel unwell.
            {"P313"          , Hazard.NONE}, // Get medical advice/attention.
            {"P314"          , Hazard.NONE}, // Get Medical advice/attention if you feel unwell.
            {"P315"          , Hazard.NONE}, // Get immediate medical advice/attention.
            {"P320"          , Hazard.NONE}, // Specific treatment is urgent (see ... on this label).
            {"P321"          , Hazard.NONE}, // Specific treatment (see ... on this label).
            {"P322"          , Hazard.NONE}, // Specific measures (see ... on this label).
            {"P330"          , Hazard.INGESTION}, // Rinse mouth.
            {"P331"          , Hazard.NONE}, // Do NOT induce vomiting.
            {"P332"          , Hazard.SKIN}, // If skin irritation occurs:
            {"P332+P313"     , Hazard.SKIN}, // If skin irritation occurs: Get medical advice/attention.
            {"P333"          , Hazard.SKIN}, // If skin irritation or a rash occurs:
            {"P333+P313"     , Hazard.SKIN}, // If skin irritation or a rash occurs: Get medical advice/attention.
            {"P334"          , Hazard.SKIN}, // Immerse in cool water [or wrap in wet bandages].
            {"P335"          , Hazard.SKIN}, // Brush off loose particles from skin.
            {"P335+P334"     , Hazard.SKIN}, // Brush off loose particles from skin. Immerse in cool water/wrap in wet bandages.
            {"P336"          , Hazard.SKIN }, // Thaw frosted parts with lukewarm water. Do not rub affected areas.
            {"P337"          , Hazard.EYES}, // If eye irritation persists:
            {"P337+P313"     , Hazard.EYES}, // If eye irritation persists: Get medical advice/attention.
            {"P338"          , Hazard.EYES}, // Remove contact lenses if present and easy to do. Continue rinsing.
            {"P340"          , Hazard.INHALATION}, // Remove victim to fresh air and keep at rest in a position comfortable for breathing.
            {"P341"          , Hazard.INHALATION}, // If breathing is difficult, remove victim to fresh air and keep at rest in a position comfortable for breathing.
            {"P342"          , Hazard.INHALATION}, // If experiencing respiratory symptoms:
            {"P342+P311"     , Hazard.INHALATION}, // If experiencing respiratory symptoms: Call a POISON CENTER or doctor/physician.
            {"P350"          , Hazard.SKIN}, // Gently wash with soap and water.
            {"P351"          , Hazard.EYES}, // Rinse cautiously with water for several minutes.
            {"P352"          , Hazard.SKIN}, // Wash with plenty of water.
            {"P353"          , Hazard.SKIN}, // Rinse skin with water [or shower].
            {"P360"          , Hazard.EYES}, // Rinse immediately contaminated clothing and skin with plenty of water before removing clothes.
            {"P361"          , Hazard.SKIN}, // Remove/Take off immediately all contaminated clothing.
            {"P361+P364"     , Hazard.SKIN}, // Take off immediately all contaminated clothing and wash it before reuse.
            {"P362"          , Hazard.SKIN}, // Take off contaminated clothing.
            {"P362+P364"     , Hazard.SKIN}, // Take off contaminated clothing and wash it before reuse.
            {"P363"          , Hazard.NONE}, // Wash contaminated clothing before reuse.
            {"P364"          , Hazard.NONE}, // And wash it before reuse.
            {"P370"          , Hazard.FLAMMABLE}, // In case of fire:
            {"P370+P376"     , Hazard.FLAMMABLE}, // In case of fire: Stop leak if safe to do so.
            {"P370+P378"     , Hazard.FLAMMABLE }, // In case of fire: Use ... to extinguish.
            {"P370+P380"     , Hazard.FLAMMABLE}, // In case of fire: Evacuate area.
            {"P370+P380+P375", Hazard.EXPLOSIVE | Hazard.FLAMMABLE}, // In case of fire: Evacuate area. Fight fire remotely due to the risk of explosion.
            {"P371"          , Hazard.FLAMMABLE}, // In case of major fire and large quantities:
            {"P371+P380+P375", Hazard.EXPLOSIVE}, // In case of major fire and large quantities: Evacuate area. Fight fire remotely due to the risk of explosion.
            {"P372"          , Hazard.EXPLOSIVE}, // Explosion risk.
            {"P373"          , Hazard.FLAMMABLE}, // DO NOT fight fire when fire reaches explosives.
            {"P374"          , Hazard.FLAMMABLE}, // Fight fire with normal precautions from a reasonable distance.
            {"P375"          , Hazard.EXPLOSIVE}, // Fight fire remotely due to the risk of explosion.
            {"P376"          , Hazard.NONE}, // Stop leak if safe to do so.
            {"P377"          , Hazard.GAS_RELEASE}, // Leaking gas fire – do not extinguish unless leak can be stopped safely.
            {"P378"          , Hazard.FLAMMABLE}, // Use ... to extinguish.
            {"P380"          , Hazard.NONE}, // Evacuate area.
            {"P381"          , Hazard.NONE}, // In case of leakage, eliminate all ignition sources.
            {"P391"          , Hazard.CONSULT_SPILL} // Collect spillage.
            // Storage precautionary statements not added
};

        public Dictionary<string, string> HazardCodeDict = new Dictionary<string, string>
        {
            { "H200", "Unstable explosive" },
            { "H201", "Explosive: mass explosion hazard" },
            { "H202", "Explosive: severe projection hazard" },
            { "H203", "Explosive: fire, blast or projection hazard" },
            { "H204", "Fire or projection hazard" },
            { "H205", "May mass explode in fire" },
            { "H206", "Fire, blast or projection hazard: increased risk of explosion if desensitizing agent is reduced" },
            { "H207", "Fire or projection hazard; increased risk of explosion if desensitizing agent is reduced" },
            { "H208", "Fire hazard; increased risk of explosion if desensitizing agent is reduced" },
            { "H209", "Explosive" },
            { "H210", "Very sensitive" },
            { "H211", "May be sensitive" },
            { "H220", "Extremely flammable gas" },
            { "H221", "Flammable gas" },
            { "H222", "Extremely flammable material" },
            { "H223", "Flammable material" },
            { "H224", "Extremely flammable liquid and vapour" },
            { "H225", "Highly flammable liquid and vapour" },
            { "H226", "Flammable liquid and vapour" },
            { "H227", "Combustible liquid" },
            { "H228", "Flammable solid" },
            { "H229", "Pressurized container: may burst if heated" },
            { "H230", "May react explosively even in the absence of air" },
            { "H231", "May react explosively even in the absence of air at elevated pressure and/or temperature" },
            { "H240", "Heating may cause an explosion" },
            { "H241", "Heating may cause a fire or explosion" },
            { "H242", "Heating may cause a fire" },
            { "H250", "Catches fire spontaneously if exposed to air" },
            { "H251", "Self-heating: may catch fire" },
            { "H252", "Self-heating in large quantities: may catch fire" },
            { "H260", "In contact with water releases flammable gases which may ignite spontaneously" },
            { "H261", "In contact with water releases flammable gas" },
            { "H270", "May cause or intensify fire: oxidizer" },
            { "H271", "May cause fire or explosion: strong oxidizer" },
            { "H272", "May intensify fire: oxidizer" },
            { "H280", "Contains gas under pressure: may explode if heated" },
            { "H281", "Contains refrigerated gas: may cause cryogenic burns or injury" },
            { "H282", "Extremely flammable chemical under pressure: May explode if heated" },
            { "H283", "Flammable chemical under pressure: May explode if heated" },
            { "H284", "Chemical under pressure: May explode if heated" },
            { "H290", "May be corrosive to metals" },
            { "H300", "Fatal if swallowed" },
            { "H300+H310", "Fatal if swallowed or in contact with skin" },
            { "H300+H310+H330", "Fatal if swallowed, in contact with skin or if inhaled" },
            { "H300+H330", "Fatal if swallowed or if inhaled" },
            { "H301", "Toxic if swallowed" },
            { "H301+H311", "Toxic if swallowed or in contact with skin" },
            { "H301+H311+H331", "Toxic if swallowed, in contact with skin or if inhaled" },
            { "H301+H331", "Toxic if swallowed or if inhaled" },
            { "H302", "Harmful if swallowed" },
            { "H302+H312", "Harmful if swallowed or in contact with skin" },
            { "H302+H312+H332", "Harmful if swallowed, in contact with skin or if inhaled" },
            { "H302+H332", "Harmful if swallowed or inhaled" },
            { "H303", "May be harmful if swallowed" },
            { "H303+H313", "May be harmful if swallowed or in contact with skin" },
            { "H303+H313+H333", "May be harmful if swallowed, in contact with skin or if inhaled" },
            { "H303+H333", "May be harmful if swallowed or if inhaled" },
            { "H304", "May be fatal if swallowed and enters airways" },
            { "H305", "May be harmful if swallowed and enters airways" },
            { "H310", "Fatal in contact with skin" },
            { "H310+H330", "Fatal in contact with skin or if inhaled" },
            { "H311", "Toxic in contact with skin" },
            { "H311+H331", "Toxic in contact with skin or if inhaled" },
            { "H312", "Harmful in contact with skin" },
            { "H312+H332", "Harmful in contact with skin or if inhaled" },
            { "H313", "May be harmful in contact with skin" },
            { "H313+H333", "May be harmful in contact with skin or if inhaled" },
            { "H314", "Causes severe skin burns and eye damage" },
            { "H315", "Causes skin irritation" },
            { "H315+H320", "Causes skin and eye irritation" },
            { "H316", "Causes mild skin irritation" },
            { "H317", "May cause an allergic skin reaction" },
            { "H318", "Causes serious eye damage" },
            { "H319", "Causes serious eye irritation" },
            { "H320", "Causes eye irritation" },
            { "H330", "Fatal if inhaled" },
            { "H331", "Toxic if inhaled" },
            { "H332", "Harmful if inhaled" },
            { "H333", "May be harmful if inhaled" },
            { "H334", "May cause allergy or asthma symptoms or breathing difficulties if inhaled" },
            { "H335", "May cause respiratory irritation" },
            { "H336", "May cause drowsiness or dizziness" },
            { "H340", "May cause genetic defects" },
            { "H341", "Suspected of causing genetic defects" },
            { "H350", "May cause cancer" },
            { "H350i", "May cause cancer by inhalation" },
            { "H351", "Suspected of causing cancer" },
            { "H360", "May damage fertility or the unborn child" },
            { "H360D", "May damage the unborn child" },
            { "H360Df", "May damage the unborn child. Suspected of damaging fertility." },
            { "H360F", "May damage fertility" },
            { "H360FD", "May damage fertility. May damage the unborn child." },
            { "H360Fd", "May damage fertility. Suspected of damaging the unborn child." },
            { "H361", "Suspected of damaging fertility or the unborn child" },
            { "H361d", "Suspected of damaging the unborn child" },
            { "H361f", "Suspected of damaging fertility" },
            { "H361fd", "Suspected of damaging fertility. Suspected of damaging the unborn child." },
            { "H362", "May cause harm to breast-fed children" },
            { "H370", "Causes damage to organs" },
            { "H371", "May cause damage to organs" },
            { "H372", "Causes damage to organs through prolonged or repeated exposure" },
            { "H373", "May cause damage to organs through prolonged or repeated exposure" },
            { "H400", "Very toxic to aquatic life" },
            { "H401", "Toxic to aquatic life" },
            { "H402", "Harmful to aquatic life" },
            { "H410", "Very toxic to aquatic life with long lasting effects" },
            { "H411", "Toxic to aquatic life with long lasting effects" },
            { "H412", "Harmful to aquatic life with long lasting effects" },
            { "H413", "May cause long lasting harmful effects to aquatic life" },
            { "H420", "Harms public health and the environment by destroying ozone in the upper atmosphere" },
            { "H441", "Very toxic to terrestrial invertebrates" },
            { "EUH006", "Explosive with or without contact with air, deleted in the fourth adaptation to technical progress of CLP." },
            { "EUH014", "Reacts violently with water" },
            { "EUH018", "In use may form flammable/explosive vapour-air mixture" },
            { "EUH019", "May form explosive peroxides" },
            { "EUH044", "Risk of explosion if heated under confinement" },
            { "EUH029", "Contact with water liberates toxic gas" },
            { "EUH031", "Contact with acids liberates toxic gas" },
            { "EUH032", "Contact with acids liberates very toxic gas" },
            { "EUH066", "Repeated exposure may cause skin dryness or cracking" },
            { "EUH070", "Toxic by eye contact" },
            { "EUH071", "Corrosive to the respiratory tract" },
            { "EUH380", "May cause endocrine disruption in humans" },
            { "EUH381", "Suspected of causing endocrine disruption in humans" },
            { "EUH059", "Hazardous to the ozone layer, superseded by GHS Class 5.1 in the second adaptation to technical progress of CLP." },
            { "EUH430", "May cause endocrine disruption in the environment" },
            { "EUH431", "Suspected of causing endocrine disruption in the environment" },
            { "EUH440", "Accumulates in the environment and living organisms including in humans" },
            { "EUH441", "Strongly accumulates in the environment and living organisms including in humans" },
            { "EUH450", "Can cause long-lasting and diffuse contamination of water resources" },
            { "EUH451", "Can cause very long-lasting and diffuse contamination of water resources" },
            { "EUH201", "Contains lead. Should not be used on surfaces liable to be chewed or sucked by children." },
            { "EUH201A", "Warning! Contains lead." },
            { "EUH202", "Cyanoacrylate.Danger.Bonds skin and eyes in seconds. Keep out of the reach of children." },
            { "EUH203", "Contains chromium(VI). May produce an allergic reaction." },
            { "EUH204", "Contains isocyanates. May produce an allergic reaction." },
            { "EUH205", "Contains epoxy constituents. May produce an allergic reaction." },
            { "EUH206", "Warning! Do not use together with other products. May release dangerous gases (chlorine)." },
            { "EUH207", "Warning! Contains cadmium. Dangerous fumes are formed during use. See information supplied by the manufacturer. Comply with the safety instructions." },
            { "EUH208", "Contains < name of sensitising substance>. May produce an allergic reaction." },
            { "EUH209", "Can become highly flammable in use." },
            { "EUH209A", "Can become flammable in use." },
            { "EUH210", "Safety data sheet available on request." },
            { "EUH211", "Warning! Hazardous respirable droplets may be formed when sprayed. Do not breathe spray or mist." },
            { "EUH401", "To avoid risks to human health and the environment, comply with the instructions for use." }
        };

        public Dictionary<string, string> PrecautionaryCodeDict = new Dictionary<string, string>
        {
            {"P101", "If medical advice is needed, have product container or label at hand."},
            {"P102", "Keep out of reach of children."},
            {"P103", "Read label before use."},
            {"P201", "Obtain special instructions before use."},
            {"P202", "Do not handle until all safety precautions have been read and understood."},
            {"P210", "Keep away from heat, hot surfaces, sparks, open flames and other ignition sources. No smoking."},
            {"P211", "Do not spray on an open flame or other ignition source."},
            {"P220", "Keep/Store away from clothing/.../combustible materials."},
            {"P221", "Take any precaution to avoid mixing with combustibles."},
            {"P222", "Do not allow contact with air."},
            {"P223", "Do not allow contact with water."},
            {"P230", "Keep wetted with ..."},
            {"P231", "Handle and store contents under inert gas/..."},
            {"P231+P232", "Handle and store contents under inert gas. Protect from moisture"},
            {"P232", "Protect from moisture."},
            {"P233", "Keep container tightly closed."},
            {"P234", "Keep only in original container/packaging."},
            {"P235", "Keep cool."},
            {"P235+P410", "Keep cool. Protect from sunlight."},
            {"P240", "Ground and bond container and receiving equipment."},
            {"P241", "Use explosion-proof electrical/ventilating/light/.../equipment."},
            {"P242", "Use only non-sparking tools."},
            {"P243", "Take precautionary measures to prevent static discharges."},
            {"P244", "Keep valves and fittings free from grease and oil"},
            {"P250", "Do not subject to grinding/shock/.../friction."},
            {"P251", "Pressurized container – Do not pierce or burn, even after use."},
            {"P260", "Do not breathe dust/fume/gas/mist/vapours/spray."},
            {"P261", "Avoid breathing dust/fume/gas/mist/vapours/spray."},
            {"P262", "Do not get in eyes, on skin, or on clothing."},
            {"P263", "Avoid contact during pregnancy and while nursing."},
            {"P264", "Wash...thoroughly after handling."},
            {"P270", "Do not eat, drink or smoke when using this product."},
            {"P271", "Use only outdoors or in a well-ventilated area."},
            {"P272", "Contaminated work clothing should not be allowed out of the workplace."},
            {"P273", "Avoid release to the environment."},
            {"P280", "Wear protective gloves/protective clothing/eye protection/face protection."},
            {"P281", "Use personal protective equipment as required."},
            {"P282", "Wear cold insulating gloves and either face shield or eye protection."},
            {"P283", "Wear fire resistant or flame retardant clothing."},
            {"P284", "Wear respiratory protection."},
            {"P285", "In case of inadequate ventilation wear respiratory protection."},
            {"P301", "IF SWALLOWED:"},
            {"P301+P310", "IF SWALLOWED: Immediately call a POISON CENTER or doctor/physician."},
            {"P301+P312", "IF SWALLOWED: Call a POISON CENTER or doctor/physician if you feel unwell."},
            {"P301+P330+P331", "IF SWALLOWED: Rinse mouth. Do NOT induce vomiting."},
            {"P302", "IF ON SKIN:"},
            {"P302+P334", "IF ON SKIN: Immerse in cool water or wrap in wet bandages."},
            {"P302+P350", "IF ON SKIN: Gently wash with soap and water."},
            {"P302+P352", "IF ON SKIN: Wash with soap and water."},
            {"P303", "IF ON SKIN (or hair):"},
            {"P303+P361+P353", "IF ON SKIN (or hair): Remove/Take off immediately all contaminated clothing. Rinse skin with water [or shower]."},
            {"P304", "IF INHALED:"},
            {"P304+P312", "IF INHALED: Call a POISON CENTER or doctor/physician if you feel unwell."},
            {"P304+P340", "IF INHALED: Remove victim to fresh air and keep at rest in a position comfortable for breathing."},
            {"P304+P341", "IF INHALED: If breathing is difficult, remove victim to fresh air and keep at rest in a position comfortable for breathing."},
            {"P305", "IF IN EYES:"},
            {"P305+P351+P338", "IF IN EYES: Rinse continuously with water for several minutes. Remove contact lenses if present and easy to do. Continue rinsing."},
            {"P306", "IF ON CLOTHING:"},
            {"P306+P360", "IF ON CLOTHING: Rinse immediately contaminated clothing and skin with plenty of water before removing clothes."},
            {"P307", "IF exposed:"},
            {"P307+P311", "IF exposed: Call a POISON CENTER or doctor/physician."},
            {"P308", "IF exposed or concerned:"},
            {"P308+P311", "IF exposed or concerned: Call a POISON CENTER or doctor/physician."},
            {"P308+P313", "IF exposed or concerned: Get medical advice/attention."},
            {"P309", "IF exposed or you feel unwell:"},
            {"P309+P311", "IF exposed or you feel unwell: Call a POISON CENTER or doctor/physician."},
            {"P310", "Immediately call a POISON CENTER or doctor/physician."},
            {"P311", "Call a POISON CENTER or doctor/physician."},
            {"P312", "Call a POISON CENTER or doctor/physician if you feel unwell."},
            {"P313", "Get medical advice/attention."},
            {"P314", "Get Medical advice/attention if you feel unwell."},
            {"P315", "Get immediate medical advice/attention."},
            {"P320", "Specific treatment is urgent (see ... on this label)."},
            {"P321", "Specific treatment (see ... on this label)."},
            {"P322", "Specific measures (see ... on this label)."},
            {"P330", "Rinse mouth."},
            {"P331", "Do NOT induce vomiting."},
            {"P332", "If skin irritation occurs:"},
            {"P332+P313", "If skin irritation occurs: Get medical advice/attention."},
            {"P333", "If skin irritation or a rash occurs:"},
            {"P333+P313", "If skin irritation or a rash occurs: Get medical advice/attention."},
            {"P334", "Immerse in cool water [or wrap in wet bandages]."},
            {"P335", "Brush off loose particles from skin."},
            {"P335+P334", "Brush off loose particles from skin. Immerse in cool water/wrap in wet bandages."},
            {"P336", "Thaw frosted parts with lukewarm water. Do not rub affected areas."},
            {"P337", "If eye irritation persists:"},
            {"P337+P313", "If eye irritation persists: Get medical advice/attention."},
            {"P338", "Remove contact lenses if present and easy to do. Continue rinsing."},
            {"P340", "Remove victim to fresh air and keep at rest in a position comfortable for breathing."},
            {"P341", "If breathing is difficult, remove victim to fresh air and keep at rest in a position comfortable for breathing."},
            {"P342", "If experiencing respiratory symptoms:"},
            {"P342+P311", "If experiencing respiratory symptoms: Call a POISON CENTER or doctor/physician."},
            {"P350", "Gently wash with soap and water."},
            {"P351", "Rinse cautiously with water for several minutes."},
            {"P352", "Wash with plenty of water."},
            {"P353", "Rinse skin with water [or shower]."},
            {"P360", "Rinse immediately contaminated clothing and skin with plenty of water before removing clothes."},
            {"P361", "Remove/Take off immediately all contaminated clothing."},
            {"P361+P364", "Take off immediately all contaminated clothing and wash it before reuse."},
            {"P362", "Take off contaminated clothing."},
            {"P362+P364", "Take off contaminated clothing and wash it before reuse."},
            {"P363", "Wash contaminated clothing before reuse."},
            {"P364", "And wash it before reuse."},
            {"P370", "In case of fire:"},
            {"P370+P376", "In case of fire: Stop leak if safe to do so."},
            {"P370+P378", "In case of fire: Use ... to extinguish."},
            {"P370+P380", "In case of fire: Evacuate area."},
            {"P370+P380+P375", "In case of fire: Evacuate area. Fight fire remotely due to the risk of explosion."},
            {"P371", "In case of major fire and large quantities:"},
            {"P371+P380+P375", "In case of major fire and large quantities: Evacuate area. Fight fire remotely due to the risk of explosion."},
            {"P372", "Explosion risk."},
            {"P373", "DO NOT fight fire when fire reaches explosives."},
            {"P374", "Fight fire with normal precautions from a reasonable distance."},
            {"P375", "Fight fire remotely due to the risk of explosion."},
            {"P376", "Stop leak if safe to do so."},
            {"P377", "Leaking gas fire – do not extinguish unless leak can be stopped safely."},
            {"P378", "Use ... to extinguish."},
            {"P380", "Evacuate area."},
            {"P381", "In case of leakage, eliminate all ignition sources."},
            {"P391", "Collect spillage."},
            {"P401", "Store in accordance with ..."},
            {"P402", "Store in a dry place."},
            {"P402+P404", "Store in a dry place. Store in a closed container."},
            {"P403", "Store in a well ventilated place."},
            {"P403+P233", "Store in a well ventilated place. Keep container tightly closed."},
            {"P403+P235", "Store in a well ventilated place. Keep cool."},
            {"P404", "Store in a closed container."},
            {"P405", "Store locked up."},
            {"P406", "Store in a corrosive resistant/... container with a resistant inner liner."},
            {"P407", "Maintain air gap between stacks or pallets."},
            {"P410", "Protect from sunlight."},
            {"P410+P403", "Protect from sunlight. Store in a well ventilated place."},
            {"P410+P412", "Protect from sunlight. Do not expose to temperatures exceeding 50 oC/122 oF."},
            {"P411", "Store at temperatures not exceeding ... oC/... oF."},
            {"P411+P235", "Store at temperatures not exceeding ... oC/... oF. Keep cool."},
            {"P412", "Do not expose to temperatures exceeding 50 oC/122 oF."},
            {"P413", "Store bulk masses greater than ... kg/... lbs at temperatures not exceeding ... °C/... °F."},
            {"P420", "Store separately/away from other materials."},
            {"P422", "Store contents under ..."},
            {"P501", "Dispose of contents / container to..."},
            {"P502", "Refer to manufacturer or supplier for information on recovery or recycling."}
        };

    }
}

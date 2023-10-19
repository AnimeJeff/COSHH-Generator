

using COSHH_Generator.Scrapers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace COSHH_Generator
{
    public class Program
    {
        public static ConsoleColor CLIColour = ConsoleColor.Green;
        public static void ColourWrite(params object[] oo)
        {
            foreach (var o in oo)
                if (o == null)
                    Console.ResetColor();
                else if (o is ConsoleColor)
                    Console.ForegroundColor = (ConsoleColor)o;
                else
                    Console.Write(o.ToString());
        }
        public static void CLIInfo(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.ResetColor();
        }
        public static void CLIError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Error: " + text);
            Console.ResetColor();
        }
        public static void CLIWarn(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("Warning: " + text);
            Console.ResetColor();
        }

        public static void PrintUsage()
        {
            string usage =
                "usage: coshhgen.exe\n" +
                "or     coshhgen.exe [file path]\n" +
                "options: \n" +
                "   --file [file path], -f [file path]\n" +
                "   --search [substance to search], -s [substance to search]\n";
            CLIInfo(usage);
        }
        public static bool ReadFile(in string path)
        {

            return true;
        }

        // For CLI mode
        struct Config
        {
            public Config() { }
            public string Name { get; set; } = "Nick Green";
            public string Title { get; set; } = "Experiment";
            public string College { get; set; } = "Jesus College";
            public int Year { get; set; } = 1;

            private List<Substance> substances = new List<Substance>();
            private string outputName { get; set; } = "output.docx";
            private string outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

            public string OutputDir
            {
                get
                {
                    return outputDir;
                }
                set
                {
                    if (Directory.Exists(value))
                    {
                        outputDir = value;
                    }
                    else
                    {
                        CLIWarn("invalid directory\n");
                    }
                }
            }
            public string OutputName
            {
                get
                {
                    return outputName;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        outputName = value + ".docx";
                    }
                    else
                    {
                        CLIWarn("output name cannot be empty\n");
                    }
                }
            }

            public string OutputPath
            {
                get
                {
                    return Path.Combine(OutputDir, OutputName);
                }
            }

            public List<Substance> Substances
            {
                get { return substances; }
            }
            public WasteDisposal WasteDisposalFlags = 0b000000;
            [Flags]
            public enum WasteDisposal
            {
                HALOGENATED               = 0b000001,
                AQUEOUS                   = 0b000010,
                HYDROCARBON               = 0b000100,
                NAMED_WASTE               = 0b001000,
                CONTAMINATED_SOLID_WASTE  = 0b010000,
                SILICA_TLC                = 0b100000,
            }
            [Flags]
            public enum SpecificSafety
            {
                FIRE_EXPLOSION        = 0b0001,
                THERMAL_RUNAWAY       = 0b0010,
                GAS_RELEASE           = 0b0100,
                MALODOROUS_SUBSTANCES = 0b1000,
            }
            public SpecificSafety SpecificSafetyFlags = 0b0000;
            
            public void print()
            {   
                string config =
                $"Name:           {Name}\n" +
                $"Title:          {Title}\n" +
                $"College:        {College}\n" +
                $"Year:           {Year}\n" +
                $"Output Path:    {OutputPath}\n" +
                $"Waste Disposal: {Convert.ToString((int)WasteDisposalFlags, 2).PadLeft(6, '0')}\n" +
                $"\t1. Halogenated:              {WasteDisposalFlags.HasFlag(WasteDisposal.HALOGENATED)} \t 2. Aqueous:     {WasteDisposalFlags.HasFlag(WasteDisposal.AQUEOUS)}\n" +
                $"\t3. Hydrocarbon:              {WasteDisposalFlags.HasFlag(WasteDisposal.HYDROCARBON)} \t 4. Named Waste: {WasteDisposalFlags.HasFlag(WasteDisposal.NAMED_WASTE)}\n" +
                $"\t5. Contaminated solid waste: {WasteDisposalFlags.HasFlag(WasteDisposal.CONTAMINATED_SOLID_WASTE)} \t 6. Silica/TLC:  {WasteDisposalFlags.HasFlag(WasteDisposal.SILICA_TLC)}\n\n" +
                $"Specific Safety or Risk Implication: {Convert.ToString((int)SpecificSafetyFlags, 2).PadLeft(4, '0')} \n" +
                $"\t1. Fire or Explosion:  {SpecificSafetyFlags.HasFlag(SpecificSafety.FIRE_EXPLOSION)} \t 2. Thermal Runaway:        {SpecificSafetyFlags.HasFlag(SpecificSafety.THERMAL_RUNAWAY)}\n" +
                $"\t3. Gas Release:        {SpecificSafetyFlags.HasFlag(SpecificSafety.GAS_RELEASE)} \t 4. Malodorous Substances:  {SpecificSafetyFlags.HasFlag(SpecificSafety.MALODOROUS_SUBSTANCES)}\n\n" +
                "Substances Added: \n";
                CLIInfo(config);
                
                string padding = "D1";
                if (substances.Count() > 99)
                {
                    padding = "D3";
                }
                else if (substances.Count() > 9)
                {
                    padding = "D2";
                }
                for (int i = 0; i < substances.Count; i++)
                {
                    CLIInfo($"\t{(i + 1).ToString(padding)}. {substances[i].Name}  amount: {substances[i].MassVolume}\n");
                }
            }
            private const int maxSubstances = 100;
            public void Add(string substanceName, string amount)
            {
                if (substances.Count() + 1 > maxSubstances)
                {
                    CLIWarn("cannot add any more substances. max capacity reached.\n");
                    return;
                }
                substances.Add(new Substance(substanceName, amount));
            }
            public void Insert(int index, string substanceName, string amount)
            {
                if (!IsValidIndex(index)) return;
                substances.Insert(index, new Substance(substanceName, amount));
            }
            public void Remove(int index)
            {
                if (!IsValidIndex(index)) return;
                substances.RemoveAt(index);
            }
            private bool IsValidIndex(int index)
            {
                bool valid = index < config.substances.Count() && index > -1;
                if (!valid) CLIError($"index {index} is out of range.\n");
                return index < config.substances.Count() && index > -1;
            }
            public void Replace(int index, string substanceName, string amount)
            {
                if (!IsValidIndex(index)) return;
                substances[index].Name = substanceName;
                substances[index].MassVolume = amount;
            }
            public void Move(int fromIndex, int toIndex)
            {
                if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex)) return;
                var tmp = substances[fromIndex];
                substances.RemoveAt(fromIndex);
                substances.Insert(toIndex, tmp);
            }
            public void RemoveAll()
            {
                substances.Clear();
            }
            public void EditName(int index, string newName)
            {
                if (!IsValidIndex(index)) return;
                substances[index].Name = newName;
            }
            public void EditAmount(int index, string newAmount)
            {
                if (!IsValidIndex(index)) return;
                substances[index].MassVolume = newAmount;
            }
        
        }
        static Config config = new Config();
        static Regex CommandRegex = new Regex(@"(?:[^\s""]+|""[^""]+"")");
        static Regex ValidFolderNameRegex = new Regex(@"^(?:.{1,2}/)*[^/\:*?<>|""]+");
        //^(([a-zA-z]:)?[/\\])?(?:[^/\\:*?<>|""]+/)*(?<file>[^/\\:*?<>|""]+\.[^/\\:*?<>|""\.]+)? valid path regex
        //folder name can be anything...
        static Regex IndexNameAmountRegex = new Regex(@"^(?<index>\d+ )?\s*(?<name>[^:\s]+[^:]+[^:]):?\s*(?<amount>[^:]+)?$");

        public static void Generate()
        {
            //if (Path.Exists(config.OutputPath))
            //{
            //    CLIWarn($"Path \"{config.OutputPath}\" already exists. Overwrite? [y/n] ");
            //    string overwrite = Console.ReadLine().Trim();
            //    if (overwrite != "y" || overwrite != "yes")
            //    {
            //        continue;
            //    }
            //}
            //using (COSHHForm form = new COSHHForm(config.OutputPath))
            //{
            //    form.FillStudentInfo(config.Title, config.Name, config.College, config.Year); //todo config.Year
            //    Task<List<SigmaAldrich.Result>>[] tasks = config.Substances.Select(substance => SigmaAldrich.SearchAsync(substance.Name)).ToArray();

            //    Task<SafetyData>[] safetyTasks = new Task<SafetyData>[config.Substances.Count()];
            //    for (int i = 0; i < tasks.Length; i++)
            //    {
            //        tasks[i].Wait();
            //        var results = tasks[i].Result;
            //        if (results.Count() == 0)
            //        {
            //            continue;
            //        }
            //        SigmaAldrich.PrintResults(results);

            //        int resultIndex = -1;
            //        while (resultIndex < 1)
            //        {
            //            CLIInfo("Result index: ");
            //            int.TryParse(Console.ReadLine(), out resultIndex);
            //        }
            //        int productIndex = -1;
            //        while (productIndex < 1)
            //        {
            //            CLIInfo("Product index: ");
            //            int.TryParse(Console.ReadLine(), out productIndex);
            //        }
            //        safetyTasks[i] = SigmaAldrich.SelectResult(results, resultIndex - 1, productIndex - 1);
            //        config.EditName(i, results[i].name);
            //    }
            //    for (int i = 0; i < config.Substances.Count(); i++)
            //    {
            //        safetyTasks[i].Wait();
            //        var substance = config.Substances[i];
            //        form.AddSubstance(substance.Name, substance.MassVolume, safetyTasks[i].Result);
            //    };
            //}

        }

        public static void CreateDirectory(string folder)
        {
            try
            {
                folder = folder.Trim().Trim('\"').Trim('\'');
                string directoryPath = folder;
                if (ValidFolderNameRegex.IsMatch(folder))
                {
                    Directory.CreateDirectory(Path.Combine(config.OutputDir, folder));
                }

                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath);
                }
                else
                {
                    CLIError("directory does not exist\n");
                }
            }
            catch (Exception e)
            {
                CLIError(e.ToString() + "\n");
            }
        }

        public static void DeleteDirectory(string folder)
        {
            try
            {
                folder = folder.Trim().Trim('\"').Trim('\'');
                string directoryPath = folder;

                if (ValidFolderNameRegex.IsMatch(folder))
                {
                    directoryPath = Path.Combine(config.OutputDir, folder);
                }
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath);
                }
                else
                {
                    CLIError("directory does not exist\n");
                }
            }
            catch (Exception e)
            {
                CLIError(e.ToString() + "\n");
            }
        }

        static void StartCommand()
        {
            string command;
            string args = string.Empty;
            List<string> commandList;
            string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, ".config");
            if (Path.Exists(configPath))
            {
                // load in the config
                var lines = File.ReadLines(configPath);
                for (int i = 0; i < 4; i++)
                {
                    var configLine = lines.ElementAt(i).Split(":", 2);
                    var variableName = configLine[0];
                    var variableValue = configLine[1].Trim();
                    switch (variableName)
                    {
                        case "name": config.Name = variableValue; break;
                        case "college": config.College = variableValue; break;
                        case "title": config.Name = variableValue; break;
                        case "year":
                            {
                                int year;
                                bool isNumeric = int.TryParse(variableValue, out year);
                                if (isNumeric)
                                {
                                    config.Year = year;
                                }
                                else
                                {
                                    CLIError("invalid value for year");
                                    return;
                                }
                                break;
                            }
                    }
                }
                if (lines.Count() > 4)
                {
                    for (int i = 4; i < lines.Count(); i++)
                    {
                        var match = IndexNameAmountRegex.Match(lines.ElementAt(i));
                        if (match.Success && 
                            !match.Groups.ContainsKey("index") &&
                             match.Groups.ContainsKey("name") 
                            )
                        {
                            string name = match.Groups["name"].Value;
                            string amount = match.Groups.ContainsKey("amount")  ? match.Groups["amount"].Value : "N/A";
                            config.Add(name, amount);
                        }
                        else
                        {
                            CLIWarn("error reading line {i} : \"{line.ElementAt(i)}\" - ignored");
                        }
                    }
                }
            }
            else
            {
                string defaultConfig =
                    "name: Nick Green\n" +
                    "college: Jesus College\n" +
                    "year: 1\n" +
                    "title: Experiment\n";
                File.WriteAllText(configPath, defaultConfig);
            }
            
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                Console.Write($"{currentTime} COSHH-God@COSHH-Gen {config.OutputDir}\n$ ");

                Console.ResetColor();
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                commandList = CommandRegex.Matches(line).Select(match => match.Value.Trim()).ToList();
                command = commandList[0].ToLower();
                var argv = commandList.GetRange(1, commandList.Count() - 1).Select(it => it.Trim()).ToList(); //idk about trimming it
                var argc = argv.Count();
                args = string.Join(" ", argv);

                if (command == "add") // e.g add [substance](:[amount])
                {
                    if (argc > 0)
                    {
                        var match = IndexNameAmountRegex.Match(args);
                        if (match.Success)
                        {
                            GroupCollection groups = match.Groups;
                            string name = groups["name"].Value;
                            string amount = groups.ContainsKey("amount") ? groups["amount"].Value : "N/A";
                            config.Add(name, amount);
                            CLIInfo($"added substance: \"{name}\"  amount: \"{amount}\"\n");
                        }else
                        {
                            CLIError("incorrect params\n");
                        }
                    }
                    else
                    {
                        CLIWarn("usage: add [substance](:[amount])\n");
                    }
                }
                else if (command == "replace")
                {
                    if (argc > 0)
                    {
                        var match = IndexNameAmountRegex.Match(args);
                        if (match.Success && match.Groups.ContainsKey("index"))
                        {
                            GroupCollection groups = match.Groups;
                            string name = groups["name"].Value;
                            string amount = groups.ContainsKey("amount") ? groups["amount"].Value : "N/A";
                            config.Replace(int.Parse(groups["index"].Value), name, amount);
                        }
                        else
                        {
                            int index;
                            bool isNumeric = int.TryParse(argv[0],out index);
                            if (isNumeric)
                            {
                                string name = string.Empty;
                                string amount = string.Empty;
                                while (string.IsNullOrEmpty(name))
                                {
                                    CLIInfo("New substance: ");
                                    name = Console.ReadLine();
                                }
                                CLIInfo("New amount: \n");
                                amount = Console.ReadLine();
                                if (string.IsNullOrEmpty(amount))
                                {
                                    amount = "N/A";
                                }
                                config.Replace(index, name, amount);
                            }
                            else
                            {
                                //CLIError("invalid index\n");
                                CLIError("incorrect params\n");
                            }
                        }
                    }
                    else
                    {
                        string usage =
                        "usage 1: replace [index] [new substance]:[new amount]" +
                        "NB: default value of new amount is \"N/A\"" +
                        "usage 2: replace [index]";
                        CLIWarn(usage);
                    }
                }
                else if (command == "edit-name")
                {
                    if (argc > 0)
                    {
                        var match = IndexNameAmountRegex.Match(args);
                        if (match.Success && match.Groups.ContainsKey("index"))
                        {
                            string newName = string.Empty;
                            if (match.Groups.ContainsKey("name"))
                            {
                                newName = match.Groups["name"].Value;
                            }
                            else
                            {
                                newName = Console.ReadLine();
                                if (string.IsNullOrEmpty(newName))
                                {
                                    CLIInfo("operation cancelled.\n");
                                    continue;
                                }
                            }
                            config.EditName(int.Parse(match.Groups["index"].Value) - 1, newName);
                        }
                    }
                    else
                    {
                        CLIWarn("usage: edit-name [index] [new name]\n");
                    }
                }
                else if (command == "edit-amount")
                {
                    if (argc > 0)
                    {
                        var match = IndexNameAmountRegex.Match(args);
                        if (match.Success && match.Groups.ContainsKey("index"))
                        {
                            string newName = string.Empty;
                            if (match.Groups.ContainsKey("name"))
                            {
                                newName = match.Groups["name"].Value;
                            }
                            else
                            {
                                newName = Console.ReadLine();
                                if (string.IsNullOrEmpty(newName))
                                {
                                    CLIInfo("operation cancelled.\n");
                                    continue;
                                }
                            }
                            config.EditAmount(int.Parse(match.Groups["index"].Value) - 1, newName);
                        }
                    }
                    else
                    {
                        CLIWarn("usage: edit-amount [index] [new amount]\n");
                    }

                }
                else if (command == "remove" || command == "rm")
                {
                    if (argc == 1)
                    {
                        if (argv[0] == "*")
                        {
                            config.RemoveAll();
                        }
                        int index;
                        bool isNumeric = int.TryParse(commandList[1], out index);
                        index -= 1;
                        if (isNumeric)
                        {
                            config.Remove(index);
                        }
                        else
                        {
                            CLIError("invalid index!");
                        }
                    }
                    else
                    {
                        CLIWarn("usage: (remove|rm) [index of substance].\n");
                        CLIWarn("      \"(remove|rm) *\" to remove all\n");
                    }

                }
                else if (command == "set")
                {
                    if (argc > 1)
                    {
                        string variableName = argv[0].ToLower();
                        string variableValue = string.Join(" ", argv.GetRange(1, argv.Count() - 1));
                        bool isTrue = variableValue == "1" || variableValue == "true";
                        bool isFalse = variableValue == "1" || variableValue == "true";
                        switch (variableName)
                        {
                            case "name": config.Name = variableValue; break;
                            case "title": config.Title = variableValue; break;
                            case "college": config.College = variableValue; break;
                            case "outputname": config.OutputName = variableValue; break;
                            case "year":
                                {
                                    int year;
                                    bool isNumeric = int.TryParse(variableValue, out year);
                                    if (isNumeric)
                                    {
                                        config.Year = year;
                                    }
                                    else
                                    {
                                        CLIError("invalid value");
                                    }
                                    break;
                                };
                            case "waste":
                                {
                                    try
                                    {
                                        int intEnum = Convert.ToInt32(variableValue);
                                        if (intEnum > 0 && intEnum < 0b111111)
                                        {
                                            config.WasteDisposalFlags = (Config.WasteDisposal)intEnum;
                                        }
                                    }
                                    catch (Exception e)
                                    {

                                    }

                                    break;
                                }
                            case "halogenated":
                                {
                                    if (isTrue)
                                    {
                                        config.WasteDisposalFlags |= Config.WasteDisposal.HALOGENATED;
                                    }
                                    else if (isFalse)
                                    {
                                        config.WasteDisposalFlags &= Config.WasteDisposal.HALOGENATED;
                                    }
                                    break;
                                }
                            case "aqueous":
                                {
                                    if (isTrue)
                                    {
                                        config.WasteDisposalFlags |= Config.WasteDisposal.AQUEOUS;
                                    }
                                    else if (isFalse)
                                    {
                                        config.WasteDisposalFlags &= Config.WasteDisposal.AQUEOUS;
                                    }
                                    break;
                                }
                            case "hydrocarbon":
                                {
                                    if (isTrue)
                                    {
                                        config.WasteDisposalFlags |= Config.WasteDisposal.HYDROCARBON;
                                    }
                                    else if (isFalse)
                                    {
                                        config.WasteDisposalFlags &= Config.WasteDisposal.HYDROCARBON;
                                    }
                                    break;
                                }
                            case "named-waste":
                                {
                                    if (isTrue)
                                    {
                                        config.WasteDisposalFlags |= Config.WasteDisposal.NAMED_WASTE;
                                    }
                                    else if (isFalse)
                                    {
                                        config.WasteDisposalFlags &= Config.WasteDisposal.NAMED_WASTE;
                                    }
                                    break;
                                }
                            case "contaminated":
                                {
                                    if (isTrue)
                                    {
                                        config.WasteDisposalFlags |= Config.WasteDisposal.CONTAMINATED_SOLID_WASTE;
                                    }
                                    else if (isFalse)
                                    {
                                        config.WasteDisposalFlags &= Config.WasteDisposal.CONTAMINATED_SOLID_WASTE;
                                    }
                                    break;
                                }
                            case "silica":
                                {
                                    if (isTrue)
                                    {
                                        config.WasteDisposalFlags |= Config.WasteDisposal.SILICA_TLC;
                                    }
                                    else if (isFalse)
                                    {
                                        config.WasteDisposalFlags &= Config.WasteDisposal.SILICA_TLC;
                                    }
                                    break;
                                }
                            case "fire-explosion":
                                {
                                    if (isTrue)
                                    {
                                        config.SpecificSafetyFlags |= Config.SpecificSafety.FIRE_EXPLOSION;
                                    }
                                    else if (isFalse)
                                    {
                                        config.SpecificSafetyFlags &= Config.SpecificSafety.FIRE_EXPLOSION;
                                    }
                                    break;
                                }
                            case "gas-release":
                                {
                                    if (isTrue)
                                    {
                                        config.SpecificSafetyFlags |= Config.SpecificSafety.GAS_RELEASE;
                                    }
                                    else if (isFalse)
                                    {
                                        config.SpecificSafetyFlags &= Config.SpecificSafety.GAS_RELEASE;
                                    }
                                    break;
                                }
                            case "thermal-runaway":
                                {
                                    if (isTrue)
                                    {
                                        config.SpecificSafetyFlags |= Config.SpecificSafety.THERMAL_RUNAWAY;
                                    }
                                    else if (isFalse)
                                    {
                                        config.SpecificSafetyFlags &= Config.SpecificSafety.THERMAL_RUNAWAY;
                                    }
                                    break;
                                }
                            case "malodorous":
                                {
                                    if (isTrue)
                                    {
                                        config.SpecificSafetyFlags |= Config.SpecificSafety.MALODOROUS_SUBSTANCES;
                                    }
                                    else if (isFalse)
                                    {
                                        config.SpecificSafetyFlags &= Config.SpecificSafety.MALODOROUS_SUBSTANCES;
                                    }
                                    break;
                                }
                            default: break;
                        }
                    }
                    else
                    {
                        string usage =
                            "usage: set [variable] [value]\n" +
                            "e.g.   set name Nick Green\n\n";
                        CLIInfo(usage);
                        string availableVariables =
                            "available variables: \n" +
                            "name, title, college, year, outputname,\n" +
                            "waste, halogenated, aqueous, hydrocarbon, named-waste, contaminated, silica,\n" +
                            "safety, fire-explosion, gas-release, thermal-runaway, malodorous\n";
                        CLIInfo(availableVariables);
                    }
                }
                else if (command == "search")
                {
                    if (argc > 0)
                    {
                        string searchQuery = args.Trim().Trim('\"').Trim('\'');
                        var results = SigmaAldrich.SearchAsync(args);
                        CLIInfo("searching\n");
                        Console.CursorVisible = false;
                        var speed = 100;
                        while (!results.IsCompleted)
                        {
                            Console.SetCursorPosition(9, Console.CursorTop - 1);
                            CLIInfo(".  \n");
                            Thread.Sleep(speed);
                            Console.SetCursorPosition(9, Console.CursorTop - 1);
                            CLIInfo(".. \n");
                            Thread.Sleep(speed);
                            Console.SetCursorPosition(9, Console.CursorTop - 1);
                            CLIInfo("...\n");
                            Thread.Sleep(speed);
                        }
                        Console.CursorVisible = true;
                        if (results.Result.Any())
                        {
                            SigmaAldrich.PrintResults(results.Result);
                        }
                        else
                        {
                            CLIInfo($"Sigma Aldrich couldn’t find any matches for \"{searchQuery}\"");
                        }
                        
                        //Console.Write("Continue? [y/n] ");
                        //string shouldContinue = Console.ReadLine();

                        //if (shouldContinue == "y" || shouldContinue == "yes")
                        //{

                        //}
                    }
                    else
                    {
                        CLIWarn("usage: search [query]\n");
                    }
                }
                else if (command == "cd")
                {
                    if (argc > 0)
                    {
                        string newPath = args.Trim().Trim('\"').Trim('\'');
                        string newDir = ValidFolderNameRegex.IsMatch(newPath) ? Path.GetFullPath(Path.Combine(config.OutputDir, newPath)) : Path.GetFullPath(args);

                        if (Directory.Exists(newDir))
                        {
                            config.OutputDir = newDir;
                        }
                        else
                        {
                            CLIWarn("invalid directory\n");
                        }
                    }
                    else
                    {
                        config.OutputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
                    }

                }
                else if (command == "mkdir")
                {
                    if (argc > 0)
                    {
                        CreateDirectory(args);
                    }
                    else
                    {
                        CLIWarn("usage: mkdir /path/to/create\n");
                    }
                }
                else if (command == "rmdir")
                {
                    if (argc > 0)
                    {
                        DeleteDirectory(args);
                    }
                    else
                    {
                        CLIWarn("usage: rmdir /path/to/remove\n");
                    }
                }

                // commands that require no arguments
                else if (command == "generate" || command == "gen")
                {
                    try
                    {
                        Generate();
                    }
                    catch (Exception e)
                    {
                        CLIError("failed to generate COSHH form");
                        CLIError(e.ToString());
                    }
                }
                else if (command == "config" || command == "conf")
                {
                    config.print();
                }
                else if (command == "ls")
                {
                    foreach (var item in Directory.GetDirectories(config.OutputDir).Select(Path.GetFileName))
                    {
                        CLIInfo(item + "\n");
                    }
                }
                else if (command == "who" || command == "whoami")
                {
                    CLIInfo(config.Name + "\n");
                }
                else if (command == "help")
                {
                    string help =
                        "add [substance](:[amount])               : adds substance\n" +
                        "set [variable] [value]                   : sets variable to value\n" +
                        "(remove|rm) [index]                      : removes substance at index\n" +
                        "replace [index] ([substance](:[amount])) : replaces substance at index\n" +
                        "search [substance]                       : searches for substance with Sigma Aldrich\n" +
                        "(who|whoami)                             : prints the name\n" +
                        "cd (path)                                : changes output directory; (default=executable-path)\n" +
                        "(generate|gen)                           : generates the COSHH form\n" +
                        "(config|conf)                            : print all the variables\n" +
                        "(clear|cls)                              : clears the console\n" +
                        "pwd                                      : prints the current output directory\n" +
                        "open                                     : opens the current output directory\n" +
                        "mkdir [folder]                           : opens the current output directory\n" +
                        "rmdir [folder]                           : opens the current output directory\n" +
                        "ls                                       : lists all the subdirectories in the current output directory\n";
                    CLIInfo(help);
                }
                else if (command == "clear" || command == "cls")
                {
                    Console.Clear();
                }
                else if (command == "exit" || command == "quit")
                {
                    break;
                }
                else if (command == "pwd")
                {
                    CLIInfo(config.OutputDir + "\n");
                }
                else if (command == "open")
                {
                    Process.Start("explorer.exe", config.OutputDir);
                }
                else if (command == "pepe")
                {
                    string pepe = "⠀⠀⢀⣠⠤⠶⠖⠒⠒⠶⠦⠤⣄⠀⠀⠀⣀⡤⠤⠤⠤⠤⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⣴⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⣦⠞⠁⠀⠀⠀⠀⠀⠀⠉⠳⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⡾⠁⠀⠀⠀⠀⠀⠀⣀⣀⣀⣀⣀⣀⣘⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⣆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⢀⡴⠚⠉⠁⠀⠀⠀⠀⠈⠉⠙⠲⣄⣤⠤⠶⠒⠒⠲⠦⢤⣜⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⡄⠀⠀⠀⠀⠀⠀⠀⠉⠳⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⠹⣆⠀⠀⠀⠀⠀⠀⣀⣀⣀⣹⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⣠⠞⣉⣡⠤⠴⠿⠗⠳⠶⣬⣙⠓⢦⡈⠙⢿⡀⠀⠀⢀⣼⣿⣿⣿⣿⣿⡿⣷⣤⡀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⣾⣡⠞⣁⣀⣀⣀⣠⣤⣤⣤⣄⣭⣷⣦⣽⣦⡀⢻⡄⠰⢟⣥⣾⣿⣏⣉⡙⠓⢦⣻⠃⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠉⠉⠙⠻⢤⣄⣼⣿⣽⣿⠟⠻⣿⠄⠀⠀⢻⡝⢿⡇⣠⣿⣿⣻⣿⠿⣿⡉⠓⠮⣿⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠙⢦⡈⠛⠿⣾⣿⣶⣾⡿⠀⠀⠀⢀⣳⣘⢻⣇⣿⣿⣽⣿⣶⣾⠃⣀⡴⣿⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠙⠲⠤⢄⣈⣉⣙⣓⣒⣒⣚⣉⣥⠟⠀⢯⣉⡉⠉⠉⠛⢉⣉⣡⡾⠁⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⣠⣤⡤⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢈⡿⠋⠀⠀⠀⠀⠈⠻⣍⠉⠀⠺⠿⠋⠙⣦⠀⠀⠀⠀⠀⠀⠀\r\n⠀⣀⣥⣤⠴⠆⠀⠀⠀⠀⠀⠀⠀⣀⣠⠤⠖⠋⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⠀⠀⠀⠀⠀⢸⣧⠀⠀⠀⠀⠀⠀\r\n⠸⢫⡟⠙⣛⠲⠤⣄⣀⣀⠀⠈⠋⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠏⣨⠇⠀⠀⠀⠀⠀\r\n⠀⠀⠻⢦⣈⠓⠶⠤⣄⣉⠉⠉⠛⠒⠲⠦⠤⠤⣤⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣠⠴⢋⡴⠋⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠉⠓⠦⣄⡀⠈⠙⠓⠒⠶⠶⠶⠶⠤⣤⣀⣀⣀⣀⣀⣉⣉⣉⣉⣉⣀⣠⠴⠋⣿⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠉⠓⠦⣄⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡼⠁⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠙⠛⠒⠒⠒⠒⠒⠤⠤⠤⠒⠒⠒⠒⠒⠒⠚⢉⡇⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠴⠚⠛⠳⣤⠞⠁⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⠚⠁⠀⠀⠀⠀⠘⠲⣄⡀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣴⠋⠙⢷⡋⢙⡇⢀⡴⢒⡿⢶⣄⡴⠀⠙⠳⣄⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢦⡀⠈⠛⢻⠛⢉⡴⣋⡴⠟⠁⠀⠀⠀⠀⠈⢧⡀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⡄⠀⠘⣶⢋⡞⠁⠀⠀⢀⡴⠂⠀⠀⠀⠀⠹⣄⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠈⠻⢦⡀⠀⣰⠏⠀⠀⢀⡴⠃⢀⡄⠙⣆⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡾⢷⡄⠀⠀⠀⠀⠉⠙⠯⠀⠀⡴⠋⠀⢠⠟⠀⠀⢹⡄\n";

                    CLIInfo(pepe);
                }
                else if (command == "oxford")
                {
                    string oxford = "\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣀⣤⣤⣶⣶⣶⣶⣿⣿⣿⣿⣿⣿⣶⣶⣶⣶⣤⣤⣀⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣤⣴⣾⣿⣿⣿⣿⡻⣿⣭⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⣿⣿⣷⣦⣤⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣀⣤⣾⣿⣿⣟⡛⣥⣿⣿⣿⣷⡜⢼⣿⣿⣿⣿⣿⣧⣿⣿⣿⣿⡏⢰⣿⣷⠙⣿⣿⣿⣿⣷⣤⣀⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣠⣶⣿⡿⣿⣿⣿⣿⣷⡈⢿⣿⣿⣿⣷⣾⣿⠿⠿⠿⠿⠿⠿⣿⣿⣿⣷⣜⣿⣋⣾⣿⣿⡿⢈⣟⢿⣿⣶⣄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⣠⣾⣿⣿⣿⣦⡙⢿⣿⣿⣿⣷⣿⠿⢛⣉⣩⣵⣶⣶⣶⣶⣶⣶⣶⣶⣮⣍⣉⡛⠿⣿⣿⣿⣟⣡⣿⣿⣿⣿⣿⣿⣷⣄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⢀⣾⣿⢟⣟⣿⣿⣿⣿⣤⣿⡿⠛⣥⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣬⠛⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡀⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⢠⣿⣿⣿⣭⣭⣤⣝⣿⣿⠟⣡⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣌⠻⣿⣿⣿⣿⣭⣿⣿⣿⣿⡄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⣰⣿⣿⣿⣿⣿⣿⣶⣿⠟⣡⣾⣏⠿⣿⣿⣿⠏⢃⠫⢿⣿⣿⡿⢏⣿⣿⣏⠻⢿⣿⣿⠏⢡⠋⢿⣿⣿⡿⠟⣷⣌⠻⣿⣿⣿⣿⣿⣿⣿⣿⣆⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⣰⣿⡟⠵⡆⣛⣛⣿⣿⢋⣶⣿⣿⣿⣠⢄⣿⢿⣒⢁⢐⡸⢺⣃⢴⣸⣿⣿⣿⡆⢅⣰⣾⣁⢠⠠⣨⣲⣃⠬⣸⣿⣿⣶⡙⣿⣿⣿⠋⣉⣝⣿⣿⣆⠄⠄⠄⠄\r\n⠄⠄⠄⢰⣿⣿⣿⣷⣦⣍⣿⣿⢁⣾⣿⣿⣿⣿⣿⡾⡍⢟⠄⠉⠅⢜⢋⠽⣾⣿⣿⣿⣿⣿⡾⠅⢛⡆⢍⠅⢜⠃⠝⣶⣿⣿⣿⣿⣷⡈⣿⣿⢾⣿⡟⢻⣿⣿⡆⠄⠄⠄\r\n⠄⠄⠄⣾⣿⢿⣿⣿⣿⣿⣿⠁⣿⣿⣿⣿⣿⣿⣿⣿⣛⣭⠙⠛⠋⢩⣝⣿⣿⣿⣿⣿⣿⣿⣿⣟⣭⠙⠛⠛⢩⣝⣿⣿⣿⣿⣿⣿⣿⣿⠈⣿⣿⣶⣾⣿⣿⣿⣷⠄⠄⠄\r\n⠄⠄⢸⣿⡟⡺⠯⠽⢦⣿⡟⣼⣿⣿⣿⣿⠿⡿⢿⣿⣿⡿⠿⠿⠿⠿⠿⠿⠿⠿⠿⡿⠿⠿⠿⠿⠿⠿⠿⠿⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣧⢻⣿⡿⣿⣿⠟⢿⣿⡇⠄⠄\r\n⠄⠄⢸⣿⣿⣿⣷⣶⣾⣿⠄⣿⣿⣿⣿⣿⠶⡷⣤⣿⢹⡇⠐⠢⣤⠔⡄⢆⣰⢰⠂⠄⢀⢠⡠⡄⢄⠄⠠⠄⠄⡏⣿⣿⣿⣿⣿⣿⣿⣿⣿⠄⣿⣷⠟⣁⣺⢾⣿⡇⠄⠄\r\n⠄⠄⣿⣿⡿⣿⣿⣿⣿⣿⢰⣿⣿⣿⣿⣿⠶⢷⣤⣿⢸⡇⠐⠂⠉⠒⠑⠚⠘⠚⠂⠄⠋⠘⠓⠃⠊⠁⠊⠂⠄⡇⣿⣿⣿⣿⣿⣿⣿⣿⣿⡆⣿⣿⣾⣿⣿⣿⣿⣿⠄⠄\r\n⠄⠄⣿⣿⣿⡿⠞⣹⣿⣿⢸⣿⣿⣿⣿⣿⣤⣧⣤⣿⢸⡇⠄⠘⢺⠘⡜⠘⢪⠄⠄⠄⠄⢹⡏⢸⢸⠄⡇⠄⠄⡇⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⣿⣿⢿⣿⣿⣿⣿⣿⠄⠄\r\n⠄⠄⢹⣿⣧⣾⣿⣿⣿⣿⠄⣿⣿⣿⣿⣿⣤⣧⡤⣿⢸⡇⠄⡄⡄⠠⡄⠠⡄⡄⠄⠄⠠⡄⡠⢠⠤⠄⡄⠄⠄⡇⣿⣿⣿⣿⣿⣿⣿⣿⣿⠄⣿⣿⣶⠶⣦⢼⣿⡏⠄⠄\r\n⠄⠄⢸⣿⣿⣿⣿⣿⠿⣿⣆⢿⣿⣿⣿⣿⣀⣶⠶⣿⢸⡇⠄⠇⠧⠤⠧⠄⠁⠄⠄⠄⠨⠄⠰⠸⠼⠨⠸⠄⠄⡇⣿⣿⣿⣿⣿⣿⣿⣿⡿⣰⣿⣿⣿⣿⣷⣾⣿⡇⠄⠄\r\n⠄⠄⠈⣿⣿⣎⣥⣶⣶⣿⣿⡈⣿⣿⣿⣿⣀⣦⠶⣿⠸⠓⠄⠄⠄⠄⠐⠒⠂⡰⣀⣀⢆⠐⠒⠂⠄⠄⠄⠄⠒⠇⣿⣿⣿⣿⣿⣿⣿⣿⢁⣿⡿⣛⡙⠻⣿⣿⣿⠁⠄⠄\r\n⠄⠄⠄⢸⣿⣿⣿⣿⣿⠿⣿⣷⠙⣿⣿⣿⣍⣵⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣾⣿⣿⣷⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠋⣾⣿⠱⣿⣿⡇⣿⣿⡇⠄⠄⠄\r\n⠄⠄⠄⠄⢻⣿⣿⢯⡾⣠⣿⣿⣷⡘⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⠻⢿⣿⣿⣿⡟⢳⠘⣻⣿⣿⣿⡿⠟⣻⣿⣿⣿⣿⣿⣿⣿⣿⣿⢃⣾⣿⣿⣷⣤⣤⣾⣿⡟⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⢻⣿⣿⢀⣻⣭⣮⣿⣿⣤⠻⣿⣿⣿⣿⣿⣿⣿⣿⣦⠂⠸⡏⣟⡆⢀⡀⣐⡋⣼⢃⠡⣸⣿⣿⣿⣿⣿⣿⣿⣿⠟⣤⣿⣟⣙⠿⣿⣿⣿⣿⡟⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠹⣿⣾⣿⣿⣿⢿⣛⡿⣷⣌⠻⢿⣿⣿⣿⣿⣿⣿⣶⡭⠕⢚⠷⡾⠷⢟⠓⠫⢵⣶⣿⣿⣿⣿⣿⣿⡿⠟⣡⣾⣿⣯⣬⡍⠶⢌⣿⣿⠏⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠘⣿⣿⣟⣽⣿⣿⠇⣿⣿⣿⣦⣉⠿⣿⢿⣿⣿⣿⣎⠠⣬⣬⣤⣤⣤⡤⢈⣽⣿⣿⣿⣿⡿⠿⣉⣴⣿⢿⠋⣿⣿⣿⣿⣶⣿⣿⠃⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠻⣿⣿⣿⣤⣿⣿⣿⣿⣿⢟⣼⠿⣣⣻⣿⣿⣷⣺⣥⣤⣤⣤⣤⣽⣷⣿⢿⡟⣋⣥⣶⣿⣿⣿⡇⣿⣿⣄⠻⣿⣿⣿⠟⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠈⠻⣿⣿⣿⣿⣯⢶⣬⣛⠱⣿⣿⣿⣿⣿⢿⣭⣿⣿⢋⣴⣴⣿⣿⣿⢂⣼⣿⣿⣿⣿⣿⣿⣿⣤⣽⣭⣿⣿⠟⠁⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠙⠿⣿⣿⢹⣷⣿⣽⣿⣿⣿⣿⢷⣮⣉⣙⠳⠴⢻⣿⡿⣏⢆⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⣋⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠈⠉⣼⣗⣿⣿⣿⣿⣿⣏⣀⣀⢹⣿⣷⣶⣶⣬⣍⣉⣘⡛⠻⠿⠭⠭⠭⠭⠽⠿⠛⠃⠉⠁⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠙⠉⠉⠛⣿⣿⡟⠋⢽⣟⡿⢿⣿⣯⣽⣿⣿⣿⣿⡟⣿⣿⣿⣶⣶⣶⡿⠛⠉⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠙⠃⠄⠄⠄⠄⣭⣟⡚⠋⠹⠿⠿⢿⣿⠿⠿⠿⠛⠙⠈⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣸⣽⣿⣿⣿⣿⣿⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠰⠿⢻⢿⠿⡿⡟⠯⠇⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠈⡿⠿⢿⠁⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⢔⣊⣶⣿⣶⣕⡢⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠐⠯⠂⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄\r\n\r";
                    CLIInfo(oxford);
                }
                else if (command == "walter")
                {
                    string walter = "⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠿⠿⠿⠿⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⣿⠟⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠉⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢺⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠆⠜⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⠿⠿⠛⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠻⣿⣿⣿⣿⣿\r\n⣿⣿⡏⠁⠀⠀⠀⠀⠀⣀⣠⣤⣤⣶⣶⣶⣶⣶⣦⣤⡄⠀⠀⠀⠀⢀⣴⣿⣿⣿⣿⣿\r\n⣿⣿⣷⣄⠀⠀⠀⢠⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⢿⡧⠇⢀⣤⣶⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣾⣮⣭⣿⡻⣽⣒⠀⣤⣜⣭⠐⢐⣒⠢⢰⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⣏⣿⣿⣿⣿⣿⣿⡟⣾⣿⠂⢈⢿⣷⣞⣸⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⣿⣽⣿⣿⣷⣶⣾⡿⠿⣿⠗⠈⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠻⠋⠉⠑⠀⠀⢘⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⣿⡿⠟⢹⣿⣿⡇⢀⣶⣶⠴⠶⠀⠀⢽⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⣿⣿⣿⡿⠀⠀⢸⣿⣿⠀⠀⠣⠀⠀⠀⠀⠀⡟⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⣿⣿⣿⡿⠟⠋⠀⠀⠀⠀⠹⣿⣧⣀⠀⠀⠀⠀⡀⣴⠁⢘⡙⢿⣿⣿⣿⣿⣿⣿⣿⣿\r\n⠉⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⢿⠗⠂⠄⠀⣴⡟⠀⠀⡃⠀⠉⠉⠟⡿⣿⣿⣿⣿\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢷⠾⠛⠂⢹⠀⠀⠀⢡⠀⠀⠀⠀⠀⠙⠛⠿⢿\n";
                    CLIInfo(walter);
                }
                else if (command == "hank")
                {
                    string hank = "⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠖⠁⠀⠀⠀⠀⠀⠈⠋⠳⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⣠⣾⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠙⢦⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⢠⣿⡿⢁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠸⣷⣀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⣿⣿⡧⢕⠂⠀⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠀⣿⣿⡄⠀⠀⠀⠀\r\n⠀⠀⠀⠀⢸⣿⣿⠏⠃⠈⠉⠀⠀⠀⠀⠀⠀⠈⠈⠀⠀⠀⠐⣿⣿⡇⠀⠀⠀⠀\r\n⠀⠀⠀⠀⢸⣿⣿⣿⣿⣴⣶⡆⠀⠀⠀⠀⠀⠀⠀⣴⣴⣴⣄⣿⣿⣇⠀⠀⠀⠀\r\n⠀⠀⠀⠀⣿⣿⣿⣦⣤⣤⠌⣻⣦⣤⡀⢀⣤⡠⢔⡀⢀⣀⣉⢻⢿⣿⠇⠀⠀⠀\r\n⠀⠀⠀⠀⢿⣿⠋⡟⠙⠛⠚⠘⠋⢉⠁⠈⠙⠋⠃⠿⠟⠛⠩⠹⣿⡿⠀⠀⠀⠀\r\n⠀⠀⠀⠀⢸⣿⡎⡐⠙⠓⠚⠉⠠⣾⡆⠀⢰⠀⠒⠒⠒⠂⡀⢺⣿⠇⠀⠀⠀⠀\r\n⠀⠀⠀⠀⢸⣿⣿⡁⠀⠀⠀⡤⠔⡎⠂⠀⠈⡖⣀⡀⠀⠀⠨⠄⢿⠂⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠈⣷⣿⣧⣠⡖⠁⠀⣿⣧⣀⣀⣠⣷⠀⠉⢦⣤⣤⣷⡗⠁⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⢻⣸⠉⠛⢿⣶⣄⡨⠻⢿⣿⣿⣅⣄⣶⣾⠿⠛⣿⡇⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠘⢿⣦⠀⠀⠈⠻⢍⠙⠉⠋⠋⠉⡿⠋⠀⠀⣼⡿⠁⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠈⠞⣦⠀⢬⢧⣄⣀⡁⠉⣀⣠⡽⠁⠀⡼⠅⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠻⣧⣄⡉⠛⠛⠛⠛⠛⠋⠀⢠⣼⠋⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣷⡄⠈⠀⠀⢀⢀⣤⡿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠻⠶⠼⠟⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣤⣶⣶⣶⣦⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⣀⣴⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⣠⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣦⡀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⣼⣿⣿⣿⣿⣿⣿⣿⡿⡱⡿⣿⣿⣿⣿⣿⣿⣿⣷⡀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⢰⣿⣿⣿⣿⡟⠟⠛⠁⠄⠂⠀⠓⠋⢟⣿⣿⣿⣿⣿⣇⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⢸⣿⣿⣿⠜⢖⣁⣀⣀⡀⠀⢀⣐⢠⣴⣟⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⢸⣿⣿⣷⣛⠿⠻⠉⠯⣉⠠⠹⠚⠺⠿⢿⣟⣿⣿⣿⣿⠀⠀⠀⠀⠀\r\n⠀⠀⠀⢐⣿⣿⡿⡿⣿⣷⠆⠀⠐⠀⠀⠀⠀⢠⣼⣶⣿⣿⣿⣿⣿⡶⠀⠀⠀⠀\r\n⠀⠀⠀⠘⣿⡟⠻⣋⢉⣉⠁⣀⢀⣄⣠⣄⡀⠀⣉⣉⣙⠻⢻⣿⣿⡷⠀⠀⠀⠀\r\n⠀⠀⠀⠀⣿⣿⣾⡏⠑⠓⠉⢉⣾⣿⣿⣿⣯⠑⠚⠓⢛⣾⣼⣿⣿⡇⠀⠀⠀⠀\r\n⠀⠀⠀⠀⢹⣿⡿⠻⠠⢤⣔⣼⣿⣿⣾⣿⣿⣲⣄⠀⠘⠻⣿⣿⡟⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠘⣿⣧⠀⣀⠟⠃⠡⣿⠿⠞⠽⣿⠈⠛⣶⣀⣇⣿⡿⠃⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⢹⣿⣾⣯⠃⢠⣴⣦⠀⠀⣠⣴⣦⡀⠘⣿⡷⣿⠁⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⣿⣾⡇⢠⡿⡿⠻⠁⢠⠸⠻⢿⢷⠀⢸⣿⠏⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠘⣿⡇⠊⣠⣤⣤⣤⣤⣶⣦⣤⣀⠃⣾⡟⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠘⣧⣬⡶⠀⠀⠀⠀⠀⠀⠸⣿⣦⡟⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣿⣿⡿⡛⢟⢶⣶⣽⡿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠛⠿⢶⣴⡿⠟⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\n";
                    CLIInfo(hank);
                }
                else
                {
                    CLIWarn("invalid command. Try 'help' for more information.\n");
                }
            }
            Console.Clear();
        }

        static bool ReadFile(string path)
        {
            //todo check if relative
            if (!File.Exists(path))
            {
                Console.WriteLine($"file {path} does not exist");
                return false;
            }
            try
            {
                var lines = File.ReadLines(path);
                foreach (var line in lines)
                {
                    var substance = line.Split(":").Select(s => s.Trim()).ToList();
                    if (substance.Count() == 2)
                    {
                        config.Add(substance[0], substance[1]);
                    }
                    else if (substance.Count == 1)
                    {
                        config.Add(substance[0], "N/A");
                    }
                    else
                    {
                        throw new Exception($"Invalid format: {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false; ;
            }
            return true;
        }

        public static int CLIMain(string[] args)
        {
            //string newpath = Path.Combine(config.OutputDir, "./../lolololo adsa d");
            //CLIInfo(newpath);
            //Directory.CreateDirectory(newpath);
            Console.WriteLine("searching");
            Console.WriteLine("searching");
            

            switch (args.Length)
            {
                case 0:
                // CLI mode
                InitCLI:
                    Console.OutputEncoding = Encoding.UTF8;
                    StartCommand();
                    break;
                case 1:
                    // Open the file containing the list of substances with amount(optional)
                    // Expected format of the file is as follows:
                    // [substance name]:[mass/volume]
                    if (ReadFile(args[0]))
                    {
                        goto InitCLI;
                    }
                    break;
                case 2:
                    if (args[0] == "-s" || args[0] == "--search")
                    {
                        var results = SigmaAldrich.SearchAsync(args[1]);
                        Console.WriteLine("searching");
                        while (!results.IsCompleted)
                        {
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            Console.WriteLine("1");
                        }
                    }
                    else if (args[0] == "-f" || args[0] == "--file")
                    {
                        if (ReadFile(args[1]))
                        {
                            goto InitCLI;
                        }
                    }
                    else
                    {
                        PrintUsage();
                        return 1;
                    }
                    break;
                default:
                    PrintUsage();
                    break;

            }


            return 0;
            
            return 0;
        }
        
    }
}
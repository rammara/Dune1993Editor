using DuneEd;

namespace DuneEdC
{
    public static class Program
    {
        const string INI_FILE = "duneed.ini";

        private const string CMD_EXIT = "EXIT";
        private const string CMD_SETGAMEPATH = "SETPATH";
        private const string CMD_UNPACK = "UNPACK";
        private const string CMD_LOAD = "LOAD";
        private const string CMD_HELP = "HELP";

        private static SavedGame? _loadedFile;

        static readonly Dictionary<string, string> commands = new()
        {
            { "0", CMD_EXIT },
            { "1", CMD_SETGAMEPATH },
            { "2", CMD_LOAD },
            { "3", CMD_UNPACK },
            { "4", CMD_HELP }
        };

        public static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                RunCommand(CMD_LOAD, args[0]);
            }
            if (File.Exists(INI_FILE))
            {
                Settings.Load(INI_FILE);
                Console.Write("INI file loaded.");
            }

            if (string.IsNullOrEmpty(Settings.GamePath))
            {
                Console.WriteLine("No game path is set.");
            }
            else
            {
                Console.WriteLine($"Game path set to: {Settings.GamePath}.");
            }

            DisplayCommands();
            while (true) // main loop
            {
                var cmd = GetCommand(out string? arguments);
                if (cmd is null) continue;
                if (CMD_EXIT == cmd) break;
                RunCommand(cmd, arguments);
            } // While
            return 0;
        } // main

        private static void RunCommand(string command, string? argument)
        {
            switch (command)
            {
                case CMD_HELP:
                    DisplayCommands();
                    return;
                case CMD_SETGAMEPATH:
                    if (string.IsNullOrEmpty(argument))
                    {
                        Console.Write("Input path: ");
                        argument = Console.ReadLine();
                        if (string.IsNullOrEmpty(argument))
                        {
                            Console.Write("No path is provided");
                            return;
                        }
                    }
                    if (!Directory.Exists(argument))
                    {
                        Console.WriteLine("Invalid path or path does not exist.");
                        return;
                    }

                    Settings.GamePath = argument;
                    Settings.Save(INI_FILE);
                    Console.WriteLine($"Game path is set to {argument}");
                    return;
                case CMD_LOAD:
                    if (string.IsNullOrEmpty(argument))
                    {
                        var path = string.Empty;
                        string[]? files = default;

                        if (string.IsNullOrEmpty(Settings.GamePath))
                        {
                            Console.WriteLine("No game path is set. Full path required.");
                        }
                        else
                        {
                            Console.WriteLine($"Game path is set to {Settings.GamePath}.");
                            files = Directory.GetFiles(Settings.GamePath, "*.sav");
                            if (files is not null && files.Length > 0)
                            {
                                Console.WriteLine("Available saved games:");
                                for (int i = 0; i < files.Length; i++)
                                {
                                    string? file = files[i];
                                    Console.WriteLine($"[{i}]: {Path.GetFileName(file)}");
                                }
                            }
                            path = Settings.GamePath;
                        }
                        if (files is null || files.Length == 0)
                        {
                            Console.WriteLine("No saved games in this directory.");
                            return;
                        }

                        Console.Write("Enter filename (or a number):");
                        var userinput = Console.ReadLine();
                        if (string.IsNullOrEmpty(userinput))
                        {
                            Console.WriteLine("No filename provided.");
                            return;
                        }
                        string? filename;
                        if (int.TryParse(userinput, out int fileIdx))
                        {
                            filename = files[fileIdx];
                        }
                        else
                        {
                            filename = Path.Combine(path, userinput);
                        }
                        argument = Path.Combine(path, filename);
                    }
                    if (!File.Exists(argument))
                    {
                        Console.WriteLine($"File {argument} does not exist.");
                        return;
                    }

                    try
                    {
                        _loadedFile = new SavedGame(argument);
                        Console.WriteLine($"File {argument} was loaded successfully.");
                    } // try
                    catch (Exception ex)
                    {
                        Console.WriteLine("There was an error wile opening the file.");
                        Console.WriteLine(ex.Message);
                    } // catch 
                    return;
                case CMD_UNPACK:
                    if (_loadedFile is null)
                    {
                        Console.WriteLine("No file is loaded.");
                        return;
                    }
                    if (string.IsNullOrEmpty(argument))
                    {
                        Console.WriteLine($"Game path is set to '{Settings.GamePath}'");
                        Console.Write("Output file name: ");
                        argument = Console.ReadLine();
                        if (string.IsNullOrEmpty(argument)) return;
                        if (!string.IsNullOrEmpty(Settings.GamePath))
                        {
                            argument = Path.Combine(Settings.GamePath, argument);
                        }
                        if (File.Exists(argument))
                        {
                            Console.Write("File exists. Overwrite (Y/n)? ");
                            var answer = Console.ReadLine();
                            if (answer?.ToUpper() != "Y") return;
                            File.Delete(argument);
                        }
                    }
                    try
                    {
                        _loadedFile.SaveUnpacked(argument);
                        Console.WriteLine("The file was unpacked successfully");
                    } // try
                    catch (Exception ex)
                    {
                        Console.WriteLine("There was an error while saving the file.");
                        Console.WriteLine(ex.Message);
                    } // catch
                    return;
            } // switch
        } // RunCommand

        private static void DisplayCommands()
        {
            Console.WriteLine("Available commands are:");
            foreach (var item in commands)
            {
                Console.WriteLine($"{item.Key}:\t{item.Value}");
            } // foreach
        } // DisplayCommands

        private static string? GetCommand(out string? args)
        {
            args = null;
            Console.Write("Type in command: ");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) return null;

            var parts = input.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2) args = parts[1];
            var filtered = parts[0].Trim().ToUpper();

            var choices = commands
                .Where(item => item.Value.Contains(filtered) || item.Key == filtered)
                .ToArray();

            if (choices is null || choices.Length == 0)
            {
                Console.WriteLine("Command not found.");
                return null;
            }

            if (choices.Length > 1)
            {
                Console.WriteLine($"Your input '{input}' is ambiguous.\nThe following command found:");
                foreach (var item in choices)
                {
                    Console.WriteLine($"{item.Key}:\t{item.Value}");
                } // foreach
                return null;
            }
            return choices[0].Value;
        } // GetCommand
    } // class Program
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuneEdC
{
    internal class Settings
    {
        public const string INI_PARAMETER_GAME_PATH = "GAMEPATH";
        private static readonly string[] ALL_PARAMS = new string[]
        {
            INI_PARAMETER_GAME_PATH
        };
        

        public static void Load(string inifile)
        {
            var lines = File.ReadAllLines(inifile);
            foreach (var line in lines)
            {
                var parts = line.Split(new char[] { '=' }, 2);
                if (ALL_PARAMS.Contains(parts[0]) && 2 == parts.Length)
                {
                    SetPropertyValue(parts[0], parts[1]);
                } // 
            } // foreach line
        } // Load

        public static void Save(string inifile)
        {
            StringBuilder sb = new();
            sb.Append(INI_PARAMETER_GAME_PATH);
            sb.Append('=');
            sb.Append(GamePath);
            File.WriteAllText(inifile, sb.ToString());
        } // Save

        private static void SetPropertyValue(string propertyName, object? value)
        {
            switch(propertyName)
            {
                case INI_PARAMETER_GAME_PATH:
                    GamePath = value?.ToString();
                    break;
            } // swith;
        } // SetPropertyValue

        public static string? GamePath { get; set; }
    } // class Settings
} // namespace

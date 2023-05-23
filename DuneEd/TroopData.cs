using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuneEd
{
    public class TroopData : IGameObject, INotifyPropertyChanged
    {
        private const byte MAXSKILL = 0x5f;
        private const byte MAXMOTIVATION = 0x64;
        private readonly byte[] _record;
        public TroopData()
        {
            _record = new byte[RECORD_LENGTH];
        } // public constructor;

        public bool IsOffSietch => Values.IsOccupationOffsite(Job);
        public SietchData? SietchInfo => null;
        public IEnumerable<TroopData> TroopsInfo => new TroopData[] {this};
        public int TroopsCount { get => 1; }
        public int TotalPopulation { get => Population * 10; }
        public Values.GameObjectTypes GameObjectType => Values.GameObjectTypes.Troop;
        public string ObjectTitle => $"Troop {TroopId}";

        public const int RECORD_LENGTH = 0x1B; // 27 bytes

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } // OnPropertyChanged

        private bool m_modified;
        public bool IsModified
        {
            get => m_modified;
            private set
            {
                m_modified = value;
                OnPropertyChanged(nameof(IsModified));
            } // set
        } // IsModified

        public long Offset { get; private set; }

        public byte TroopId { get => _record[0]; }
        public byte NextTroopId { get => _record[1]; }
        public byte Location { get => _record[2]; }
        public byte Job 
        {
            get => _record[3];
            set
            {
                _record[3] = value;
                OnPropertyChanged(nameof(Job));
                IsModified = true;
            } // set
        } // Job
        // +2 unknown bytes (3+2) = 5
        public short Xcoord => BitConverter.ToInt16(_record, 6); // 2 bytes
        public short Ycoord => BitConverter.ToInt16(_record, 8); // 2 bytes
        // +8 unknown bytes (10 + 8) = 18
        public byte Dissatisfaction
        {
            get => _record[18];
            set
            {
                _record[18] = value;
                OnPropertyChanged(nameof(Dissatisfaction));
                IsModified = true;
            } // set
        } // Dissatisfaction
        public byte SpeechByte
        {
            get => _record[19];
            set
            {
                _record[19] = value;
                IsModified = true;
            } // set
        } // SpeechByte
        public byte MissedDays
        {
            get => _record[20];
            set
            {
                _record[20] = value;
                OnPropertyChanged(nameof(MissedDays));
                IsModified = true;
            } // set
        } // MissedDays
        public byte Motivation
        {
            get => _record[21];
            set
            {
                if (value > MAXMOTIVATION) throw new ArgumentException($"Motivation must be less than or equal to {MAXSKILL}");
                _record[21] = value;
                OnPropertyChanged(nameof(Motivation));
                IsModified = true;
            } // set
        } // Motivation
        public byte SpiceSkill
        {
            get => _record[22];
            set {
                if (value > MAXSKILL) throw new ArgumentException($"Skill must be less than or equal to {MAXSKILL}");
                _record[22] = value;
                OnPropertyChanged(nameof(SpiceSkill));
                IsModified = true;
            } // set
        } // SpiceSkill
        public byte ArmySkill
        {
            get => _record[23];
            set {
                if (value > MAXSKILL) throw new ArgumentException($"Skill must be less than or equal to {MAXSKILL}");
                _record[23] = value;
                OnPropertyChanged(nameof(ArmySkill));
                IsModified = true;
            } // set
        } // ArmySkill
        public byte EcologySkill
        {
            get => _record[24];
            set {
                if (value > MAXSKILL) throw new ArgumentException($"Skill must be less than or equal to {MAXSKILL}");
                _record[24] = value;
                OnPropertyChanged(nameof(EcologySkill));
                IsModified = true;
            } // set
        } // EcologySkill
        public byte EquipmentByte 
        { 
            get => _record[25];
            set {
                _record[25] = value;
                OnPropertyChanged(nameof(EquipmentByte));
                IsModified = true;
            } // set
        } // EquipmentByte 
        public byte Population
        {
            get => _record[26];
            set {
                _record[26] = value;
                OnPropertyChanged(nameof(Population));
                IsModified = true;
            } // set
        } // Population

        public string JobDescription { get => Values.GetOccupationString(Job); }
        public string LocationDescription { get => Values.GetLocation(Location); }

        public bool HasHarvester 
        {
            get => (EquipmentByte & (byte)Values.Equipment.SpiceHarvester) == (byte)Values.Equipment.SpiceHarvester;
            set
            {
                var oldValue = (EquipmentByte & (byte)Values.Equipment.SpiceHarvester) == (byte)Values.Equipment.SpiceHarvester;
                IsModified = value != oldValue;
                if (IsModified) OnPropertyChanged(nameof(HasHarvester));
                if (value)
                {
                    EquipmentByte |= (byte)Values.Equipment.SpiceHarvester;
                    return;
                }
                EquipmentByte &= ~(byte)Values.Equipment.SpiceHarvester & 0xff;
            } // set
        } // HasHarvester
        public bool HasOrni
        {
            get => (EquipmentByte & (byte)Values.Equipment.Ornithopter) == (byte)Values.Equipment.Ornithopter;
            set
            {
                var oldValue = (EquipmentByte & (byte)Values.Equipment.Ornithopter) == (byte)Values.Equipment.Ornithopter;
                IsModified = value != oldValue;
                if (IsModified) OnPropertyChanged(nameof(HasOrni));
                if (value)
                {
                    EquipmentByte |= (byte)Values.Equipment.Ornithopter;
                    return;
                }
                EquipmentByte &= ~(byte)Values.Equipment.Ornithopter & 0xff;
            } // set
        } // HasHarvester
        public bool HasKnives
        {
            get => (EquipmentByte & (byte)Values.Equipment.KrysKnives) == (byte)Values.Equipment.KrysKnives;
            set
            {
                var oldValue = (EquipmentByte & (byte)Values.Equipment.KrysKnives) == (byte)Values.Equipment.KrysKnives;
                IsModified = value != oldValue;
                if (IsModified) OnPropertyChanged(nameof(HasKnives));
                if (value)
                {
                    EquipmentByte |= (byte)Values.Equipment.KrysKnives;
                    return;
                }
                EquipmentByte &= ~(byte)Values.Equipment.KrysKnives & 0xff;
            } // set
        } // HasKnives
        public bool HasLasGuns
        {
            get => (EquipmentByte & (byte)Values.Equipment.LaserGuns) == (byte)Values.Equipment.LaserGuns;
            set
            {
                var oldValue = (EquipmentByte & (byte)Values.Equipment.LaserGuns) == (byte)Values.Equipment.LaserGuns;
                IsModified = value != oldValue;
                if (IsModified) OnPropertyChanged(nameof(HasLasGuns));
                if (value)
                {
                    EquipmentByte |= (byte)Values.Equipment.LaserGuns;
                    return;
                }
                EquipmentByte &= ~(byte)Values.Equipment.LaserGuns & 0xff;
            } // set
        } // HasLasGuns
        public bool HasWeirdingModules
        {
            get => (EquipmentByte & (byte)Values.Equipment.WeirdingModules) == (byte)Values.Equipment.WeirdingModules;
            set
            {
                var oldValue = (EquipmentByte & (byte)Values.Equipment.WeirdingModules) == (byte)Values.Equipment.WeirdingModules;
                IsModified = value != oldValue;
                if (IsModified) OnPropertyChanged(nameof(HasWeirdingModules));
                if (value)
                {
                    EquipmentByte |= (byte)Values.Equipment.WeirdingModules;
                    return;
                }
                EquipmentByte &= ~(byte)Values.Equipment.WeirdingModules & 0xff;
            } // set
        } // HasWeirdingModules
        public bool HasAtomics
        {
            get => (EquipmentByte & (byte)Values.Equipment.Atomics) == (byte)Values.Equipment.Atomics;
            set
            {
                var oldValue = (EquipmentByte & (byte)Values.Equipment.Atomics) == (byte)Values.Equipment.Atomics;
                IsModified = value != oldValue;
                if (IsModified) OnPropertyChanged(nameof(HasAtomics));
                if (value)
                {
                    EquipmentByte |= (byte)Values.Equipment.Atomics;
                    return;
                }
                EquipmentByte &= ~(byte)Values.Equipment.Atomics & 0xff;
            } // set
        } // HasWeirdingModules
        public bool HasBulbs
        {
            get => (EquipmentByte & (byte)Values.Equipment.Bulbs) == (byte)Values.Equipment.Bulbs;
            set
            {
                var oldValue = (EquipmentByte & (byte)Values.Equipment.Bulbs) == (byte)Values.Equipment.Bulbs;
                IsModified = value != oldValue;
                if (IsModified) OnPropertyChanged(nameof(HasBulbs));
                if (value)
                {
                    EquipmentByte |= (byte)Values.Equipment.Bulbs;
                    return;
                } // set
                EquipmentByte &= ~(byte)Values.Equipment.Bulbs & 0xff;
            }
        } // HasBulbs

        public byte[] GetRecord() => _record;

        public static TroopData ReadFrom(BinaryReader reader)
        {
            TroopData data = new() { Offset = reader.BaseStream.Position };
            int bytesRead =
                reader.Read(data._record, 0, RECORD_LENGTH);
            if (RECORD_LENGTH != bytesRead)
                throw new InvalidOperationException("Unexpected data length");
            return data;
        } // ReadFrom
    } // class TroopData
} // namespace

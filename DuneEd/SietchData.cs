using System.ComponentModel;

namespace DuneEd
{

    public class SietchData : IGameObject, INotifyPropertyChanged
    {
        private readonly SavedGame _game;
        protected SietchData(SavedGame ownergame) {
            _record = new byte[RECORD_LENGTH];
            _game = ownergame;
        } // public constructor

        public Values.GameObjectTypes GameObjectType => Values.GameObjectTypes.Sietch;
        public string ObjectTitle => Name;
        public SietchData? SietchInfo => this;
        public IEnumerable<TroopData> TroopsInfo => BuildTroopsList();
        public int TroopsCount { get => TroopsInfo.Count(); }
        public int TotalPopulation { get => TroopsInfo.Sum(t => t.Population * 10); }

        private IEnumerable<TroopData> BuildTroopsList()
        {
            var noTroops = Enumerable.Empty<TroopData>();
            if (0 == HostedTroopId || !_game.Troops.Any()) return noTroops;
            var currentTroop = _game.Troops.Where(t => HostedTroopId == t.TroopId).FirstOrDefault();
            if (currentTroop is null) return noTroops;
            var alltroops = new List<TroopData>() { currentTroop };
            while (currentTroop.NextTroopId != 0)
            {
                currentTroop = _game.Troops.Where(t => t.TroopId == currentTroop.NextTroopId).FirstOrDefault();
                if (currentTroop is null) break;
                alltroops.Add(currentTroop);
            } // while
            return alltroops;
        } // BuildTroopsList

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } // OnPropertyChanged


        private readonly byte[] _record;
        public long Offset { get; private set; }
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

        private byte FirstNameByte => _record[0];
        private byte LastNameByte => _record[1];
        public string FirstName => Values.FirstNames[FirstNameByte];
        public string LastName => Values.LastNames[LastNameByte];
        public string Name => FirstName + LastName;
        
        public short Xcoord => BitConverter.ToInt16(_record, 2);
        public short Ycoord => BitConverter.ToInt16(_record, 4);
        public byte InteriorId 
        { 
            get => _record[8];
            set
            {
                var oldValue = _record[8];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(InteriorId));
                _record[8] = value;
            } // set
        } // InteriorId
        public byte HostedTroopId => _record[9];
        public byte StatusByte
        { 
            get => _record [10];
            set
            {
                var oldValue = _record[10];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(StatusByte));
                _record[10] = value;
            } // set
        } // StatusByte
        public byte SpiceFieldId => _record[16];
        public byte SpiceDensity 
        { 
            get => _record[18];
            set {
                var oldValue = _record[18];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(SpiceDensity));
                _record[18] = value;
            } // set
        } // SpiceDensity
        public byte Harvesters 
        {
            get => _record[20];
            set {
                var oldValue = _record[20];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(Harvesters));
                _record[20] = value;
            } // set
        } // Harvesters
        public byte Ornithopters
        {
            get => _record[21];
            set {
                var oldValue = _record[21];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(Ornithopters));
                _record[21] = value;
            } // set
        } // Ornithopters
        public byte Knives
        {
            get => _record[22];
            set {
                var oldValue = _record[22];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(Knives));
                _record[22] = value;
            } // set
        } // Knives
        public byte Lasguns
        {
            get => _record[23];
            set {
                var oldValue = _record[23];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(Lasguns));
                _record[23] = value;
            } // set
        } // Lasguns
        public byte WeirdingModules
        {
            get => _record[24];
            set {
                var oldValue = _record[24];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(WeirdingModules));
                _record[24] = value;
            } // set
        } // WeirdingModules
        public byte Atomics
        {
            get => _record[25];
            set {
                var oldValue = _record[25];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(Atomics));
                _record[25] = value;
            } // set
        } // Atomics
        public byte Bulbs
        {
            get => _record[26];
            set {
                var oldValue = _record[26];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(Bulbs));
                _record[26] = value;
            } // set
        } // Bulbs
        public byte Water
        {
            get => _record[27];
            set {
                var oldValue = _record[27];
                IsModified = oldValue != value;
                if (!IsModified) return;
                OnPropertyChanged(nameof(Water));
                _record[27] = value;
            } // set
        } // Water

        public bool IsUndiscovered
        {
            get 
            =>
                (StatusByte & (byte)Values.SietchStatus.NotDiscovered) == (byte)Values.SietchStatus.NotDiscovered;
        } // IsUndiscovered

        public bool IsProspected
        {
            get =>
                (StatusByte & (byte)Values.SietchStatus.AreaProspected) == (byte)Values.SietchStatus.AreaProspected;
            set
            {
                var currentStatus = (StatusByte & (byte)Values.SietchStatus.AreaProspected) == (byte)Values.SietchStatus.AreaProspected;
                IsModified = (value != currentStatus);
                if (!IsModified) return;
                OnPropertyChanged(nameof(IsProspected));
                if (value)
                {
                    StatusByte |= (byte)Values.SietchStatus.AreaProspected;
                    return;
                }
                StatusByte &= ~(byte)Values.SietchStatus.AreaProspected & 0xff;
            } // set
        } // IsProspected

        public bool IsWindTrapConstucted
        {
            get
            =>
                (StatusByte & (byte)Values.SietchStatus.WindtrapPresent) == (byte)Values.SietchStatus.WindtrapPresent;
            set
            {
                var currentStatus = (StatusByte & (byte)Values.SietchStatus.WindtrapPresent) == (byte)Values.SietchStatus.WindtrapPresent;
                IsModified = (value != currentStatus);
                if (!IsModified) return;
                OnPropertyChanged(nameof(IsWindTrapConstucted));
                if (value)
                {
                    StatusByte |= (byte)Values.SietchStatus.WindtrapPresent;
                    return;
                }
                StatusByte &= ~(byte)Values.SietchStatus.WindtrapPresent & 0xff;
            } // set
        } // IsWindTrapConstucted

        public bool IsUndiscoveredWindtrap
        {
            get
            =>
                (StatusByte & (byte)Values.SietchStatus.UndiscoveredWindtrap) == (byte)Values.SietchStatus.UndiscoveredWindtrap;
        } // IsUndiscoveredWindtrap


        public bool IsVegetationPresent
        {
            get =>
                (StatusByte & (byte)Values.SietchStatus.VegetationPresent) == (byte)Values.SietchStatus.VegetationPresent;
        } // IsVegetationPresent

        public bool IsInventoryVisible
        {
            get =>
                (StatusByte & (byte)Values.SietchStatus.InventoryVisible) == (byte)Values.SietchStatus.InventoryVisible;
            set
            {
                var currentStatus = (StatusByte & (byte)Values.SietchStatus.InventoryVisible) == (byte)Values.SietchStatus.InventoryVisible;
                IsModified = (value != currentStatus);
                if (!IsModified) return;
                OnPropertyChanged(nameof(IsInventoryVisible));
                if (value)
                {
                    StatusByte |= (byte)Values.SietchStatus.InventoryVisible;
                    return;
                }
                StatusByte &= ~(byte)Values.SietchStatus.InventoryVisible & 0xff;
            } // set
        } // IsInventoryVisible

        public bool TuonoTabrInitial
        {
            get =>
                (StatusByte & (byte)Values.SietchStatus.TuonoTabrInitial) == (byte)Values.SietchStatus.TuonoTabrInitial;
        } // TuonoTabrInitial

        public bool IsUnknownAStatus
        {
            get =>
                (StatusByte & (byte)Values.SietchStatus.UnknownA) == (byte)Values.SietchStatus.UnknownA;
        } // IsUnknownAStatus

        public bool IsUnknownBStatus
        {
            get =>
                (StatusByte & (byte)Values.SietchStatus.UnknownB) == (byte)Values.SietchStatus.UnknownB;
        } // IsUnknownBStatus

        public bool IsInBattle
        {
            get =>
                (StatusByte & (byte)Values.SietchStatus.InBattle) == (byte)Values.SietchStatus.InBattle;
        } // IsInBattle

        /*Visible = 0x00,
            VegetationPresent = 0x01,
            InBattle = 0x02,
            UnknownA = 0x04,
            UnknownB = 0x08,
            InventoryVisible = 0x10,
            WindtrapPresent = 0x20,
            AreaProspected = 0x40,
            NotDiscovered = 0x80,
            UndiscoveredWindtrap = 0xA0,
            TuonoTabrInitial = 0xE0 */

        public const int RECORD_LENGTH = 0x1C; // 28 bytes

        public byte[] GetRecord() => _record;

        public static SietchData ReadFrom(SavedGame game, BinaryReader reader)
        {
            SietchData data = new(game) { Offset = reader.BaseStream.Position };
            int bytesRead =
                reader.Read(data._record, 0, RECORD_LENGTH);
            if (RECORD_LENGTH != bytesRead)
                throw new InvalidOperationException("Unexpected data length");
            return data;
        } // ReadFrom

    } // class SietchData
} // namespace

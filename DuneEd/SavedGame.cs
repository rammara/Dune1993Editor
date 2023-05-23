using System.Collections.ObjectModel;

namespace DuneEd
{
    public class SavedGame
    {
        const byte REPEAT_BYTE = 0xf7;
        private byte[] _contents;
        private byte[] _unpacked;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SavedGame(string filename)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            if (filename is null) throw new ArgumentNullException(nameof(filename));
            Filename = filename;
            ReadFile();
        } // ctor

        private void ReadFile()
        {
            _contents = File.ReadAllBytes(Filename);
            _unpacked = Unpack();
            Scan();
        } // ReadFile

        public bool IsModified 
        {
            get => Troops.Where(t => t.IsModified).Any() || Sietches.Where(s => s.IsModified).Any();
        }

        public string Filename { get; init; }

        private byte[] Unpack()
        {
            var mem = new MemoryStream();
            using var wrt = new BinaryWriter(mem);

            var pos = 0;
            while (pos < _contents.Length)
            {
                var currentByte = _contents[pos];
                if (REPEAT_BYTE == currentByte && pos > 4)
                {
                    pos++;
                    var RepeatCount = _contents[pos];
                    pos++;
                    var RepeatValue = _contents[pos];

                    // Possible exception for 0xF70200
                    if (2 == RepeatCount && RepeatValue == 00)
                    {

                    }

                    for (int iteration = 0; iteration < RepeatCount; ++iteration)
                    {
                        wrt.Write(RepeatValue);
                    } // for
                    pos++;
                    continue;
                }
                wrt.Write(currentByte);
                pos++;
            } // while
            return mem.ToArray();
        } // Unpack

        private byte[] Pack()
        {
            var mem = new MemoryStream();
            using var wrt = new BinaryWriter(mem);

            var pos = 1;
            byte Prev = _unpacked[0];
            byte Current = 0;
            int count = 1;
            while (pos < _unpacked.Length)
            {
                Current = _unpacked[pos];
                pos++;
                if (Prev == Current)
                {
                    count++;
                    Prev = Current;
                    continue;
                }
                if (REPEAT_BYTE == Prev && pos > 4)
                {
                    wrt.Write(REPEAT_BYTE);
                    wrt.Write((byte)1);
                    wrt.Write(REPEAT_BYTE);
                    Prev = Current;
                    continue;
                }
                if (Prev != Current)
                {
                    if (2 == count && 0xeb != Prev)
                    {
                        wrt.Write(Prev);
                        wrt.Write(Prev);
                        Prev = Current;
                        count = 1;
                        continue;
                    }
                    if (count > 1)
                    {
                        while (count > 0xff)
                        {
                            wrt.Write(REPEAT_BYTE);
                            wrt.Write((byte)0xff);
                            wrt.Write(Prev);
                            count -= 0xff;
                        }
                        wrt.Write(REPEAT_BYTE);
                        wrt.Write((byte)count);
                        wrt.Write(Prev);
                        count = 1;
                        Prev = Current;
                        continue;
                    }
                    wrt.Write(Prev);
                }
                Prev = Current;
            } // while
            wrt.Write(Prev);
            wrt.Write(Current);
            
            wrt.Flush();
            return mem.ToArray();
        } // Pack   

        private void ApplyChanges()
        {
            foreach(var sietch in Sietches.Where(s => s.IsModified))
            {
                Array.ConstrainedCopy(sietch.GetRecord(), 0, _unpacked, (int)sietch.Offset, SietchData.RECORD_LENGTH);
            } // foreach sietch
            foreach(var troop in Troops.Where(t => t.IsModified))
            {
                Array.ConstrainedCopy(troop.GetRecord(), 0, _unpacked, (int)troop.Offset, TroopData.RECORD_LENGTH);
            } // foreach troop
        } // ApplyChanges

        public void SaveUnpacked(string filename)
        {
            ApplyChanges();
            File.WriteAllBytes(filename, _unpacked);
        } // SaveUnpacked
        public void SavePacked(string filename)
        {
            ApplyChanges();
            _contents = Pack();
            File.WriteAllBytes(filename, _contents);
        } // SavePacked

        public ObservableCollection<SietchData> Sietches { get; private set; } = new();
        public ObservableCollection<TroopData> Troops { get; private set; } = new();

        IEnumerable<IGameObject> m_AllLocatableObjects;
        public IEnumerable<IGameObject> GetLocatableObjects()
        {
            if (m_AllLocatableObjects is null)
            {
                var troopsOffSietch = Troops?.Where(t => t.IsOffSietch);
                m_AllLocatableObjects = (troopsOffSietch is not null && troopsOffSietch.Any()) ?
                    Enumerable.Union<IGameObject>(troopsOffSietch, Sietches) :
                    Sietches;
            }
            return m_AllLocatableObjects;
        } // GetLocatableObject

        public void Scan()
        {
            var DATA_LENGTH = Values.SietchesStartPattern.Length;
            var firstByte = Values.SietchesStartPattern[0];

            int offset = Array.IndexOf<byte>(_unpacked, firstByte, 0);
            while (offset >= 0 && offset <= _unpacked.Length - DATA_LENGTH)
            {
                byte[] segment = new byte[DATA_LENGTH];
                Buffer.BlockCopy(_unpacked, offset, segment, 0, DATA_LENGTH);
                if (segment.SequenceEqual<byte>(Values.SietchesStartPattern))
                    break;
                offset = Array.IndexOf<byte>(_unpacked, firstByte, offset + 1);
            }

            
            var mem = new MemoryStream(_unpacked);
            var reader = new BinaryReader(mem);
            reader.BaseStream.Position = offset;
            if (Sietches.Any()) Sietches.Clear();
            for (int sietchIdx = 0; sietchIdx < Values.SIETCHES_COUNT; sietchIdx++)
            {
                var currentSietch = SietchData.ReadFrom(this, reader);
                Sietches.Add(currentSietch);
            } // for
            if (Troops.Any()) Troops.Clear();
            _ = reader.ReadUInt16(); // skip 2 bytes
            while(true)
            {
                var currentTroop = TroopData.ReadFrom(reader);
                var TroopId = currentTroop.TroopId;
                if (TroopId > 0)
                {
                    Troops.Add(currentTroop);
                    continue;
                }
                break;
            }
        } // Scan
        

    } // class SaveFile
} // namespace

namespace DuneEd
{
    public interface IGameObject : ILocation
    {
        Values.GameObjectTypes GameObjectType { get; }
        string ObjectTitle { get; }
        SietchData? SietchInfo { get; }
        IEnumerable<TroopData> TroopsInfo { get; }
        int TroopsCount { get; }
        int TotalPopulation { get; }
    } // intarface IGameObject 
} // namespace

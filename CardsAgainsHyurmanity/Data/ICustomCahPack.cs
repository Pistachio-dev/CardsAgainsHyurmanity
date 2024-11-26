namespace CardsAgainsHyurmanity.Data
{
    internal interface ICustomCahPack
    {
        string Name { get; }
        string Description { get; }
        bool Official { get; }
        string[] White { get; }
        string[] Black { get; }
    }
}

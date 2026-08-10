namespace RestaurantApp.Options;

public class PersistenceOptions
{
    public const string SectionName = "Persistence";

    public string Provider { get; set; } = "Json";

    public string JsonDataPath { get; set; } = "App_Data/restaurant-data.json";
}

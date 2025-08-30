using SimHub.Plugins;

public class PropertyManager : IPropertyManager
{
    private static readonly PropertyManager _instance = new PropertyManager();

    private PropertyManager()
    {
        AddAll();
    }

    public static PropertyManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public void Add<T>(string key, T value)
    {
        AddInPluginManager(PropertyManagerConstants.PREFIX + key, value);
    }

    public void Add<T>(int pos, string key, T value)
    {
        Add("Driver" + pos + "." + key, value);
    }

    public virtual void AddInPluginManager<T>(string key, T value)
    {
        PluginManager.GetInstance().AddProperty(key, GetType(), value);
    }

    public void ResetAll()
    {
        SimHub.Logging.Current.Info($"PropertyManager:ResetAll()");
        AddAll();
    }

    private void AddAll()
    {
        for (int i = 1; i <= 50; i++)
        {
            Add(i, PropertyManagerConstants.POSITION, "");
            Add(i, PropertyManagerConstants.IS_PLAYER, false);
            Add(i, PropertyManagerConstants.CAR_NUMBER, "");
            Add(i, PropertyManagerConstants.DISPLAY_NAME, "");
            Add(i, PropertyManagerConstants.CAR_CLASS, "");
            Add(i, PropertyManagerConstants.MANUFACTURER, "");
            Add(i, PropertyManagerConstants.CAR_MODEL, "");
            Add(i, PropertyManagerConstants.CURRENT_LAP_NUMBER, 0);
            Add(i, PropertyManagerConstants.CURRENT_SECTOR, 0);
            Add(i, PropertyManagerConstants.GAP_TO_LEADER, "");
            Add(i, PropertyManagerConstants.GAP_TO_CLASS_LEADER, "");
            Add(i, PropertyManagerConstants.GAP_TO_IN_FRONT, "");
            Add(i, PropertyManagerConstants.SECTOR_1_TIME, "");
            Add(i, PropertyManagerConstants.SECTOR_1_PACE_INDICATOR, "");
            Add(i, PropertyManagerConstants.SECTOR_2_TIME, "");
            Add(i, PropertyManagerConstants.SECTOR_2_PACE_INDICATOR, "");
            Add(i, PropertyManagerConstants.SECTOR_3_TIME, "");
            Add(i, PropertyManagerConstants.SECTOR_3_PACE_INDICATOR, "");
            Add(i, PropertyManagerConstants.LAP_TIME, "");
            Add(i, PropertyManagerConstants.LAP_TIME_PACE_INDICATOR, "");
            Add(i, PropertyManagerConstants.BEST_LAP_TIME, "");
            Add(i, PropertyManagerConstants.BEST_LAP_TIME_PACE_INDICATOR, "");
            Add(i, PropertyManagerConstants.IN_PITS, false);
            Add(i, PropertyManagerConstants.PIT_STOPS_TOTAL, 0);
            Add(i, PropertyManagerConstants.PIT_STOPS_TOTAL_DURATION, 0);
            Add(i, PropertyManagerConstants.PIT_STOPS_LAST_DURATION, 0);
            Add(i, PropertyManagerConstants.LAPS_IN_CURRENT_STINT, 0);
            Add(i, PropertyManagerConstants.TYRE_COMPOUND, "");
            Add(i, PropertyManagerConstants.FUEL_CAPACITY, "");
            Add(i, PropertyManagerConstants.FUEL_LOAD, "");
        }
    }
}
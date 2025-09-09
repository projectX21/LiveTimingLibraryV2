using SimHub.Plugins;
using System.Linq;
using System.Collections.Generic;
using GameReaderCommon;
using System.Security.Cryptography;
using System;

public class PropertyManager : IPropertyManager
{
    private static readonly PropertyManager _instance = new PropertyManager();
    private static readonly int maxEntries = 70;

    private PropertyManager()
    {
        AddAllProperties();
        AddActions();
    }

    public static PropertyManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public object GetPropertyValue(string key)
    {
        return PluginManager.GetInstance().GetPropertyValue(GetType() + "." + PropertyManagerConstants.PREFIX + key);
    }

    public object GetPropertyValue(int pos, string key)
    {
        return GetPropertyValue("Driver" + pos + "." + key);
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
        AddAllProperties();
    }

    private void AddActions()
    {
        PluginManager.GetInstance().AddAction("CHB.FlexDisplayScrollUp", GetType(), (_a, _b) =>
        {
            var value = GetPropertyValue(PropertyManagerConstants.FLEX_OFFSET) ?? 0;
            SimHub.Logging.Current.Info($"PropertyManager::ScrollUp(): new value {(int)value + 1}");
            Add(PropertyManagerConstants.FLEX_OFFSET, (int)value + 1);
        });

        PluginManager.GetInstance().AddAction("CHB.FlexDisplayScrollDown", GetType(), (_a, _b) =>
        {
            var value = GetPropertyValue(PropertyManagerConstants.FLEX_OFFSET) ?? 0;
            SimHub.Logging.Current.Info($"PropertyManager::ScrollDown(): new value {(int)value - 1}");
            Add(PropertyManagerConstants.FLEX_OFFSET, (int)value - 1);
        });

        PluginManager.GetInstance().AddAction("CHB.IterateCarClassHighlight", GetType(), (_a, _b) =>
        {
            var value = (string)GetPropertyValue(PropertyManagerConstants.HIGHLIGHT_CAR_CLASS);
            SimHub.Logging.Current.Info($"PropertyManager::IterateCarClassHighlight(): current value: {value}");
            var carClasses = GetPossibleCarClassesFromProperties();
            SimHub.Logging.Current.Info($"PropertyManager::IterateCarClassHighlight(): possible car classes: {string.Join(", ", carClasses)}");
            string nextValue = null;

            if (carClasses.Count > 0)
            {
                var index = carClasses.IndexOf(value);
                SimHub.Logging.Current.Info($"PropertyManager::IterateCarClassHighlight(): index: {index}");
                nextValue = carClasses.ElementAtOrDefault(index + 1);
                SimHub.Logging.Current.Info($"PropertyManager::IterateCarClassHighlight(): next value: {nextValue}");
            }

            SimHub.Logging.Current.Info($"PropertyManager::IterateCarClassHighlight(): set new value: {nextValue}");
            Add(PropertyManagerConstants.HIGHLIGHT_CAR_CLASS, nextValue);
        });
    }

    private void AddAllProperties()
    {
        Add<int?>(PropertyManagerConstants.FLEX_OFFSET, null);
        Add<string>(PropertyManagerConstants.HIGHLIGHT_CAR_CLASS, null);
        Add<string>(PropertyManagerConstants.SESSION_TYPE, null);
        Add<int?>(PropertyManagerConstants.ENTRY_COUNT, null);
        Add<int?>(PropertyManagerConstants.CURRENT_LAP_NUMBER, null);
        Add<TimeSpan?>(PropertyManagerConstants.CURRENT_LAP_TIME, null);

        for (int i = 1; i <= maxEntries; i++)
        {
            Add<int?>(i, PropertyManagerConstants.POSITION, null);
            Add<bool?>(i, PropertyManagerConstants.IS_PLAYER, null);
            Add<string>(i, PropertyManagerConstants.CAR_NUMBER, null);
            Add<string>(i, PropertyManagerConstants.DISPLAY_NAME, null);
            Add<string>(i, PropertyManagerConstants.CAR_CLASS, null);
            Add<string>(i, PropertyManagerConstants.MANUFACTURER, null);
            Add<string>(i, PropertyManagerConstants.CAR_MODEL, null);
            Add<int?>(i, PropertyManagerConstants.CURRENT_SECTOR, null);
            Add<string>(i, PropertyManagerConstants.GAP_TO_LEADER, null);
            Add<string>(i, PropertyManagerConstants.GAP_TO_CLASS_LEADER, null);
            Add<string>(i, PropertyManagerConstants.GAP_TO_IN_FRONT, null);
            Add<string>(i, PropertyManagerConstants.SECTOR_1_TIME, null);
            Add<string>(i, PropertyManagerConstants.SECTOR_1_PACE_INDICATOR, null);
            Add<string>(i, PropertyManagerConstants.SECTOR_2_TIME, null);
            Add<string>(i, PropertyManagerConstants.SECTOR_2_PACE_INDICATOR, null);
            Add<string>(i, PropertyManagerConstants.SECTOR_3_TIME, null);
            Add<string>(i, PropertyManagerConstants.SECTOR_3_PACE_INDICATOR, null);
            Add<string>(i, PropertyManagerConstants.LAP_TIME, null);
            Add<string>(i, PropertyManagerConstants.LAP_TIME_PACE_INDICATOR, null);
            Add<string>(i, PropertyManagerConstants.BEST_LAP_TIME, null);
            Add<string>(i, PropertyManagerConstants.BEST_LAP_TIME_PACE_INDICATOR, null);
            Add<bool?>(i, PropertyManagerConstants.IN_PITS, null);
            Add<int?>(i, PropertyManagerConstants.PIT_STOPS_TOTAL, null);
            Add<string>(i, PropertyManagerConstants.PIT_STOPS_TOTAL_DURATION, null);
            Add<string>(i, PropertyManagerConstants.PIT_STOPS_LAST_DURATION, null);
            Add<string>(i, PropertyManagerConstants.PIT_STOPS_CURRENT_DURATION, null);
            Add<int?>(i, PropertyManagerConstants.LAPS_IN_CURRENT_STINT, null);
            Add<string>(i, PropertyManagerConstants.TYRE_COMPOUND, null);
            Add<double?>(i, PropertyManagerConstants.FUEL_CAPACITY, null);
            Add<double?>(i, PropertyManagerConstants.FUEL_LOAD, null);
        }
    }

    private List<string> GetPossibleCarClassesFromProperties()
    {
        return Enumerable.Range(1, maxEntries)
            .Select(i => (string)GetPropertyValue(i, PropertyManagerConstants.CAR_CLASS))
            .Distinct()
            .Where(i => i != null)
            .ToList();
    }
}
using System.Linq;
using AcTools.Utils.Helpers;

public class LiveTimingPluginProcessor
{
    private readonly IRaceEntryProcessor _raceEntryProcessor;

    private readonly ISessionDataProvider _sessionDataProvider;

    public LiveTimingPluginProcessor(
        ISessionDataProvider sessionDataProvider,
        IRaceEntryProcessor raceEntryProcessor
    )
    {
        _raceEntryProcessor = raceEntryProcessor;
        _sessionDataProvider = sessionDataProvider;
    }

    public void DataUpdate(SessionData data)
    {
        if (data == null || !data.IsValid())
        {
            return;
        }

        SessionData mergedData = _sessionDataProvider.MergeWithPreviousData(data);
        SetSessionProperties(mergedData);
        mergedData.Entries.ForEach(entry => _raceEntryProcessor.Process(entry));

        //mergedData.Log(true);
    }

    private void SetSessionProperties(SessionData mergedData)
    {
        PropertyManager.Instance.Add(PropertyManagerConstants.SESSION_TYPE, mergedData.SessionType.ToString());
        PropertyManager.Instance.Add(PropertyManagerConstants.ENTRY_COUNT, mergedData.Entries.Count);
        PropertyManager.Instance.Add(PropertyManagerConstants.CURRENT_LAP_NUMBER, mergedData.PlayerCurrentLapNumber);
        PropertyManager.Instance.Add(PropertyManagerConstants.CURRENT_LAP_TIME, mergedData.PlayerCurrentLapTime);
    }
}
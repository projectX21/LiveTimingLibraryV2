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
        if (data == null || !data.IsValid)
        {
            return;
        }

        SessionData mergedData = _sessionDataProvider.MergeWithPreviousData(data);
        PropertyManager.Instance.Add(PropertyManagerConstants.SESSION_TYPE, mergedData.SessionType.ToString());
        mergedData.Entries.ForEach(entry => _raceEntryProcessor.Process(entry));
        mergedData.Entries.Where(e => e.CarNumber == "70").ForEach(entry => entry.CustomLog());
    }
}
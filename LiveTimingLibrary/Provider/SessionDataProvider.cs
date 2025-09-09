using System.Collections.Generic;
using System.Linq;
using AcTools.Utils.Helpers;

public class SessionDataProvider : ISessionDataProvider
{
    private readonly ISessionDataRecovery _sessionDataRecovery;

    public SessionDataProvider(ISessionDataRecovery sessionDataRecovery)
    {
        _sessionDataRecovery = sessionDataRecovery;
    }

    public SessionData MergeWithPreviousData(SessionData newData)
    {
        var recoveredData = _sessionDataRecovery.GetRecoveryData();
        var mergedData = Merge(GetOldData(recoveredData, newData), newData);
        _sessionDataRecovery.SaveSessionData(mergedData);
        return mergedData;
    }

    private SessionData GetOldData(SessionData recoveredData, SessionData newData)
    {
        if (HasSessionIdChanged(recoveredData, newData))
        {
            SimHub.Logging.Current.Info($"SessionDataProvider::GetOldData(): session change detected: {recoveredData?.SessionId} to {newData.SessionId}");
            _sessionDataRecovery.Clear();
            PropertyManager.Instance.ResetAll();
            PropertyManager.Instance.Add(PropertyManagerConstants.FLEX_OFFSET, 0);

            return null;
        }
        else if (WasRaceSessionReloaded(recoveredData, newData))
        {
            SimHub.Logging.Current.Info($"SessionDataProvider::GetOldData(): session reload detected");
            SimHub.Logging.Current.Info($"SessionDataProvider::GetOldData(): Recovered: {recoveredData.PlayerCurrentLapNumber}, {recoveredData.PlayerCurrentLapTime}, new: {newData.PlayerCurrentLapNumber}, {newData.PlayerCurrentLapTime}");
            PropertyManager.Instance.ResetAll();

            // Event must be fired _before_ creating the recovery data
            _sessionDataRecovery.SaveRaceEvent(new SessionReloadEvent(newData.SessionId, newData.PlayerCurrentLapNumber, newData.PlayerCurrentLapTime));
            return _sessionDataRecovery.CreateBasicSessionDataFromRaceEventRecovery();
        }

        return recoveredData;
    }

    private bool HasSessionIdChanged(SessionData recoveredData, SessionData newData)
    {
        return recoveredData == null || recoveredData.SessionId != newData.SessionId;
    }

    private bool WasRaceSessionReloaded(SessionData recoveredData, SessionData newData)
    {
        return recoveredData != null && newData != null && newData.SessionType == SessionType.Race &&
            (
                recoveredData.PlayerCurrentLapNumber > newData.PlayerCurrentLapNumber ||
                (recoveredData.PlayerCurrentLapNumber == newData.PlayerCurrentLapNumber && recoveredData.PlayerCurrentLapTime > newData.PlayerCurrentLapTime)
            );
    }

    private SessionData Merge(SessionData oldData, SessionData newData)
    {
        // New data is the basis
        var mergedData = newData.Clone();

        if (oldData == null)
        {
            return mergedData;
        }

        // Adopt the player completed laps from the old data
        mergedData.PlayerCompletedLaps = oldData.PlayerCompletedLaps.Select(p => p.Clone()).ToList();

        // Player has finished a lap -> add it to the completed laps store
        if (oldData.PlayerCurrentLapNumber >= 1 &&
            newData.PlayerCurrentLapNumber > oldData.PlayerCurrentLapNumber &&
            newData.PlayerEntryData.LastLapTimes.FullLap.HasValue)
        {
            SimHub.Logging.Current.Info($"SessionDataProvider::Merge(SessionData): player has finished lap: new lap number {newData.PlayerCurrentLapNumber}, old lap number {oldData.PlayerCurrentLapNumber}, lap time: {newData.PlayerEntryData.LastLapTimes.FullLap.Value}");
            var playerFinishedLapEvent = new PlayerFinishedLapEvent(newData.SessionId, oldData.PlayerCurrentLapNumber, newData.PlayerEntryData.LastLapTimes.FullLap.Value);

            if (mergedData.AddPlayerFinishedLapEvent(playerFinishedLapEvent))
            {
                _sessionDataRecovery.SaveRaceEvent(playerFinishedLapEvent);
            }
        }

        // Merge the entries
        // But only do a proper merging with the old data when the session wasn't reloaded nor the session id has changed,
        // because in such a case the old data is really useless.
        var entries = mergedData.Entries.Select(newEntry =>
        {
            var oldEntry = oldData.Entries.FirstOrDefault(o => o.CarNumber == newEntry.CarNumber);
            return Merge(oldEntry, newEntry, mergedData);
        })
        .Where(entry => entry != null && entry.IsValid(mergedData.Game))
        .Sort((a, b) => newData.SessionType == SessionType.Race ? a.CompareByProgressTo(b) : a.CompareByBestLapTimeTo(b))
        .ToList();

        SetAdditionalTimingData(entries, newData.SessionType);
        mergedData.Entries = entries;
        return mergedData;
    }

    private SessionEntryData Merge(SessionEntryData oldData, SessionEntryData newData, SessionData newSessionData)
    {
        if (!newData.IsValid(newSessionData.Game))
        {
            return oldData;
        }

        var mergedData = newData.Clone();

        if (oldData == null)
        {
            return mergedData;
        }

        // Adopt the pit events and the progresses from the old data
        mergedData.PitEvents = oldData.PitEvents.Select(p => p.Clone()).ToList();
        mergedData.EntryProgresses = oldData.EntryProgresses.Select(p => p.Clone()).ToList();

        // Adopt the ElapsedSessionTime from the SessionData
        mergedData.ElapsedSessionTime = newSessionData.ElapsedSessionTime;

        // Add events or progression only in race session
        if (newSessionData.SessionType == SessionType.Race)
        {
            // Add pit event
            if (mergedData.IsInPit != oldData.IsInPit)
            {
                var pitEvent = new PitEvent(
                    newSessionData.SessionId,
                    mergedData.IsInPit ? RaceEventType.PitIn : RaceEventType.PitOut,
                    mergedData.CarNumber,
                    mergedData.CurrentLapNumber,
                    newSessionData.ElapsedSessionTime,
                    newSessionData.PlayerCurrentLapNumber,
                    newSessionData.PlayerCurrentLapTime
                );

                if (mergedData.AddPitEvent(pitEvent))
                {
                    _sessionDataRecovery.SaveRaceEvent(pitEvent);
                }
            }

            // Add progress (only if SessionTimeLeft property is available)
            if (mergedData.ElapsedSessionTime != null && (
                    oldData.CurrentLapNumber != mergedData.CurrentLapNumber ||
                    EntryProgress.GetMiniSector(oldData.TrackPositionPercent) != EntryProgress.GetMiniSector(mergedData.TrackPositionPercent))
                )
            {
                var currentProgress = new EntryProgress()
                {
                    LapNumber = mergedData.CurrentLapNumber,
                    MiniSector = EntryProgress.GetMiniSector(mergedData.TrackPositionPercent),
                    ElapsedSessionTime = mergedData.ElapsedSessionTime,
                    SimHubPosition = mergedData.SimHubPosition
                };

                mergedData.AddEntryProgress(currentProgress);
            }
        }

        return mergedData;
    }

    private void SetAdditionalTimingData(List<SessionEntryData> entries, SessionType sessionType)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            entries[i].Position = i + 1;

            // Omit the leader, because we don't have to calculate any gaps for him{
            if (i > 0)
            {
                var gapToLeader = entries[i].CalcGap(entries[0], sessionType);
                entries[i].GapToLeader = gapToLeader?.StartsWith("+") == true ? gapToLeader : null;

                var gapToClassLeader = entries[i].CalcGap(GetClassLeader(entries, entries[i].CarClass), sessionType);
                entries[i].GapToClassLeader = gapToClassLeader?.StartsWith("+") == true ? gapToClassLeader : null;

                var gapToInFront = entries[i].CalcGap(entries[i - 1], sessionType);
                entries[i].GapToInFront = gapToInFront?.StartsWith("+") == true ? gapToInFront : null;
            }
        }
    }

    private SessionEntryData GetClassLeader(List<SessionEntryData> entries, string raceClass)
    {
        return entries.FirstOrDefault(e => e.CarClass == raceClass);
    }
}
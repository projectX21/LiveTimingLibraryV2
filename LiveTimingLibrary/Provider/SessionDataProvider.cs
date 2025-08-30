using System;
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
            SimHub.Logging.Current.Info($"SessionDataProvider::MergeWithPreviousData() of SessionData: session change detected: {recoveredData?.SessionId} to {newData.SessionId}");
            _sessionDataRecovery.Clear();
            PropertyManager.Instance.ResetAll();
            return null;
        }
        else if (WasSessionReloaded(recoveredData, newData))
        {
            SimHub.Logging.Current.Info($"SessionDataProvider::MergeWithPreviousData() of SessionData: session reload detected");
            PropertyManager.Instance.ResetAll();

            // Event must be fired _before_ creating the recovery data
            _sessionDataRecovery.SaveRaceEvent(new SessionReloadEvent(newData.SessionId, newData.CurrentLapNumber, newData.CurrentLapTime));
            return _sessionDataRecovery.CreateBasicSessionDataFromRaceEventRecovery();
        }

        return recoveredData;
    }

    private bool HasSessionIdChanged(SessionData recoveredData, SessionData newData)
    {
        return recoveredData == null || recoveredData.SessionId != newData.SessionId;
    }

    private bool WasSessionReloaded(SessionData recoveredData, SessionData newData)
    {
        return recoveredData != null && (
                    recoveredData.CurrentLapNumber > newData.CurrentLapNumber ||
                    (recoveredData.CurrentLapNumber == newData.CurrentLapNumber && recoveredData.CurrentLapTime > newData.CurrentLapTime)
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

        // Merge the entries
        // But only do a proper merging with the old data when the session wasn't reloaded nor the session id has changed,
        // because in such a case the old data is really useless.
        var entries = mergedData.Entries.Select(newEntry =>
            {
                var oldEntry = oldData.Entries.FirstOrDefault(o => o.CarNumber == newEntry.CarNumber);
                return Merge(oldEntry, newEntry, mergedData);
            })
            .Where(entry => entry != null)
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

        // Add pit event
        if (mergedData.IsInPit != oldData.IsInPit)
        {
            if (mergedData.IsInPit)
            {
                var pitInEvent = new PitInEvent(newSessionData.SessionId, mergedData.CarNumber, mergedData.CurrentLapNumber, mergedData.CurrentLapTime);
                mergedData.AddPitEvent(pitInEvent);
                _sessionDataRecovery.SaveRaceEvent(pitInEvent);
            }
            else
            {
                var pitOutEvent = new PitOutEvent(newSessionData.SessionId, mergedData.CarNumber, mergedData.CurrentLapNumber, mergedData.CurrentLapTime, mergedData.LastLapTimes?.FullLap ?? TimeSpan.Zero);
                mergedData.AddPitEvent(pitOutEvent);
                _sessionDataRecovery.SaveRaceEvent(pitOutEvent);
            }
        }

        // Add progress (only if SessionTimeLeft property is available)
        if (newSessionData.SessionTimeLeft != null && (
                oldData.CurrentLapNumber != mergedData.CurrentLapNumber ||
                EntryProgress.GetMiniSector(oldData.TrackPositionPercent) != EntryProgress.GetMiniSector(mergedData.TrackPositionPercent))
            )
        {
            var currentProgress = new EntryProgress()
            {
                LapNumber = mergedData.CurrentLapNumber,
                MiniSector = EntryProgress.GetMiniSector(mergedData.TrackPositionPercent),
                SessionTimeLeft = newSessionData.SessionTimeLeft,
                SimHubPosition = mergedData.SimHubPosition
            };

            mergedData.AddEntryProgress(currentProgress);
        }

        /* Special handling for ACC:
        * ACC has the strange behavior, that it will change the lap number and the track position % some milliseconds too late when the lap time is already reseted to 0.
        *
        * Example:
        * [2024-11-13 21:56:08,387] INFO - Lap: 2 - Lap time 98.087
        * [2024-11-13 21:56:08,404] INFO - Lap: 2 - Lap time 0.012
        * [2024-11-13 21:56:08,420] INFO - Lap: 3 - Lap time 0.027 */
        if (oldData.CurrentLapNumber == mergedData.CurrentLapNumber && oldData.CurrentLapTime > mergedData.CurrentLapTime)
        {
            mergedData.CurrentLapNumber += 1;
            mergedData.TrackPositionPercent = 0.0;
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
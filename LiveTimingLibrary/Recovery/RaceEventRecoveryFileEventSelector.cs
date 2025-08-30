using System;
using System.Collections.Generic;
using System.IO;

public class RaceEventRecoveryFileEventSelector<T> : IRaceEventRecoveryFileEventSelector<T> where T : RaceEvent
{
    public List<T> SelectSpecificEvents(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<T>();
        }

        var lines = File.ReadAllLines(filePath);
        var events = new List<T>();
        int? lapNumberLastReload = null;
        TimeSpan? lapTimeLastReload = null;
        // TimeSpan? elapsedTimeLastReload = null;

        // read the recovery file in reverse order
        for (var i = lines.Length - 1; i >= 0; i--)
        {
            var raceEvent = ToRaceEvent(lines[i]);

            if (raceEvent is SessionReloadEvent @sessionEvent)
            {
                lapNumberLastReload = @sessionEvent.CurrentLapNumber;
                lapTimeLastReload = @sessionEvent.CurrentLapTime;
            }
            else if (raceEvent is T @specificEvent)
            {
                if (lapNumberLastReload == null || @specificEvent.CurrentLapNumber < lapNumberLastReload || (
                    @specificEvent.CurrentLapNumber == lapNumberLastReload && @specificEvent.CurrentLapTime <= lapTimeLastReload
                ))
                {
                    events.Add(@specificEvent);
                }
            }
        }

        // the file was read in reverse order, therefore it has to reversed again to have the initial order.
        events.Reverse();
        return events;
    }

    private RaceEvent ToRaceEvent(string line)
    {
        if (PitInEvent.MatchesRecoveryFileFormat(line))
        {
            return PitInEvent.CreateFromRecoveryFileLine(line);
        }
        else if (PitOutEvent.MatchesRecoveryFileFormat(line))
        {
            return PitOutEvent.CreateFromRecoveryFileLine(line);
        }
        else if (SessionReloadEvent.MatchesRecoveryFileFormat(line))
        {
            return SessionReloadEvent.CreateFromRecoveryFileLine(line);
        }
        else
        {
            throw new Exception("RaceEventRecoveryFileEventSelector::ToRaceEvent(): cannot parse line into RaceEvent: " + line);
        }
    }
}
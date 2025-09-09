using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class RaceEventRecoveryFileEventSelector<T> : IRaceEventRecoveryFileEventSelector<T> where T : RaceEvent
{
    public List<T> SelectSpecificEvents(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<T>();
        }

        var lines = Utils.ReadLinesFromFile(filePath);
        var events = new List<T>();
        int? lapNumberLastReload = null;
        TimeSpan? lapTimeLastReload = null;

        // read the recovery file in reverse order
        for (var i = lines.Count() - 1; i >= 0; i--)
        {
            var raceEvent = ToRaceEvent(lines[i]);

            if (raceEvent is SessionReloadEvent @sessionEvent)
            {
                lapNumberLastReload = @sessionEvent.PlayerLapNumber;
                lapTimeLastReload = @sessionEvent.PlayerLapTime;
            }
            else if (raceEvent is T @specificEvent)
            {
                if (lapNumberLastReload == null || @specificEvent.PlayerLapNumber < lapNumberLastReload || (
                    @specificEvent.PlayerLapNumber == lapNumberLastReload && @specificEvent.PlayerLapTime <= lapTimeLastReload
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
        if (PitEvent.MatchesRecoveryFileFormat(line))
        {
            return PitEvent.CreateFromRecoveryFileLine(line);
        }
        else if (PlayerFinishedLapEvent.MatchesRecoveryFileFormat(line))
        {
            return PlayerFinishedLapEvent.CreateFromRecoveryFileLine(line);
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
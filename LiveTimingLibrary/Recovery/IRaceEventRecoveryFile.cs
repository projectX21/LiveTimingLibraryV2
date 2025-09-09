using System.Collections.Generic;

public interface IRaceEventRecoveryFile
{
    void AddEvent(RaceEvent raceEvent);

    List<PitEvent> ReadPitEvents();

    List<PlayerFinishedLapEvent> ReadPlayerFinishedLapEvents();

    void Clear();
}
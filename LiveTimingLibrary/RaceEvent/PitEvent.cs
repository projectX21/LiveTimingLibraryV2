using System;

public abstract class PitEvent : RaceEvent
{
    public string EntryId { get; }

    public PitEvent(string sessionId, RaceEventType type, string entryId, int currentLapNumber, TimeSpan currentLapTime)
        : base(sessionId, type, currentLapNumber, currentLapTime)
    {
        EntryId = entryId;
    }

    public abstract PitEvent Clone();

    public static TimeSpan CalcPitStopDuration(PitInEvent pitIn, PitOutEvent pitOut)
    {
        return pitOut.LastLapTime - pitIn.CurrentLapTime + pitOut.CurrentLapTime;
    }
}
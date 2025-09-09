using System;

public class PitEvent : RaceEvent
{
    public string EntryId { get; }

    public int EntryLapNumber { get; }

    public TimeSpan ElapsedSessionTime { get; }

    public PitEvent(string sessionId, RaceEventType type, string entryId, int entryLapNumber, TimeSpan elapsedSessionTime, int playerLapNumber, TimeSpan playerLapTime)
        : base(sessionId, type, playerLapNumber, playerLapTime)
    {
        EntryId = entryId;
        EntryLapNumber = entryLapNumber;
        ElapsedSessionTime = elapsedSessionTime;
    }

    public PitEvent Clone()
    {
        return new PitEvent(SessionId, Type, EntryId, EntryLapNumber, ElapsedSessionTime, PlayerLapNumber, PlayerLapTime);
    }

    public static PitEvent CreateFromRecoveryFileLine(string line)
    {
        if (!MatchesRecoveryFileFormat(line))
        {
            throw new Exception("PitEvent(): Cannot parse line: " + line + " into PitEvent!");
        }

        var tokens = SplitLine(line);

        if (tokens.Length != 7)
        {
            throw new Exception("PitEvent(): Cannot init PitEvent! Invalid line:  " + line);
        }

        string sessionId = tokens[0];
        RaceEventType type = RaceEventTypeConverter.ToEnum(tokens[1]);
        string entryId = tokens[2];
        int entryLapNumber = ToInt(tokens[3]);
        TimeSpan elapsedSessionTime = ToTimeSpan(tokens[4]);
        int playerLapNumber = ToInt(tokens[5]);
        TimeSpan playerLapTime = ToTimeSpan(tokens[6]);

        return new PitEvent(sessionId, type, entryId, entryLapNumber, elapsedSessionTime, playerLapNumber, playerLapTime);
    }

    public static bool MatchesRecoveryFileFormat(string line)
    {
        var eventType = GetRaceEventTypeFromLine(line);
        return eventType == RaceEventType.PitIn || eventType == RaceEventType.PitOut;
    }

    public static TimeSpan CalcPitStopDuration(PitEvent pitIn, PitEvent pitOut)
    {
        return pitOut.ElapsedSessionTime.Subtract(pitIn.ElapsedSessionTime);
    }

    public override string ToRecoveryFileFormat()
    {
        return SessionId + s_recoveryFilePatternDelimiter
         + RaceEventTypeConverter.FromEnum(Type) + s_recoveryFilePatternDelimiter
         + EntryId + s_recoveryFilePatternDelimiter
         + EntryLapNumber + s_recoveryFilePatternDelimiter
         + ElapsedSessionTime + s_recoveryFilePatternDelimiter
         + PlayerLapNumber + s_recoveryFilePatternDelimiter
         + PlayerLapTime;
    }

    public override string ToString()
    {
        return $"[ sessionId: {SessionId}, type: {RaceEventTypeConverter.FromEnum(Type)}, EntryId: {EntryId}, EntryLapNumber: {EntryLapNumber}, ElapsedSessionTime: {ElapsedSessionTime}, PlayerLapNumber: {PlayerLapNumber}, PlayerLapTime: {PlayerLapTime} ]";
    }
}
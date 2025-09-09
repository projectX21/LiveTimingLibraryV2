using System;

public class SessionReloadEvent : RaceEvent
{
    public SessionReloadEvent(string sessionId, int playerLapNumber, TimeSpan playerLapTime)
        : base(sessionId, RaceEventType.SessionReload, playerLapNumber, playerLapTime) { }

    public static SessionReloadEvent CreateFromRecoveryFileLine(string line)
    {
        if (!MatchesRecoveryFileFormat(line))
        {
            throw new Exception("SessionReloadEvent(): Cannot parse line: " + line + " into SessionReloadEvent!");
        }

        var tokens = SplitLine(line);

        if (tokens.Length != 4)
        {
            throw new Exception("SessionReloadEvent(): Cannot create SessionReloadEvent from recovery file! Invalid line:  " + line);
        }

        string sessionId = tokens[0];
        int currentLapNumber = ToInt(tokens[2]);
        TimeSpan currentLapTime = ToTimeSpan(tokens[3]);

        return new SessionReloadEvent(sessionId, currentLapNumber, currentLapTime);
    }

    public override string ToRecoveryFileFormat()
    {
        return SessionId + s_recoveryFilePatternDelimiter
          + RaceEventTypeConverter.FromEnum(Type) + s_recoveryFilePatternDelimiter
          + PlayerLapNumber + s_recoveryFilePatternDelimiter
          + PlayerLapTime;
    }

    public override string ToString()
    {
        return $"[ sessionId: {SessionId}, type: {RaceEventTypeConverter.FromEnum(Type)}, PlayerLapNumber: {PlayerLapNumber}, PlayerLapTime: {PlayerLapTime} ]";
    }

    public static bool MatchesRecoveryFileFormat(string line)
    {
        return GetRaceEventTypeFromLine(line) == RaceEventType.SessionReload;
    }
}
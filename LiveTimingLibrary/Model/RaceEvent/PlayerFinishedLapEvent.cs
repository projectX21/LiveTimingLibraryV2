using System;

public class PlayerFinishedLapEvent : RaceEvent
{
    public PlayerFinishedLapEvent(string sessionId, int playerLapNumber, TimeSpan playerLapTime)
    : base(sessionId, RaceEventType.PlayerFinishedLap, playerLapNumber, playerLapTime) { }

    public static PlayerFinishedLapEvent CreateFromRecoveryFileLine(string line)
    {
        if (!MatchesRecoveryFileFormat(line))
        {
            throw new Exception("PlayerFinishedLapEvent(): Cannot parse line: " + line + " into PlayerFinishedLapEvent!");
        }

        var tokens = SplitLine(line);

        if (tokens.Length != 4)
        {
            throw new Exception("PlayerFinishedLapEvent(): Cannot create PlayerFinishedLapEvent from recovery file! Invalid line:  " + line);
        }

        string sessionId = tokens[0];
        int currentLapNumber = ToInt(tokens[2]);
        TimeSpan currentLapTime = ToTimeSpan(tokens[3]);

        return new PlayerFinishedLapEvent(sessionId, currentLapNumber, currentLapTime);
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
        return $"[ sessionId: {SessionId}, type: {RaceEventTypeConverter.FromEnum(Type)}, CurrentLapNumber: {PlayerLapNumber}, CurrentLapTime: {PlayerLapTime} ]";
    }

    public PlayerFinishedLapEvent Clone()
    {
        return new PlayerFinishedLapEvent(SessionId, PlayerLapNumber, PlayerLapTime);
    }

    public static bool MatchesRecoveryFileFormat(string line)
    {
        return GetRaceEventTypeFromLine(line) == RaceEventType.PlayerFinishedLap;
    }
}
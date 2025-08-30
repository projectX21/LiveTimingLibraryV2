using System;
using System.Linq;
using System.Collections.Generic;

public class SessionData
{
    public GameTitle Game { get; set; }

    public SessionType SessionType { get; set; }

    public string SessionId
    {
        get
        {
            return SessionIdGenerator.Generate(Game, SessionType);
        }
    }

    // The entry progresses will be created by the left session time.
    // If this property isn't set by the specific game, the entry progresses won't be created,
    // and therefore no gap calculation by the entry progresses possible (then the gap will be calculated by the SimHub property GapToLeader)
    public TimeSpan SessionTimeLeft { get; set; }

    public int CurrentLapNumber { get; set; }

    public TimeSpan CurrentLapTime { get; set; }

    public string TrackName { get; set; }

    public List<SessionEntryData> Entries { get; set; } = new List<SessionEntryData>();

    public bool IsValid
    {
        get
        {
            if (Game == GameTitle.ACC && !SessionDataValidator.IsValidAccData(CurrentLapTime))
            {
                return false;
            }

            return true;
        }
    }

    public SessionData Clone()
    {
        return new SessionData()
        {
            Game = Game,
            SessionType = SessionType,
            SessionTimeLeft = SessionTimeLeft,
            CurrentLapNumber = CurrentLapNumber,
            CurrentLapTime = CurrentLapTime,
            TrackName = TrackName,
            Entries = Entries.Select(e => e.Clone()).ToList()
        };
    }

    public void Log(bool logEntries = true)
    {
        SimHub.Logging.Current.Debug($"SessionData::Log()");
        SimHub.Logging.Current.Debug($"SessionData::Log(): Game:                {Game}");
        SimHub.Logging.Current.Debug($"SessionData::Log(): SessionType:         {SessionType}");
        SimHub.Logging.Current.Debug($"SessionData::Log(): SessionId:           {SessionId}");
        SimHub.Logging.Current.Debug($"SessionData::Log(): SessionTimeLeft:     {SessionTimeLeft}");
        SimHub.Logging.Current.Debug($"SessionData::Log(): CurrentLapNumber:    {CurrentLapNumber}");
        SimHub.Logging.Current.Debug($"SessionData::Log(): CurrentLapTime:      {CurrentLapTime}");
        SimHub.Logging.Current.Debug($"SessionData::Log(): TrackName:           {TrackName}");

        if (logEntries)
        {
            Entries.ForEach(e => e.Log());
        }
    }
}

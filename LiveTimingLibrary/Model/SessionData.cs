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

    public string TrackName { get; set; }

    public List<SessionEntryData> Entries { get; set; } = new List<SessionEntryData>();

    public int PlayerCurrentLapNumber { get; set; }

    public TimeSpan PlayerCurrentLapTime { get; set; }

    public List<PlayerFinishedLapEvent> PlayerCompletedLaps { get; set; } = new List<PlayerFinishedLapEvent>();

    public SessionEntryData PlayerEntryData
    {
        get
        {
            return Entries.Where(e => e.IsPlayer).FirstOrDefault();
        }
    }

    public TimeSpan ElapsedSessionTime
    {
        get
        {
            return TimeSpan.FromTicks(PlayerCompletedLaps.Sum(lap => lap.PlayerLapTime.Ticks)).Add(PlayerCurrentLapTime);
        }
    }

    public SessionData Clone()
    {
        return new SessionData()
        {
            Game = Game,
            SessionType = SessionType,
            TrackName = TrackName,
            Entries = Entries.Select(e => e.Clone()).ToList(),
            PlayerCurrentLapNumber = PlayerCurrentLapNumber,
            PlayerCurrentLapTime = PlayerCurrentLapTime,
            PlayerCompletedLaps = PlayerCompletedLaps.Select(l => l.Clone()).ToList()
        };
    }

    public bool IsValid()
    {
        return SessionDataValidator.IsValidSession(this);
    }

    public bool AddPlayerFinishedLapEvent(PlayerFinishedLapEvent playerFinishedLapEvent)
    {
        if (IsPlayerFinishedLapEventAddable(playerFinishedLapEvent))
        {
            SimHub.Logging.Current.Info($"SessionEntryData::AddPlayerFinishedLapEvent(): add event: {playerFinishedLapEvent}");
            PlayerCompletedLaps.Add(playerFinishedLapEvent);
            return true;
        }

        return false;
    }

    public void Log(bool logEntries = true)
    {
        SimHub.Logging.Current.Info($"SessionData::Log(): Game:                   {Game}");
        SimHub.Logging.Current.Info($"SessionData::Log(): SessionType:            {SessionType}");
        SimHub.Logging.Current.Info($"SessionData::Log(): SessionId:              {SessionId}");
        SimHub.Logging.Current.Info($"SessionData::Log(): TrackName:              {TrackName}");
        SimHub.Logging.Current.Info($"SessionData::Log(): PlayerCurrentLapNumber: {PlayerCurrentLapNumber}");
        SimHub.Logging.Current.Info($"SessionData::Log(): PlayerCurrentLapTime:   {PlayerCurrentLapTime}");
        SimHub.Logging.Current.Info($"SessionData::Log(): ElapsedSessionTime:     {ElapsedSessionTime}");
        SimHub.Logging.Current.Info($"SessionData::Log(): PlayerFinishedLaps:     {PlayerCompletedLaps.Count()}");
        PlayerCompletedLaps.ForEach(l => SimHub.Logging.Current.Info(l));

        if (logEntries)
        {
            Entries.ForEach(e => e.Log());
        }
    }

    private bool IsPlayerFinishedLapEventAddable(PlayerFinishedLapEvent playerFinishedLapEvent)
    {
        if (PlayerCompletedLaps.Count() == 0 && playerFinishedLapEvent.PlayerLapNumber != 1)
        {
            SimHub.Logging.Current.Warn("SessionData::IsPlayerFinishedLapEventAddable(): PlayerFinishedLapEvent is not addable!. First element must have the lap number 1!");
            return false;
        }
        else if (PlayerCompletedLaps.Count() > 0
                && playerFinishedLapEvent.PlayerLapNumber != (PlayerCompletedLaps.Last().PlayerLapNumber + 1))
        {
            SimHub.Logging.Current.Warn($"SessionData::IsPlayerFinishedLapEventAddable(): PlayerFinishedLapEvent is not addable!. New lap number {playerFinishedLapEvent.PlayerLapNumber} is not the consecutive one in the current data (last lap number in store: {PlayerCompletedLaps.Last().PlayerLapNumber})!");
            return false;
        }

        return true;
    }

    private TimeSpan CalcElapsedSessionTime()
    {
        var elapsedSessionTime = TimeSpan.Zero;

        foreach (var lap in PlayerCompletedLaps)
        {
            elapsedSessionTime = elapsedSessionTime.Add(lap.PlayerLapTime);
        }

        return elapsedSessionTime;
    }
}

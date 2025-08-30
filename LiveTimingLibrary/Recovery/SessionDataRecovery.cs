using System.Linq;

public class SessionDataRecovery : ISessionDataRecovery
{
    private readonly ISessionDataRecoveryFile _sessionDataRecoveryFile;

    private readonly IRaceEventRecoveryFile _raceEventRecoveryFile;

    private SessionData _previousData;

    public SessionDataRecovery(ISessionDataRecoveryFile sessionDataRecoveryFile, IRaceEventRecoveryFile raceEventRecoveryFile)
    {
        _sessionDataRecoveryFile = sessionDataRecoveryFile;
        _raceEventRecoveryFile = raceEventRecoveryFile;
    }

    public SessionData GetRecoveryData()
    {
        if (_previousData != null)
        {
            return _previousData;
        }

        SessionData recovery = _sessionDataRecoveryFile.Load();

        if (recovery != null)
        {
            return recovery;
        }
        else
        {
            return null;
        }
    }

    public void SaveSessionData(SessionData data)
    {
        _previousData = data;
        _sessionDataRecoveryFile.Save(data);
    }

    public void SaveRaceEvent(RaceEvent raceEvent)
    {
        _raceEventRecoveryFile.AddEvent(raceEvent);
    }

    public SessionData CreateBasicSessionDataFromRaceEventRecovery()
    {
        var entries = _raceEventRecoveryFile.ReadPitEvents().GroupBy(p => p.EntryId).Select(group =>
            new SessionEntryData()
            {
                CarNumber = group.Key,
                PitEvents = group.ToList()
            }
        ).ToList();

        SimHub.Logging.Current.Debug($"SessionDataRecovery::CreateBasicSessionDataFromRaceEventRecovery(): found {entries.Count} entries");

        return new SessionData()
        {
            Entries = entries
        };
    }

    public void Clear()
    {
        SimHub.Logging.Current.Debug("SessionDataRecovery::Clear()");
        _previousData = null;
        _sessionDataRecoveryFile.Clear();
        _raceEventRecoveryFile.Clear();
    }
}
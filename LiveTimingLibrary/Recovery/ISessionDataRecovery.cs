using System;

public interface ISessionDataRecovery
{
    SessionData GetRecoveryData();

    void SaveSessionData(SessionData data);

    void SaveRaceEvent(RaceEvent raceEvent);

    SessionData CreateBasicSessionDataFromRaceEventRecovery();

    void Clear();
}
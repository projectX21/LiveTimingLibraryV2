using GameReaderCommon;
using System.Linq;
using System.Collections.Generic;
using System;

public class SessionDataConverter
{
    public static SessionData ToSessionData(GameData data)
    {
        // In ACC it needs quite a long time to initiate all opponents. Only the player exists in the first few seconds.
        if (data.GamePaused || data.NewData == null || (data.NewData.Opponents?.Count ?? 0) <= 1)
        {
            return null;
        }

        GameTitle game = GameTitleConverter.ToEnum(data.GameName);

        return new SessionData
        {
            Game = game,
            SessionType = SessionTypeConverter.ToEnum(data.NewData.SessionTypeName),
            PlayerCurrentLapNumber = data.NewData.CurrentLap,
            PlayerCurrentLapTime = data.NewData.CurrentLapTime,
            TrackName = data.NewData.TrackName,
            Entries = GetSessionEntries(data)
        };
    }

    private static List<SessionEntryData> GetSessionEntries(GameData data)
    {
        var entries = data.NewData.Opponents.Select(opponent => SessionEntryDataConverter.ToSessionEntryData(opponent, data)).ToList();

        // This is not possible beforehand, because all opponents must be converted into the SessionEntryData at first
        entries.ForEach(e => SessionEntryDataConverter.SetPaceIndicators(
            e,
            entries.Where(en => en.CarClass == e.CarClass).Select(en => en.BestTimes).ToArray()
        ));

        return entries;
    }
}
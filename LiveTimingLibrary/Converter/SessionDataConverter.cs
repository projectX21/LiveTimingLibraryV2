using GameReaderCommon;
using System.Linq;
using RfactorReader.RF2;
using CrewChiefV4.rFactor2_V2.rFactor2Data;
using System.Collections.Generic;
using AcTools.Utils.Helpers;
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

        var result = new SessionData
        {
            Game = game,
            SessionType = SessionTypeConverter.ToEnum(data.NewData.SessionTypeName),
            SessionTimeLeft = data.NewData.SessionTimeLeft,
            CurrentLapNumber = data.NewData.CurrentLap,
            CurrentLapTime = data.NewData.CurrentLapTime.TotalSeconds > 0.0 ? data.NewData.CurrentLapTime : TimeSpan.Zero,
            TrackName = data.NewData.TrackName,
            Entries = GetSessionEntries(data.NewData.Opponents, game)
        };

        GameSpecificConvertions(result, data);
        return result;
    }

    private static List<SessionEntryData> GetSessionEntries(List<Opponent> opponents, GameTitle game)
    {
        var entries = opponents.Select(opponent => SessionEntryDataConverter.ToSessionEntryData(opponent, game)).ToList();

        // This is not possible beforehand, because all opponents must be converted into the SessionEntryData at first
        entries.ForEach(e => SessionEntryDataConverter.SetPaceIndicators(
            e,
            entries.Where(en => en.CarClass == e.CarClass).Select(en => en.BestTimes).ToArray()
        ));

        // TODO Set progress

        return entries;
    }

    private static SessionData GameSpecificConvertions(SessionData result, GameData originData)
    {
        if (result.Entries?.Count() > 0)
        {
            if (result.Game == GameTitle.RF2)
            {
                RF2SpecificConvertions(result.Entries, (originData as GameData<WrapV2>).GameNewData.Raw.telemetry.mVehicles);
            }
        }

        return result;
    }

    /*
        private static List<SessionEntryData> RF2SpecificConvertions(List<SessionEntryData> entries, rF2VehicleScoring[] rF2Entries)
        {
            foreach (var entry in entries)
            {
                foreach (var rF2Entry in rF2Entries)
                {
                    if (GetRF2Name(rF2Entry.mVehicleName).Contains("#" + entry.CarNumber + " "))
                    {
                        if (rF2Entry.mTimeBehindNext != 0.0)
                        {
                            entry.SimHubGapToInFront = rF2Entry.mTimeBehindNext;
                        }
                    }
                }
            }

            return entries;
        }
    */

    private static List<SessionEntryData> RF2SpecificConvertions(List<SessionEntryData> entries, rF2VehicleTelemetry[] rF2Entries)
    {
        foreach (var entry in entries)
        {
            foreach (var rF2Entry in rF2Entries)
            {
                if (GetRF2Name(rF2Entry.mVehicleName).Contains("#" + entry.CarNumber + " "))
                {
                    entry.FuelLoad = rF2Entry.mFuel;
                    entry.FuelCapacity = rF2Entry.mFuelCapacity;
                }
            }
        }

        return entries;
    }

    private static string GetRF2Name(byte[] vehicleName)
    {
        return new string(
            System.Text.Encoding.Default.GetString(vehicleName).Where(c => !char.IsControl(c)).ToArray()
        );
    }
}
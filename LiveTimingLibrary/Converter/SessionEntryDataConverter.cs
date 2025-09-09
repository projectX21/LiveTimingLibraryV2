using GameReaderCommon;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using RfactorReader.RF2;
using CrewChiefV4.rFactor2_V2.rFactor2Data;

public class SessionEntryDataConverter
{
    public SessionEntryDataCustomStatementConverter StatementConverter { get; set; }

    public static SessionEntryData ToSessionEntryData(Opponent entry, GameData data)
    {
        GameTitle game = GameTitleConverter.ToEnum(data.GameName);

        var sessionEntryData = new SessionEntryData
        {
            CarNumber = GetCarNumber(entry, game),
            DisplayName = GetDisplayName(entry, game),
            Manufacturer = GetManufacturer(entry, game),
            CarModel = GetCarModel(entry, game),
            CarClass = entry.CarClass,
            IsInPit = entry.IsCarInPit || entry.IsCarInPitLane || (entry.IsCarInGarage ?? false) || entry.StandingStillInPitLane,
            IsPlayer = entry.IsPlayer,
            CurrentLapNumber = entry.CurrentLap ?? 1,
            TrackPositionPercent = entry.TrackPositionPercent ?? 0.0,
            SimHubPosition = entry.Position,
            SimHubGapToLeader = entry.GaptoLeader,
            LastLapTimes = new FragmentTimes()
            {
                Sector1 = NormalizeTimeSpan(entry.LastLapSectorTimes?.GetSectorSplit(1)),
                Sector2 = NormalizeTimeSpan(entry.LastLapSectorTimes?.GetSectorSplit(2)),
                Sector3 = NormalizeTimeSpan(entry.LastLapSectorTimes?.GetSectorSplit(3)),
                FullLap = NormalizeTimeSpan(entry.LastLapTime)
            },
            CurrentLapTimes = new FragmentTimes()
            {
                Sector1 = NormalizeTimeSpan(entry.CurrentLapSectorTimes?.GetSectorSplit(1)),
                Sector2 = NormalizeTimeSpan(entry.CurrentLapSectorTimes?.GetSectorSplit(2)),
            },
            BestTimes = new FragmentTimes()
            {
                Sector1 = NormalizeTimeSpan(GetBestLapFragmentTime(entry, LapFragmentType.SECTOR_1)),
                Sector2 = NormalizeTimeSpan(GetBestLapFragmentTime(entry, LapFragmentType.SECTOR_2)),
                Sector3 = NormalizeTimeSpan(GetBestLapFragmentTime(entry, LapFragmentType.SECTOR_3)),
                FullLap = NormalizeTimeSpan(GetBestLapFragmentTime(entry, LapFragmentType.FULL_LAP))
            },
            FrontTyreCompound = entry.FrontTyreCompound,
            RearTyreCompound = entry.RearTyreCompound,
            StartPosition = entry.StartPosition,
            Game = GameTitleConverter.ToEnum(data.GameName)
        };

        GameSpecificConvertions(sessionEntryData, data);
        SessionEntryDataCustomStatementConverter.ModifyMatchingEntry(sessionEntryData, GameTitleConverter.ToEnum(data.GameName));
        return sessionEntryData;
    }

    public static void SetPaceIndicators(SessionEntryData entry, FragmentTimes[] bestFragmentTimesInSameClass)
    {
        SetPaceIndicators(entry.CurrentLapTimes, entry.BestTimes, bestFragmentTimesInSameClass);
        SetPaceIndicators(entry.LastLapTimes, entry.BestTimes, bestFragmentTimesInSameClass);
        SetPaceIndicators(entry.BestTimes, entry.BestTimes, bestFragmentTimesInSameClass);
    }

    private static SessionEntryData GameSpecificConvertions(SessionEntryData entry, GameData data)
    {
        switch (GameTitleConverter.ToEnum(data.GameName))
        {
            case GameTitle.RF2:
                RF2SpecificConvertions(entry, (data as GameData<WrapV2>).GameNewData.Raw.Scoring.mVehicles);
                RF2SpecificConvertions(entry, (data as GameData<WrapV2>).GameNewData.Raw.telemetry.mVehicles);
                break;

            case GameTitle.LMU:
                RF2SpecificConvertions(entry, (data as GameData<LMUWrapV2>).GameNewData.Raw.Scoring.mVehicles);
                RF2SpecificConvertions(entry, (data as GameData<LMUWrapV2>).GameNewData.Raw.telemetry.mVehicles);
                break;
        }

        return entry;
    }

    private static SessionEntryData RF2SpecificConvertions(SessionEntryData entry, rF2VehicleScoring[] rF2Entries)
    {
        // Not needed at the moment
        /*
        var carNumberPattern = GetCarNumberPatternForRF2VehicleName(entry.CarNumber);

        foreach (var rF2Entry in rF2Entries)
        {
            if (Regex.IsMatch(Utils.ByteArrayToString(rF2Entry.mVehicleName), carNumberPattern))
            {
            }
        }
        */
        return entry;
    }

    private static SessionEntryData RF2SpecificConvertions(SessionEntryData entry, rF2VehicleTelemetry[] rF2Entries)
    {
        var carNumberPattern = GetCarNumberPatternForRF2VehicleName(entry.CarNumber);

        foreach (var rF2Entry in rF2Entries)
        {
            if (Regex.IsMatch(Utils.ByteArrayToString(rF2Entry.mVehicleName), carNumberPattern))
            {
                entry.FuelLoad = rF2Entry.mFuel;
                entry.FuelCapacity = rF2Entry.mFuelCapacity;
            }
        }

        return entry;
    }

    private static string GetCarNumberPatternForRF2VehicleName(string CarNumber)
    {
        // # + any numbers with negative lookahead: @"(?!\d) No number after the CarNumber itself
        return @"#" + Regex.Escape(CarNumber) + @"(?!\d)";
    }

    private static void SetPaceIndicators(FragmentTimes times, FragmentTimes entryBestTimes, FragmentTimes[] bestFragmentTimesInSameClass)
    {
        if (times == null)
        {
            return;
        }

        times.Sector1PaceIndicator = GetPaceIndicator(times, entryBestTimes, bestFragmentTimesInSameClass, LapFragmentType.SECTOR_1);
        times.Sector2PaceIndicator = GetPaceIndicator(times, entryBestTimes, bestFragmentTimesInSameClass, LapFragmentType.SECTOR_2);
        times.Sector3PaceIndicator = GetPaceIndicator(times, entryBestTimes, bestFragmentTimesInSameClass, LapFragmentType.SECTOR_3);
        times.FullLapPaceIndicator = GetPaceIndicator(times, entryBestTimes, bestFragmentTimesInSameClass, LapFragmentType.FULL_LAP);
    }

    private static FragmentTimePaceIndicator? GetPaceIndicator(FragmentTimes times, FragmentTimes entryBestTimes, FragmentTimes[] bestFragmentTimesInSameClass, LapFragmentType lapFragmentType)
    {
        TimeSpan? time = times.GetTimeByLapFragmentType(lapFragmentType);
        TimeSpan? entryBestTime = entryBestTimes.GetTimeByLapFragmentType(lapFragmentType);
        TimeSpan? classBestTime = bestFragmentTimesInSameClass.Select(e => e.GetTimeByLapFragmentType(lapFragmentType)).Min();

        if (time != null)
        {
            if (time == classBestTime)
            {
                return FragmentTimePaceIndicator.CLASS_BEST;
            }
            else if (time == entryBestTime)
            {
                return FragmentTimePaceIndicator.ENTRY_BEST;
            }
        }

        return null;
    }

    private static string GetCarNumber(Opponent entry, GameTitle game)
    {
        switch (game)
        {
            case GameTitle.AMS2:
                // Format of Name in AMS2: #9.Scott Dixon
                return Regex.Replace(entry.Name, "\\..*$", "").Substring(1).Trim(); // Substring(1) to omit the '#'

            default:
                return entry.CarNumber;
        }
    }

    private static string GetDisplayName(Opponent entry, GameTitle game)
    {
        switch (game)
        {
            case GameTitle.RF2:
                // Format of CarName in RFactor2: "Oreca 07  United Autosport". Remove the manufacturer + car model
                return Regex.Replace(entry.CarName, "^.*  ", "").Trim();

            case GameTitle.AMS2:
                // Format of Name in AMS2 can be #9.Scott Dixon
                return entry.Name.StartsWith("#") ? Regex.Replace(entry.Name, "^.*\\.", "").Trim() : entry.Name.Trim();

            case GameTitle.LMU:
                return Regex.Replace(entry.CarName, @"\d+$", ""); // Porsche Penske Motorsport 2024 -> Remove the year at the end

            default:
                return entry.TeamName;
        }
    }

    private static string GetManufacturer(Opponent entry, GameTitle game)
    {
        return ManufacturerUtils.GetManufacturer(entry.CarName);
    }

    private static string GetCarModel(Opponent entry, GameTitle game)
    {
        string carName = entry.CarName;

        if (game == GameTitle.RF2)
        {
            // Format of CarName in RFactor2: "Oreca 07  United Autosport". Remove the team name at first
            carName = Regex.Replace(carName, "  .*$", "");
        }

        string manufacturer = ManufacturerUtils.GetManufacturer(carName);

        return (manufacturer?.Length > 0)
            ? Regex.Replace(carName, manufacturer, "", RegexOptions.IgnoreCase).Trim()
            : carName;
    }

    private static TimeSpan? GetBestLapFragmentTime(Opponent entry, LapFragmentType lapFragment)
    {
        TimeSpan?[] times = lapFragment == LapFragmentType.FULL_LAP
            ? new TimeSpan?[]
            {
                entry.LastLapTime,
                entry.BestLapTime
            }
            : new TimeSpan?[]
            {
                entry.LastLapSectorTimes?.GetSectorSplit((int)lapFragment),
                entry.CurrentLapSectorTimes?.GetSectorSplit((int)lapFragment),
                entry.BestLapSectorTimes?.GetSectorSplit((int)lapFragment)
            };

        return times.Where(t => t.HasValue).Min();
    }

    private static TimeSpan? NormalizeTimeSpan(TimeSpan? value)
    {
        return value != null && value?.TotalMilliseconds > 0 ? value : null;
    }
}
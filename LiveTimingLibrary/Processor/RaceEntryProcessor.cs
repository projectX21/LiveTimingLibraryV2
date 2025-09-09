public class RaceEntryProcessor : IRaceEntryProcessor
{
    private SessionEntryData _entry;

    public void Process(SessionEntryData entry)
    {
        _entry = entry;
        UpdateGeneralProperties();
        UpdateLapFragmentProperties();
    }

    private void UpdateGeneralProperties()
    {
        if (_entry.Position <= 0)
        {
            return;
        }

        if (_entry.IsPlayer)
        {
            PropertyManager.Instance.Add(PropertyManagerConstants.PLAYER_POSITION, _entry.Position);
        }

        UpdateProperty(PropertyManagerConstants.POSITION, _entry.Position);
        UpdateProperty(PropertyManagerConstants.IS_PLAYER, _entry.IsPlayer);
        UpdateProperty(PropertyManagerConstants.CAR_NUMBER, _entry.CarNumber);
        UpdateProperty(PropertyManagerConstants.DISPLAY_NAME, _entry.DisplayName);
        UpdateProperty(PropertyManagerConstants.CAR_CLASS, _entry.CarClass);
        UpdateProperty(PropertyManagerConstants.MANUFACTURER, _entry.Manufacturer);
        UpdateProperty(PropertyManagerConstants.CAR_MODEL, _entry.CarModel);
        UpdateProperty(PropertyManagerConstants.CURRENT_LAP_NUMBER, _entry.CurrentLapNumber);
        UpdateProperty(PropertyManagerConstants.CURRENT_SECTOR, _entry.CurrentSector);
        UpdateProperty(PropertyManagerConstants.GAP_TO_LEADER, _entry.GapToLeader);
        UpdateProperty(PropertyManagerConstants.GAP_TO_CLASS_LEADER, _entry.GapToClassLeader);
        UpdateProperty(PropertyManagerConstants.GAP_TO_IN_FRONT, _entry.GapToInFront);
        UpdateProperty(PropertyManagerConstants.TYRE_COMPOUND, _entry.FrontTyreCompound);
        UpdateProperty(PropertyManagerConstants.FUEL_CAPACITY, _entry.FuelCapacity);
        UpdateProperty(PropertyManagerConstants.FUEL_LOAD, _entry.FuelLoad);

        // Pit stop properties
        UpdateProperty(PropertyManagerConstants.IN_PITS, _entry.IsInPit);
        UpdateProperty(PropertyManagerConstants.PIT_STOPS_TOTAL, _entry.PitStopCount);
        UpdateProperty(PropertyManagerConstants.PIT_STOPS_TOTAL_DURATION, TimeSpanFormatter.Format(_entry.TotalPitStopDuration));
        UpdateProperty(PropertyManagerConstants.PIT_STOPS_CURRENT_DURATION, TimeSpanFormatter.Format(_entry.CurrentPitStopDuration));
        UpdateProperty(PropertyManagerConstants.PIT_STOPS_LAST_DURATION, TimeSpanFormatter.Format(_entry.LastPitStopDuration));
        UpdateProperty(PropertyManagerConstants.LAPS_IN_CURRENT_STINT, _entry.LapCountInCurrentStint);
    }

    private void UpdateLapFragmentProperties()
    {
        // init the sector times from the last lap
        var s1Time = _entry.LastLapTimes.GetTimeByLapFragmentType(LapFragmentType.SECTOR_1);
        var s1Indicator = _entry.LastLapTimes.GetPaceIndicatorByLapFragmentType(LapFragmentType.SECTOR_1);

        var s2Time = _entry.LastLapTimes.GetTimeByLapFragmentType(LapFragmentType.SECTOR_2);
        var s2Indicator = _entry.LastLapTimes.GetPaceIndicatorByLapFragmentType(LapFragmentType.SECTOR_2);

        var s3Time = _entry.LastLapTimes.GetTimeByLapFragmentType(LapFragmentType.SECTOR_3);
        var s3Indicator = _entry.LastLapTimes.GetPaceIndicatorByLapFragmentType(LapFragmentType.SECTOR_3);

        var lapTime = _entry.LastLapTimes.GetTimeByLapFragmentType(LapFragmentType.FULL_LAP);
        var lapIndicator = _entry.LastLapTimes.GetPaceIndicatorByLapFragmentType(LapFragmentType.FULL_LAP);

        int currentSector = (int)_entry.CurrentSector;

        // Not every sim fills the current lap sector times. For example ACC doesn't do it on every track.
        // Therefore we only use the current lap sector times when they are available. Otherwise we only show the sector times of the last lap
        if (currentSector > 1 && _entry.CurrentLapTimes.Sector1 != null)
        {
            s1Time = _entry.CurrentLapTimes.Sector1;
            s1Indicator = _entry.CurrentLapTimes.Sector1PaceIndicator;

            s2Time = null;
            s2Indicator = null;

            s3Time = null;
            s3Indicator = null;

            if (currentSector > 2 && _entry.CurrentLapTimes.Sector2 != null)
            {
                s2Time = _entry.CurrentLapTimes.Sector2;
                s2Indicator = _entry.CurrentLapTimes.Sector2PaceIndicator;
            }
        }

        // We don't want to highlight the entry best lap time, because otherwise every best lap of any entry will be highlighted...
        var bestLapPaceIndicator = _entry.BestTimes.FullLapPaceIndicator.HasValue && _entry.BestTimes.FullLapPaceIndicator.Value != FragmentTimePaceIndicator.ENTRY_BEST ? _entry.BestTimes.FullLapPaceIndicator : null;

        UpdateProperty(PropertyManagerConstants.SECTOR_1_TIME, TimeSpanFormatter.Format(s1Time));
        UpdateProperty(PropertyManagerConstants.SECTOR_1_PACE_INDICATOR, FragmentTimePaceIndicatorConverter.ToString(s1Indicator));
        UpdateProperty(PropertyManagerConstants.SECTOR_2_TIME, TimeSpanFormatter.Format(s2Time));
        UpdateProperty(PropertyManagerConstants.SECTOR_2_PACE_INDICATOR, FragmentTimePaceIndicatorConverter.ToString(s2Indicator));
        UpdateProperty(PropertyManagerConstants.SECTOR_3_TIME, TimeSpanFormatter.Format(s3Time));
        UpdateProperty(PropertyManagerConstants.SECTOR_3_PACE_INDICATOR, FragmentTimePaceIndicatorConverter.ToString(s3Indicator));
        UpdateProperty(PropertyManagerConstants.LAP_TIME, TimeSpanFormatter.Format(lapTime));
        UpdateProperty(PropertyManagerConstants.LAP_TIME_PACE_INDICATOR, FragmentTimePaceIndicatorConverter.ToString(lapIndicator));
        UpdateProperty(PropertyManagerConstants.BEST_LAP_TIME, TimeSpanFormatter.Format(_entry.BestTimes.FullLap));
        UpdateProperty(PropertyManagerConstants.BEST_LAP_TIME_PACE_INDICATOR, FragmentTimePaceIndicatorConverter.ToString(bestLapPaceIndicator));
    }

    private void UpdateProperty<U>(string key, U value)
    {
        PropertyManager.Instance.Add(_entry.Position, key, value);
    }
}
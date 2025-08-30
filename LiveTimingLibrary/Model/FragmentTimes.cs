using System;

public class FragmentTimes
{
    public TimeSpan? Sector1 { get; set; }

    public FragmentTimePaceIndicator? Sector1PaceIndicator { get; set; }

    public TimeSpan? Sector2 { get; set; }

    public FragmentTimePaceIndicator? Sector2PaceIndicator { get; set; }

    public TimeSpan? Sector3 { get; set; }

    public FragmentTimePaceIndicator? Sector3PaceIndicator { get; set; }

    public TimeSpan? FullLap { get; set; }

    public FragmentTimePaceIndicator? FullLapPaceIndicator { get; set; }

    public TimeSpan? GetLapTimeByLapFragmentType(LapFragmentType type)
    {
        switch (type)
        {
            case LapFragmentType.SECTOR_1: return Sector1;
            case LapFragmentType.SECTOR_2: return Sector2;
            case LapFragmentType.SECTOR_3: return Sector3;
            case LapFragmentType.FULL_LAP: return FullLap;
        }

        return null;
    }


    public FragmentTimePaceIndicator? GetPaceIndicatorByLapFragmentType(LapFragmentType type)
    {
        switch (type)
        {
            case LapFragmentType.SECTOR_1: return Sector1PaceIndicator;
            case LapFragmentType.SECTOR_2: return Sector2PaceIndicator;
            case LapFragmentType.SECTOR_3: return Sector3PaceIndicator;
            case LapFragmentType.FULL_LAP: return FullLapPaceIndicator;
        }

        return null;
    }

    public FragmentTimes Clone()
    {
        return new FragmentTimes()
        {
            Sector1 = Sector1,
            Sector1PaceIndicator = Sector1PaceIndicator,
            Sector2 = Sector2,
            Sector2PaceIndicator = Sector2PaceIndicator,
            Sector3 = Sector3,
            Sector3PaceIndicator = Sector3PaceIndicator,
            FullLap = FullLap,
            FullLapPaceIndicator = FullLapPaceIndicator
        };
    }
}
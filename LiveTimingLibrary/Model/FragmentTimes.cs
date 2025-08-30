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

    /*
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (!(obj is FragmentTimes other))
            {
                return false;
            }

            // Return true if the fields match:
            return Sector1 == other.Sector1 &&
                   Sector1PaceIndicator == other.Sector1PaceIndicator &&
                   Sector2 == other.Sector2 &&
                   Sector2PaceIndicator == other.Sector2PaceIndicator &&
                   Sector3 == other.Sector3 &&
                   Sector3PaceIndicator == other.Sector3PaceIndicator &&
                   FullLap == other.FullLap &&
                   FullLapPaceIndicator == other.FullLapPaceIndicator;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    */
}
using System;
using System.Linq;

public class GapUtils
{
    public static string CalcGap(SessionEntryData a, SessionEntryData b, SessionType sessionType)
    {
        if (sessionType == SessionType.Race)
        {
            var compare = a.CompareByProgressTo(b);

            if (compare > 0)
            {
                return ToPositiveGap(CalcGapInRace(a, b)); // b is in front of a (or has the same progress);
            }
            else if (compare < 0)
            {
                return ToNegativeGap(CalcGapInRace(b, a)); // a is in front of b
            }
        }
        else
        {
            var compare = a.CompareByBestLapTimeTo(b);

            if (compare > 0)
            {
                return ToPositiveGap(CalcGapInQualiOrTraining(a, b)); // b is in front of a (or has the same best time)
            }
            else if (compare < 0)
            {
                ToNegativeGap(CalcGapInQualiOrTraining(b, a)); // a is in front of b
            }
        }

        return null;
    }

    private static string CalcGapInRace(SessionEntryData entryBehind, SessionEntryData entryInFront)
    {
        if (entryBehind == null || entryInFront == null)
        {
            return null;
        }

        // Don't calculate the gap when any of the two entries is in the pits
        if (entryBehind.IsInPit || entryInFront.IsInPit)
        {
            return null;
        }

        return CalcGapInRaceWithEntryProgress(entryBehind, entryInFront) ?? CalcGapInRaceWithSimHubData(entryBehind, entryInFront);
    }

    private static string CalcGapInRaceWithEntryProgress(SessionEntryData entryBehind, SessionEntryData entryInFront)
    {
        var latestProgress = entryBehind.EntryProgresses.LastOrDefault();

        // Cannot calculate a gap if the entry has no progress yet
        if (latestProgress == null)
        {
            return null;
        }

        // Search the latest progress of the entry in front for the same mini sector
        var progressEntryInFront = entryInFront.EntryProgresses.LastOrDefault(p => p.MiniSector == latestProgress.MiniSector);

        // Same as above, both progress must exist to calculate a gap
        if (progressEntryInFront == null)
        {
            return null;
        }

        // behind is actually in front of InFront
        if (latestProgress.CompareTo(progressEntryInFront) > 0)
        {
            return null;
        }

        var diffInLaps = progressEntryInFront.LapNumber - latestProgress.LapNumber;

        if (diffInLaps > 0)
        {
            // entry InFront has more laps and the latest mini sector is the same, but the entry in front has crossed it later.
            // Therefore we have to decrement one lap
            if (progressEntryInFront.ElapsedSessionTime.TotalSeconds > latestProgress.ElapsedSessionTime.TotalSeconds)
            {
                diffInLaps -= 1;

                if (diffInLaps == 0)
                {
                    // behind isn't lapped yet, therefore we have to fetch the progress of entry in front with the same lap number and mini sector as the entry behind
                    progressEntryInFront = entryInFront.EntryProgresses.LastOrDefault(p => p.LapNumber == latestProgress.LapNumber && p.MiniSector == latestProgress.MiniSector);

                    // Same as above, both progress must exist to calculate a gap
                    if (progressEntryInFront == null)
                    {
                        return null;
                    }
                }
            }
        }

        if (diffInLaps > 0)
        {
            // Gap is still in laps
            return ToLapGap(diffInLaps);
        }
        else
        {
            var gap = latestProgress.ElapsedSessionTime.Subtract(progressEntryInFront.ElapsedSessionTime);

            // only display positive gaps
            return gap >= TimeSpan.Zero ? TimeSpanFormatter.Format(gap) : null;
        }
    }

    private static string CalcGapInRaceWithSimHubData(SessionEntryData entryBehind, SessionEntryData entryInFront)
    {
        var diffLaps = entryInFront.CurrentLapNumber - entryBehind.CurrentLapNumber;

        if (entryBehind.TrackPositionPercent > entryInFront.TrackPositionPercent)
        {
            diffLaps--;
        }

        if (diffLaps > 0)
        {
            return ToLapGap(diffLaps);
        }
        else
        {
            var gap = TimeSpan.FromSeconds(Utils.ToSafeDouble(entryBehind.SimHubGapToLeader)).Subtract(TimeSpan.FromSeconds(Utils.ToSafeDouble(entryInFront.SimHubGapToLeader)));

            // only display positive gaps
            return gap >= TimeSpan.Zero ? TimeSpanFormatter.Format(gap) : null;
        }
    }

    private static string CalcGapInQualiOrTraining(SessionEntryData entryBehind, SessionEntryData entryInFront)
    {
        var bestLap = entryBehind.BestTimes.GetTimeByLapFragmentType(LapFragmentType.FULL_LAP);
        var inFrontBestLap = entryInFront.BestTimes.GetTimeByLapFragmentType(LapFragmentType.FULL_LAP);

        // Cannot calculate a gap if one of the entries has no best lap time yet
        if (bestLap == null || inFrontBestLap == null)
        {
            return null;
        }

        TimeSpan gap = ((TimeSpan)bestLap).Subtract((TimeSpan)inFrontBestLap);

        // only display positive gaps
        return gap.Milliseconds > 0 ? TimeSpanFormatter.Format(gap) : null;
    }

    private static string ToLapGap(int gap)
    {
        return $"{gap}L";
    }

    private static string ToNegativeGap(string gap)
    {
        return gap != null ? $"-{gap}" : null;
    }

    private static string ToPositiveGap(string gap)
    {
        return gap != null ? $"+{gap}" : null;
    }
}
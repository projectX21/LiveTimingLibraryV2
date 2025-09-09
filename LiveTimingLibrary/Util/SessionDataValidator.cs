using System;

public class SessionDataValidator
{
    public static bool IsValidSession(SessionData data)
    {
        if (!IsPlausibleLapTime(data))
        {
            return false;
        }

        return true;
    }

    public static bool IsValidEntry(SessionEntryData entry, GameTitle title)
    {
        if (!IsPlausibleLap(entry))
        {
            return false;
        }
        else if (title == GameTitle.AMS2 && !IsValidAms2Entry(entry))
        {
            return false;
        }

        return true;
    }

    /*
        public static bool IsPlausibleUpdate(SessionEntryData oldData, SessionEntryData newData)
        {
            if (oldData.CurrentLapNumber > newData.CurrentLapNumber)
        }
    */

    private static bool IsPlausibleLapTime(SessionData data)
    {
        /* Special handling for ACC:
        * ACC has the strange behavior, that it will change the lap number some milliseconds too late (and the track pos % much too late) when the lap time is already reseted to 0.
        * As a result some calculations afterwards (e.g. the session reload handling) won't work in the correct way and it's better to skip them completely.
        *
        * Example:
        * [2024-11-13 21:56:08,387] INFO - Lap: 24 - Lap time 4:08.015 - Track Pos %: 0.996
        * [2024-11-13 21:56:08,404] INFO - Lap: 25 - Lap time 0:00.177 - Track Pos %: 0.997
        * [2024-11-13 21:56:08,404] INFO - Lap: 25 - Lap time 0:00.177 - Track Pos %: 0.997
        * [2024-11-13 21:56:08,420] INFO - Lap: 25 - Lap time 0:00.612 - Track Pos %: 0.999 
        * [2024-11-13 21:56:08,420] INFO - Lap: 25 - Lap time 0:00.645 - Track Pos %: 0.000
        *
        * Furthermore LMU has the problem, that the current lap number will be 1 for quite a long time when a session gets resumed again.
        */
        if (data.PlayerCurrentLapTime.TotalSeconds < 1 && data.PlayerEntryData.TrackPositionPercent > 0.9)
        {
            return false;
        }

        return true;
    }

    private static bool IsPlausibleLap(SessionEntryData entry)
    {
        /* Special handling for ACC:
        * ACC has the strange behavior, that it will change the lap number some milliseconds too late (and the track pos % much too late) when the lap time is already reseted to 0.
        * As a result some calculations afterwards (e.g. the session reload handling) won't work in the correct way and it's better to skip them completely.
        *
        * Example:
        * [2024-11-13 21:56:08,387] INFO - Lap: 24 - Lap time 4:08.015 - Track Pos %: 0.996
        * [2024-11-13 21:56:08,404] INFO - Lap: 25 - Lap time 0:00.177 - Track Pos %: 0.997
        * [2024-11-13 21:56:08,404] INFO - Lap: 25 - Lap time 0:00.177 - Track Pos %: 0.997
        * [2024-11-13 21:56:08,420] INFO - Lap: 25 - Lap time 0:00.612 - Track Pos %: 0.999 
        * [2024-11-13 21:56:08,420] INFO - Lap: 25 - Lap time 0:00.645 - Track Pos %: 0.000
        *
        * Furthermore LMU has the problem, that the current lap number will be 1 for quite a long time when a session gets resumed again.
        */
        /*
        if (entry.CurrentLapTime.TotalSeconds < 3 && entry.TrackPositionPercent > 0.9)
        {
            SimHub.Logging.Current.Info($"SessionDataValidator::IsPlausibleLap(): 1: {entry.CurrentLapTime.TotalSeconds} - {entry.TrackPositionPercent}");
            return false;
        }
        */

        /*
        * AMS2 has the strange behavior, that it will change the lap number some milliseconds too early (see below)
        * Lap: 2 - Lap time 00:01:02.3835792 - Track Pos %: 1 - Sector1: 00:00:22.5840000
        * Lap: 3 - Lap time 00:01:02.4166136 - Track Pos %: 0.999830378025917 - Sector1: <null>

        if (entry.TrackPositionPercent > 0.9 && !entry.CurrentLapTimes.Sector1.HasValue)
        {
            SimHub.Logging.Current.Info($"SessionDataValidator::IsPlausibleLap(): 2: {entry.TrackPositionPercent} - no first sector");
            return false;
        }
        */

        return true;
    }

    private static bool IsValidAms2Entry(SessionEntryData entry)
    {
        return !entry.DisplayName.Equals("Safety Car");
    }
}
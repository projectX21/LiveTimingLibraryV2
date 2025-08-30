using System;

public class SessionDataValidator
{
    public static bool IsValidAccData(TimeSpan CurrentLapTime)
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
        */
        return CurrentLapTime.TotalSeconds >= 1;
    }
}
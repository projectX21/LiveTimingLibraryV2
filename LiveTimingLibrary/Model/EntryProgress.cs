using System;

public class EntryProgress
{
    public int LapNumber { get; set; }

    public int MiniSector { get; set; }

    public TimeSpan ElapsedSessionTime { get; set; }

    // only used for ordering the entries when the two EntryProgress are identical apart from that
    public int SimHubPosition { get; set; }

    public static int GetMiniSector(double? trackPositionPercent)
    {
        return trackPositionPercent != null
            ? (int)(trackPositionPercent * 30)
            : 1;
    }

    public EntryProgress Clone()
    {
        return new EntryProgress()
        {
            LapNumber = LapNumber,
            MiniSector = MiniSector,
            ElapsedSessionTime = ElapsedSessionTime,
            SimHubPosition = SimHubPosition
        };
    }

    public bool IdenticalLapNumberAndMiniSector(EntryProgress other)
    {
        return IdenticalLapNumberAndMiniSector(other.LapNumber, other.MiniSector);
    }

    public bool IdenticalLapNumberAndMiniSector(int lapNumber, int miniSector)
    {
        return LapNumber == lapNumber && MiniSector == miniSector;
    }

    public int CompareTo(EntryProgress other)
    {
        var diffLapNumber = LapNumber - other.LapNumber;
        var diffMiniSector = MiniSector - other.MiniSector;
        var diffElapsedTime = other.ElapsedSessionTime.CompareTo(ElapsedSessionTime);

        if (diffLapNumber != 0)
        {
            return diffLapNumber;
        }
        else if (diffMiniSector != 0)
        {
            return diffMiniSector;
        }
        else if (diffElapsedTime != 0)
        {
            return diffElapsedTime;
        }
        else
        {
            return other.SimHubPosition - SimHubPosition;
        }
    }

    public override string ToString()
    {
        return $"EntryProgress: LapNumber: {LapNumber}, MiniSector: {MiniSector}, ElapsedSessionTime: {ElapsedSessionTime}, SimHubPosition: {SimHubPosition}";
    }
}
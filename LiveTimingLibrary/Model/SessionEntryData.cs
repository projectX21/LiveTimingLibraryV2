using System;
using System.Collections.Generic;
using System.Linq;

public class SessionEntryData
{
    public string CarNumber { get; set; }

    public string DisplayName { get; set; }

    public string Manufacturer { get; set; }

    public string CarModel { get; set; }

    public string CarClass { get; set; }

    public int Position { get; set; }

    public bool IsInPit { get; set; }

    public bool IsPlayer { get; set; }

    public int CurrentLapNumber { get; set; }

    public TimeSpan CurrentLapTime { get; set; }

    public double TrackPositionPercent { get; set; }

    public string GapToLeader { get; set; }

    public string GapToClassLeader { get; set; }

    public string GapToInFront { get; set; }

    public int SimHubPosition { get; set; }

    public double? SimHubGapToLeader { get; set; }

    public FragmentTimes LastLapTimes { get; set; }

    public FragmentTimes CurrentLapTimes { get; set; }

    public FragmentTimes BestTimes { get; set; }

    public string FrontTyreCompound { get; set; }

    public string RearTyreCompound { get; set; }

    public int? StartPosition { get; set; }

    public double? FuelCapacity { get; set; }

    public double? FuelLoad { get; set; }

    public List<PitEvent> PitEvents { get; set; } = new List<PitEvent>();

    // Will be used for gap calculations. But the game has to deliver the property SessionTimeLeft, otherwise a calculation isn't possible at all and this list will be empty.
    public List<EntryProgress> EntryProgresses { get; set; } = new List<EntryProgress>();

    public LapFragmentType CurrentSector
    {
        get
        {
            return GetCurrentSector();
        }
    }

    public int LapCountInCurrentStint
    {
        get
        {
            return GetLapCountInCurrentStint();
        }
    }

    public int PitStopCount
    {
        get
        {
            return GetPitStopCount();
        }
    }

    public TimeSpan? LastPitStopDuration
    {
        get
        {
            return GetLastPitStopDuration();
        }
    }

    public TimeSpan TotalPitStopDuration
    {
        get
        {
            return GetTotalPitStopDuration();
        }
    }

    public SessionEntryData Clone()
    {
        return new SessionEntryData()
        {
            CarNumber = CarNumber,
            DisplayName = DisplayName,
            Manufacturer = Manufacturer,
            CarModel = CarModel,
            CarClass = CarClass,
            Position = Position,
            IsInPit = IsInPit,
            IsPlayer = IsPlayer,
            CurrentLapNumber = CurrentLapNumber,
            CurrentLapTime = CurrentLapTime,
            TrackPositionPercent = TrackPositionPercent,
            GapToLeader = GapToLeader,
            GapToClassLeader = GapToClassLeader,
            GapToInFront = GapToInFront,
            SimHubPosition = SimHubPosition,
            SimHubGapToLeader = SimHubGapToLeader,
            LastLapTimes = LastLapTimes?.Clone(),
            CurrentLapTimes = CurrentLapTimes?.Clone(),
            BestTimes = BestTimes?.Clone(),
            FrontTyreCompound = FrontTyreCompound,
            RearTyreCompound = RearTyreCompound,
            StartPosition = StartPosition,
            FuelCapacity = FuelCapacity,
            FuelLoad = FuelLoad,
            PitEvents = PitEvents?.Select(p => p.Clone()).ToList(),
            EntryProgresses = EntryProgresses?.Select(e => e.Clone()).ToList()
        };
    }

    public bool IsValid(GameTitle game)
    {
        if (game == GameTitle.ACC && !SessionDataValidator.IsValidAccData(CurrentLapTime))
        {
            return false;
        }

        return true;
    }

    public void AddPitEvent(PitEvent pitEvent)
    {
        if (IsPitEventAddable(pitEvent))
        {
            SimHub.Logging.Current.Debug($"SessionEntryData::AddPitEvent(): add event: {pitEvent}");
            PitEvents.Add(pitEvent);
        }
    }

    public void AddEntryProgress(EntryProgress progress)
    {
        if (IsEntryProgressAddable(progress))
        {
            EntryProgresses.Add(progress);
            ReorgEntryProgresses();
        }
    }

    public string CalcGap(SessionEntryData entryInFront, SessionType sessionType)
    {
        return GapUtils.CalcGap(this, entryInFront, sessionType);
    }

    public int CompareByProgressTo(SessionEntryData other)
    {
        var progress = EntryProgresses.LastOrDefault();
        var progressOther = other.EntryProgresses.LastOrDefault();

        if (progress != null && progressOther != null)
        {
            // The bigger the better
            return progressOther.CompareTo(progress);
        }
        else
        {
            // The smaller the better
            return SimHubPosition.CompareTo(other.SimHubPosition);
        }
    }

    public int CompareByBestLapTimeTo(SessionEntryData other)
    {
        var bestLapTime = BestTimes.FullLap;
        var bestLapTimeOther = other.BestTimes.FullLap;

        if (bestLapTime.HasValue && bestLapTimeOther.HasValue)
        {
            // The smaller the better
            return bestLapTime.Value.CompareTo(bestLapTimeOther.Value);
        }
        else
        {
            // The smaller the better
            return SimHubPosition.CompareTo(other.SimHubPosition);
        }
    }

    public void Log()
    {
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Position:           {Position}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - CarNumber:          {CarNumber}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - DisplayName:        {DisplayName}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Manufacturer:       {Manufacturer}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - CarModel:           {CarModel}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - CarClass:           {CarClass}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - IsInPit:            {IsInPit}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - IsPlayer:           {IsPlayer}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - TrackPosition%:     {TrackPositionPercent}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - CurrentLapNumber:   {CurrentLapNumber}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - CurrentSector:      {CurrentSector}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - CurrentLapTime:     {CurrentLapTime}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - GapToLeader:        {GapToLeader}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - GapToClassLeader:   {GapToClassLeader}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - GapToInFront:       {GapToInFront}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - SimHubPosition:     {SimHubPosition}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - SimHubGapToLeader:  {SimHubGapToLeader}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Last Lap S1:        {LastLapTimes?.Sector1}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Last Lap S1 PI:     {LastLapTimes?.Sector1PaceIndicator}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Last Lap S2:        {LastLapTimes?.Sector2}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Last Lap S2 PI:     {LastLapTimes?.Sector2PaceIndicator}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Last Lap S3:        {LastLapTimes?.Sector3}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Last Lap S3 PI:     {LastLapTimes?.Sector3PaceIndicator}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Last Lap FL:        {LastLapTimes?.FullLap}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Last Lap FL PI:     {LastLapTimes?.FullLapPaceIndicator}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Current Lap S1:     {CurrentLapTimes?.Sector1}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Current Lap S1 PI:  {CurrentLapTimes?.Sector1PaceIndicator}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Current Lap S2:     {CurrentLapTimes?.Sector2}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Current Lap S2 PI:  {CurrentLapTimes?.Sector2PaceIndicator}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Current Lap S3:     {CurrentLapTimes?.Sector3}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Current Lap S3 PI:  {CurrentLapTimes?.Sector3PaceIndicator}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Current Lap FL:     {CurrentLapTimes?.FullLap}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Current Lap FL PI:  {CurrentLapTimes?.FullLapPaceIndicator}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Best S1:            {BestTimes?.Sector1}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Best S2:            {BestTimes?.Sector2}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Best S3:            {BestTimes?.Sector3}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Best FL:            {BestTimes?.FullLap}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Best FL PI:         {BestTimes?.FullLapPaceIndicator}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Front Tyre:         {FrontTyreCompound}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Rear Tyre:          {RearTyreCompound}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Start position:     {StartPosition}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Fuel capacity:      {FuelCapacity}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Fuel load:          {FuelLoad}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Pit stop count:     {PitStopCount}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Laps in curr stint: {LapCountInCurrentStint}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Last pit duration:  {LastPitStopDuration}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Total pit duration: {LastPitStopDuration}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Latest progress:    {EntryProgresses.LastOrDefault()}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log() --------------------------------------------------");
    }

    public void CustomLog()
    {
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Position:           {Position}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - Latest progress:    {EntryProgresses.LastOrDefault()}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - CurrentLapNumber:   {CurrentLapNumber}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - CurrentSector:      {CurrentSector}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - TrackPosition%:     {TrackPositionPercent}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log(): {CarNumber} - CurrentLapTime:     {CurrentLapTime}");
        SimHub.Logging.Current.Debug($"SessionEntryData::Log() --------------------------------------------------");
    }

    private LapFragmentType GetCurrentSector()
    {
        if (CurrentLapTimes != null)
        {
            if (CurrentLapTimes.GetLapTimeByLapFragmentType(LapFragmentType.SECTOR_2) != null)
            {
                return LapFragmentType.SECTOR_3;
            }
            else if (CurrentLapTimes.GetLapTimeByLapFragmentType(LapFragmentType.SECTOR_1) != null)
            {
                return LapFragmentType.SECTOR_2;
            }
        }

        return LapFragmentType.SECTOR_1;
    }

    private bool IsPitEventAddable(PitEvent pitEvent)
    {
        if (pitEvent.Type == RaceEventType.PitIn)
        {
            // a PitIn event is only addable when there aren't any pit events yet, or the last one is a PitOut event
            if (PitEvents.Count > 0 && PitEvents[PitEvents.Count - 1].Type != RaceEventType.PitOut)
            {
                var message = $"SessionEntryData::IsPitEventAddable(): PitIn event is not addable: {pitEvent}! Current size of pit events: {PitEvents.Count}";

                if (PitEvents.Count > 0)
                {
                    message += $", last event type: {RaceEventTypeConverter.FromEnum(PitEvents[PitEvents.Count - 1].Type)}";
                }

                SimHub.Logging.Current.Warn(message);
                return false;
            }
        }
        else if (pitEvent.Type == RaceEventType.PitOut)
        {
            // a PitOut event is only addable when there are at least one pit event yet and the last one in the current data is a PitIn event

            if (PitEvents.Count == 0 || PitEvents[PitEvents.Count - 1].Type != RaceEventType.PitIn)
            {
                var message = $"SessionEntryData::IsPitEventAddable(): PitOut event is not addable: {pitEvent}! Current size of pit events: {PitEvents.Count}";

                if (PitEvents.Count > 0)
                {
                    message += $", last event type: {RaceEventTypeConverter.FromEnum(PitEvents[PitEvents.Count - 1].Type)}";
                }

                SimHub.Logging.Current.Warn(message);
                return false;
            }
        }

        return true;
    }

    private bool IsEntryProgressAddable(EntryProgress entryProgress)
    {
        // Is only addable if the same lap number and sector doesn't exist yet
        return !EntryProgresses.Any(ep => entryProgress.IdenticalLapNumberAndMiniSector(ep));
    }


    private int GetLapCountInCurrentStint()
    {
        var lapNumberLastPitOut = PitEvents.FindLast(e => e.Type == RaceEventType.PitOut)?.CurrentLapNumber ?? 1;
        // e.g. current lap number = 3, last pit out lap: 3 -> means the entry is the first lap of the current stint. Therefore the +1 is needed
        return CurrentLapNumber - lapNumberLastPitOut + 1;
    }

    private int GetPitStopCount()
    {
        return PitEvents.Where(e => e.Type == RaceEventType.PitIn).Count();
    }

    private TimeSpan? GetLastPitStopDuration()
    {
        var indexLastPitOut = PitEvents.FindLastIndex(e => e.Type == RaceEventType.PitOut);

        if (indexLastPitOut > 0)
        {
            var lastPitIn = PitEvents[indexLastPitOut - 1];

            if (lastPitIn.Type != RaceEventType.PitIn)
            {
                return null;
            }

            var lastPitOut = (PitOutEvent)PitEvents[indexLastPitOut];
            return PitEvent.CalcPitStopDuration((PitInEvent)lastPitIn, lastPitOut);
        }

        return null;
    }

    private TimeSpan GetTotalPitStopDuration()
    {
        TimeSpan totalPitDuration = TimeSpan.Zero;
        PitOutEvent lastPitOut = null;

        foreach (PitEvent pitEvent in PitEvents.AsEnumerable().Reverse())
        {
            if (pitEvent.Type == RaceEventType.PitOut)
            {
                lastPitOut = (PitOutEvent)pitEvent;
            }
            else if (pitEvent.Type == RaceEventType.PitIn)
            {
                if (lastPitOut != null)
                {
                    totalPitDuration.Add(PitEvent.CalcPitStopDuration((PitInEvent)pitEvent, lastPitOut));
                }
            }
        }

        return totalPitDuration;
    }

    private void ReorgEntryProgresses()
    {
        while (EntryProgresses.Count() > 90)
        {
            EntryProgresses.RemoveAt(0);
        }
    }
}
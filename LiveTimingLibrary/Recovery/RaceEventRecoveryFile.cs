using System;
using System.Collections.Generic;
using System.IO;

public class RaceEventRecoveryFile : IRaceEventRecoveryFile
{
    private readonly IRaceEventRecoveryFileEventSelector<PitEvent> _pitEventSelector;

    private readonly string _filePath;

    private static readonly string s_defaultFilePath = @"C:\Users\chris\Documents\simhub\race-event-recovery.csv";

    public RaceEventRecoveryFile(IRaceEventRecoveryFileEventSelector<PitEvent> pitEventSelector)
    {
        _filePath = s_defaultFilePath;
        _pitEventSelector = pitEventSelector;
        SimHub.Logging.Current.Debug($"RaceEventRecoveryFile(): use {_filePath} as the recovery file path");
    }

    public RaceEventRecoveryFile(IRaceEventRecoveryFileEventSelector<PitEvent> pitEventSelector, string path)
    {
        _pitEventSelector = pitEventSelector;
        _filePath = path;
        SimHub.Logging.Current.Debug($"RaceEventRecoveryFile(): use {_filePath} as the recovery file path");
    }

    public void AddEvent(RaceEvent raceEvent)
    {
        Write(raceEvent.ToRecoveryFileFormat());
    }

    public List<PitEvent> ReadPitEvents()
    {
        var pitEvents = _pitEventSelector.SelectSpecificEvents(_filePath);
        SimHub.Logging.Current.Debug($"RaceEventRecoveryFile::ReadPitEvents(): found {pitEvents.Count} events");
        return pitEvents;
    }

    public void Clear()
    {
        SimHub.Logging.Current.Debug($"RaceEventRecoveryFile::Clear()");
        File.WriteAllText(_filePath, string.Empty);
    }

    private void Write(string recoveryFileFormat)
    {
        SimHub.Logging.Current.Debug($"RaceEventRecoveryFile::Write(): write '{recoveryFileFormat}' into recovery file");
        File.AppendAllText(_filePath, recoveryFileFormat + Environment.NewLine);
    }
}
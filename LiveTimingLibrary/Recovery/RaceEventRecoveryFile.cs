using System;
using System.Collections.Generic;
using System.IO;

public class RaceEventRecoveryFile : IRaceEventRecoveryFile
{
    private readonly IRaceEventRecoveryFileEventSelector<PitEvent> _pitEventSelector;

    private readonly IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent> _playerFinishedLapEventSelector;

    private readonly string _filePath;

    private static readonly string s_defaultFilePath = @"C:\Users\chris\Documents\simhub\race-event-recovery.csv";

    public RaceEventRecoveryFile(IRaceEventRecoveryFileEventSelector<PitEvent> pitEventSelector,
                                 IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent> playerFinishedLapEventSelector)
    {
        _filePath = s_defaultFilePath;
        _pitEventSelector = pitEventSelector;
        _playerFinishedLapEventSelector = playerFinishedLapEventSelector;
        SimHub.Logging.Current.Info($"RaceEventRecoveryFile(): use {_filePath} as the recovery file path");
    }

    public RaceEventRecoveryFile(IRaceEventRecoveryFileEventSelector<PitEvent> pitEventSelector,
                                 IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent> playerFinishedLapEventSelector,
                                 string path)
    {
        _filePath = path;
        _pitEventSelector = pitEventSelector;
        _playerFinishedLapEventSelector = playerFinishedLapEventSelector;
        SimHub.Logging.Current.Info($"RaceEventRecoveryFile(): use {_filePath} as the recovery file path");
    }

    public void AddEvent(RaceEvent raceEvent)
    {
        Write(raceEvent.ToRecoveryFileFormat());
    }

    public List<PitEvent> ReadPitEvents()
    {
        var pitEvents = _pitEventSelector.SelectSpecificEvents(_filePath);
        SimHub.Logging.Current.Info($"RaceEventRecoveryFile::ReadPitEvents(): found {pitEvents.Count} events");
        return pitEvents;
    }

    public List<PlayerFinishedLapEvent> ReadPlayerFinishedLapEvents()
    {
        var playerFinishedLapEvents = _playerFinishedLapEventSelector.SelectSpecificEvents(_filePath);
        SimHub.Logging.Current.Info($"RaceEventRecoveryFile::ReadPlayerFinishedLapEvents(): found {playerFinishedLapEvents.Count} events");
        return playerFinishedLapEvents;
    }

    public void Clear()
    {
        SimHub.Logging.Current.Info($"RaceEventRecoveryFile::Clear()");
        File.WriteAllText(_filePath, string.Empty);
    }

    private void Write(string recoveryFileFormat)
    {
        SimHub.Logging.Current.Info($"RaceEventRecoveryFile::Write(): write '{recoveryFileFormat}' into recovery file");
        File.AppendAllText(_filePath, recoveryFileFormat + Environment.NewLine);
    }
}
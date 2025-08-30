using System.IO;
using Newtonsoft.Json;

public class SessionDataRecoveryFile : ISessionDataRecoveryFile
{
    private readonly string _filePath;

    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto
    };

    private static readonly string s_defaultFilePath = @"C:\Users\chris\Documents\simhub\session-data-recovery.json";

    public SessionDataRecoveryFile()
    {
        _filePath = s_defaultFilePath;
        SimHub.Logging.Current.Debug($"SessionDataRecoveryFile(): use {_filePath} as the recovery file path");

    }

    public SessionDataRecoveryFile(string filePath)
    {
        _filePath = filePath;
        SimHub.Logging.Current.Debug($"SessionDataRecoveryFile(): use {_filePath} as the recovery file path");
    }

    public void Save(SessionData sessionData)
    {
        string json = JsonConvert.SerializeObject(sessionData, _jsonSerializerSettings);
        File.WriteAllText(_filePath, json);
    }

    public SessionData Load()
    {
        SimHub.Logging.Current.Debug($"SessionDataRecoveryFile::Load(): load session data from recovery file");
        string jsonFromFile = File.ReadAllText(_filePath);
        return JsonConvert.DeserializeObject<SessionData>(jsonFromFile, _jsonSerializerSettings);
    }

    public void Clear()
    {
        SimHub.Logging.Current.Debug($"SessionDataRecoveryFile::Clear()");
        File.WriteAllText(_filePath, string.Empty);
    }
}
public interface ISessionDataRecoveryFile
{
    void Save(SessionData sessionData);

    SessionData Load();

    void Clear();
}
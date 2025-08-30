public interface ISessionDataProvider
{
    SessionData MergeWithPreviousData(SessionData newData);
}
public class SessionIdGenerator
{
    public static string Generate(GameTitle game, SessionType sessionType)
    {
        return $"{game}_{sessionType}".ToLower().Replace(" ", "-");
    }
}
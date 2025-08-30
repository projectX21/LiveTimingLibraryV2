public interface IPropertyManager
{
    void Add<T>(string key, T value);

    void Add<T>(int pos, string k, T value);

    void AddInPluginManager<T>(string key, T value);

    void ResetAll();
}
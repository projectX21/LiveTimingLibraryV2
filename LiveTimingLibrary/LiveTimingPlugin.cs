using GameReaderCommon;
using SimHub.Plugins;

namespace LiveTimingLibrary
{
    [PluginDescription("Live Timing")]
    [PluginAuthor("Cebra")]
    [PluginName("Live Timing")]
    public class LiveTimingPlugin : IPlugin, IDataPlugin
    {
        public PluginManager PluginManager { get; set; }

        private readonly LiveTimingPluginProcessor _processor;

        public LiveTimingPlugin()
        {
            var raceEntryProcessor = new RaceEntryProcessor();

            var sessionDataProvider = new SessionDataProvider(
                new SessionDataRecovery(
                    new SessionDataRecoveryFile(),
                    new RaceEventRecoveryFile(new RaceEventRecoveryFileEventSelector<PitEvent>())
                )
            );

            _processor = new LiveTimingPluginProcessor(sessionDataProvider, raceEntryProcessor);
        }

        public void Init(PluginManager pluginManager) { }

        public void DataUpdate(PluginManager pluginManager, ref GameData gameData)
        {
            _processor.DataUpdate(SessionDataConverter.ToSessionData(gameData));
        }

        public void End(PluginManager pluginManager) { }
    }
}
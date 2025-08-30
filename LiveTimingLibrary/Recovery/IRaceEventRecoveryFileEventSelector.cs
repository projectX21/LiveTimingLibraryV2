using System.Collections.Generic;

public interface IRaceEventRecoveryFileEventSelector<T> where T : RaceEvent
{
    List<T> SelectSpecificEvents(string filePath);
}
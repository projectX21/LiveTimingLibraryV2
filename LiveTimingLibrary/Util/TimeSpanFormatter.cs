using System;

public class TimeSpanFormatter
{
    public static string Format(TimeSpan? value)
    {
        if (!value.HasValue)
        {
            return "";
        }

        var hours = value.Value.Hours;
        var minutes = value.Value.Minutes;
        var seconds = value.Value.Seconds;
        var millis = value.Value.Milliseconds;

        if (hours > 0)
        {
            return hours + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2") + "." + millis.ToString("D3");
        }
        else if (minutes > 0)
        {
            return minutes.ToString() + ":" + seconds.ToString("D2") + "." + millis.ToString("D3");
        }
        else
        {
            return seconds.ToString() + "." + millis.ToString("D3");
        }
    }
}
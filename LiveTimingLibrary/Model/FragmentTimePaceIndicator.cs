public enum FragmentTimePaceIndicator
{
    CLASS_BEST,

    ENTRY_BEST
}

public class FragmentTimePaceIndicatorConverter
{
    public static string ToString(FragmentTimePaceIndicator? value)
    {
        if (value.HasValue)
        {
            switch (value.Value)
            {
                case FragmentTimePaceIndicator.CLASS_BEST:
                    return "CLASS_BEST";

                case FragmentTimePaceIndicator.ENTRY_BEST:
                    return "ENTRY_BEST";
            }
        }

        return null;
    }
}
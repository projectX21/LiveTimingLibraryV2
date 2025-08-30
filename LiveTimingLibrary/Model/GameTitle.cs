using System;

public enum GameTitle
{
    LMU,
    RF2,
    ACC,
    AMS2
}

public class GameTitleConverter
{
    public static GameTitle ToEnum(string gameName)
    {
        switch (gameName)
        {
            case "LMU":
                return GameTitle.LMU;

            case "RFactor2":
                return GameTitle.RF2;

            case "AssettoCorsaCompetizione":
                return GameTitle.ACC;

            case "Automobilista2":
                return GameTitle.AMS2;

            default:
                throw new Exception("GameTitleConverter::ToEnum(): Cannot convert value to Game: " + gameName + "!");
        }
    }
}
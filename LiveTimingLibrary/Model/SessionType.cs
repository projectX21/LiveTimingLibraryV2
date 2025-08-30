using System;

public enum SessionType
{
    Race,
    Qualifying,
    Practice
}

public class SessionTypeConverter
{
    public static SessionType ToEnum(string value)
    {
        switch (value.ToLower())
        {
            case "practice":
            case "practice 1":
            case "practice 2":
            case "practice 3":
                return SessionType.Practice;

            case "qualifying":
            case "qualify":
            case "qualifying 1":
            case "qualifying 2":
            case "qualifying 3":
                return SessionType.Qualifying;

            case "race":
                return SessionType.Race;

            default:
                throw new Exception("SessionTypeConverter::ToEnum(): Cannot convert value to SessionType: " + value + "!");
        }
    }
}
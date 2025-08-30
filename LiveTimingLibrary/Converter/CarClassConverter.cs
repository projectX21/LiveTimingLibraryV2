using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CarClassConverterEntry
{
    public GameTitle Game { get; set; }

    public string Origin { get; set; }

    public string Target { get; set; }
}

public class CarClassConverter
{
    private static readonly string s_defaultFilePath = @"C:\Users\chris\Documents\simhub\car-class-converter.csv";
    private static readonly char s_recoveryFilePatternDelimiter = ';';
    private static List<CarClassConverterEntry> s_entries;

    public static string GetConvertedCarClass(GameTitle game, string carClass)
    {
        if (s_entries == null)
        {
            s_entries = GetEntriesFromFile();
        }

        return s_entries.FirstOrDefault(e => e.Origin == carClass)?.Target ?? carClass;
    }

    private static List<CarClassConverterEntry> GetEntriesFromFile()
    {
        return File.Exists(s_defaultFilePath)
            ? File.ReadAllLines(s_defaultFilePath).Select(line => ToEntry(line)).Where(e => e != null).ToList()
            : new List<CarClassConverterEntry>();
    }

    private static CarClassConverterEntry ToEntry(string line)
    {
        if ((line.Trim() ?? "") == "" || line.Trim().StartsWith("#"))
        {
            return null;
        }

        var tokens = line.Split(s_recoveryFilePatternDelimiter);

        if (tokens.Length != 3)
        {
            throw new Exception("CarClassConverterEntry(): Cannot init CarClassConverterEntry! Invalid line:  " + line);
        }

        return new CarClassConverterEntry()
        {
            Game = GameTitleConverter.ToEnum(tokens[0].Trim()),
            Origin = tokens[1].Trim(),
            Target = tokens[2].Trim()
        };
    }
}
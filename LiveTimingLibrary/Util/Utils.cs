using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class Utils
{
    public static double ToSafeDouble(double? value)
    {
        return (value.HasValue && !double.IsNaN(value.Value)) ? value.Value : 0.0;
    }

    public static string ByteArrayToString(byte[] byteArray)
    {
        return new string(
            System.Text.Encoding.Default.GetString(byteArray).Where(c => !char.IsControl(c)).ToArray()
        );
    }

    public static List<string> ReadLinesFromFile(string fileName)
    {
        if (File.Exists(fileName))
        {
            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                var lines = new List<string>();
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
                return lines;
            }
        }
        else
        {
            throw new Exception($"Cannot read file: {fileName}!");
        }
    }


    public static void PrintAllFieldsAndProperties(object obj)
    {
        if (obj == null)
        {
            return;
        }

        Type type = obj.GetType();
        SimHub.Logging.Current.Info($"Typ: {type.FullName}");

        // Alle Felder, auch private und vererbte
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        SimHub.Logging.Current.Info("Felder:");
        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(obj);
            SimHub.Logging.Current.Info($"- {field.Name} = {value} (Zugriff: {field.Attributes})");
        }

        // Alle Properties, auch vererbte
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        SimHub.Logging.Current.Info("Properties:");
        foreach (PropertyInfo prop in properties)
        {
            try
            {
                object value = prop.GetValue(obj, null);
                SimHub.Logging.Current.Info($"- {prop.Name} = {value}");
            }
            catch (Exception ex)
            {
                SimHub.Logging.Current.Info($"- {prop.Name} konnte nicht gelesen werden: {ex.Message}");
            }
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using AcTools.Utils.Helpers;

public class Statement
{
    public Dictionary<string, string[]> Conditions { get; set; } = new Dictionary<string, string[]>();

    public Dictionary<string, string> Actions { get; set; } = new Dictionary<string, string>();

    public void ModifyMatchingEntry(SessionEntryData entry, GameTitle game)
    {
        if (EntryMatchesConditions(entry, game))
        {
            Actions.ForEach(action => entry.SetValueByProperty(action.Key, action.Value));
        }
    }

    private bool EntryMatchesConditions(SessionEntryData entry, GameTitle game)
    {
        return Conditions.All(condition =>
        {
            // Game is the only condition which isn't applied on the entry itself
            if (condition.Key.Equals("Game", StringComparison.OrdinalIgnoreCase))
            {
                return condition.Value.Any(conditionValue => game == GameTitleConverter.ToEnum(conditionValue));
            }

            var actualValue = entry.GetValueByProperty(condition.Key);
            return condition.Value.Any(conditionValue => actualValue.Equals(conditionValue, StringComparison.OrdinalIgnoreCase));
        });
    }

}

public class SessionEntryDataCustomStatementConverter
{
    private static readonly string s_defaultFilePath = @"C:\Users\chris\Documents\simhub\custom-statement-converter.txt";

    private static List<Statement> _statements;

    public static void ModifyMatchingEntry(SessionEntryData entry, GameTitle game)
    {
        if (_statements == null)
        {
            Init();
        }

        _statements.ForEach(statement =>
        {
            statement.ModifyMatchingEntry(entry, game);
        });
    }

    private static void Init()
    {
        SimHub.Logging.Current.Info("SessionEntryDataCustomStatementConverter::Init()");
        InitStatements();
        ListenToFileContentChanges();
    }

    private static void ListenToFileContentChanges()
    {
        var watcher = new FileSystemWatcher
        {
            Path = Path.GetDirectoryName(s_defaultFilePath),
            Filter = Path.GetFileName(s_defaultFilePath),
            NotifyFilter = NotifyFilters.LastWrite,
        };

        watcher.Created += OnFileContentChanged;
        watcher.Changed += OnFileContentChanged;
        watcher.Deleted += OnFileContentChanged;
        watcher.EnableRaisingEvents = true;
    }

    private static void OnFileContentChanged(object source, FileSystemEventArgs e)
    {
        SimHub.Logging.Current.Info("SessionEntryDataCustomStatementConverter::OnFileContentChanged(): File content has changed");
        InitStatements();
    }

    private static void InitStatements()
    {
        _statements = Utils.ReadLinesFromFile(s_defaultFilePath).Select(line => LineToStatement(line)).Where(e => e != null).ToList();
        SimHub.Logging.Current.Info($"SessionEntryDataCustomStatementConverter::InitStatements(): Initialized {_statements.Count()} statements");
    }

    private static Statement LineToStatement(string line)
    {
        // CarClass = GTE, Manufacturer = Porsche WHERE CarNumber = 14
        if ((line.Trim() ?? "") == "" || line.Trim().StartsWith("#"))
        {
            return null;
        }

        if (!line.Trim().ToLower().StartsWith("set "))
        {
            SimHub.Logging.Current.Warn($"SessionEntryDataCustomStatementConverter::LineToStatement(): Invalid line: {line}. Missing 'SET' command.");
            return null;
        }

        var indexWhere = line.ToLower().IndexOf(" where ");

        if (indexWhere == -1)
        {
            SimHub.Logging.Current.Warn($"SessionEntryDataCustomStatementConverter::LineToStatement(): Invalid line: {line}. Missing 'WHERE' command.");
            return null;
        }

        var conditions = GetConditionPairs(line.Substring(indexWhere + 7));

        if (conditions == null)
        {
            SimHub.Logging.Current.Warn($"SessionEntryDataCustomStatementConverter::LineToStatement(): Invalid conditions in line: {line}");
            return null;
        }

        var actions = GetActionPairs(line.Substring(4, indexWhere - 4));

        if (actions == null)
        {
            SimHub.Logging.Current.Warn($"SessionEntryDataCustomStatementConverter::LineToStatement(): Invalid actions in line: {line}");
            return null;
        }

        return new Statement()
        {
            Conditions = conditions,
            Actions = actions
        };
    }

    private static Dictionary<string, string[]> GetConditionPairs(string text)
    {
        var pairs = new Dictionary<string, string[]>();

        foreach (var pair in text.Trim().Split(','))
        {
            var tokens = pair.Split(new[] { '=' }, 2);

            if (tokens.Length == 2)
            {
                pairs.Add(tokens[0].Trim(), tokens[1].Trim().Split('|').Select(t => t.Trim()).ToArray());
            }
            else
            {
                SimHub.Logging.Current.Warn($"SessionEntryDataCustomStatementConverter::GetConditionPairs(): Invalid pair: {pair}");
                return null;
            }
        }

        return pairs;
    }

    private static Dictionary<string, string> GetActionPairs(string text)
    {
        var pairs = new Dictionary<string, string>();

        foreach (var pair in text.Trim().Split(','))
        {
            var tokens = pair.Split(new[] { '=' }, 2);

            if (tokens.Length == 2)
            {
                pairs.Add(tokens[0].Trim(), tokens[1].Trim());
            }
            else
            {
                SimHub.Logging.Current.Warn($"SessionEntryDataCustomStatementConverter::GetActionPairs(): Invalid pair: {pair}");
                return null;
            }
        }

        return pairs;
    }
}
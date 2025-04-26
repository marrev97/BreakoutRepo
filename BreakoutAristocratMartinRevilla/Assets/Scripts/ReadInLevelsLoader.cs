using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int columns;
    public int rows;
    public float powerUpProbability;
}

public class ReadInLevelsLoader
{
    private string defaultLevelPath = "CustomLevels/levels";
    private List<LevelData> allLevels = new List<LevelData>();
    public int LevelCount => allLevels.Count;

    public ReadInLevelsLoader()
    {
        LoadAllLevels(defaultLevelPath);
    }

    private void LoadAllLevels(string path = null)
    {
        TextAsset levelFile = Resources.Load<TextAsset>(path);
        if (levelFile == null)
        {
            Debug.LogWarning($"Level file not found at {path}");
            return;
        }
        ParseAllLevelData(levelFile.text);
    }

    private void ParseAllLevelData(string levelText)
    {
        allLevels.Clear();

        if (string.IsNullOrWhiteSpace(levelText))
        {
            Debug.LogWarning("Level text is empty or null");
            return;
        }

        // Split text into lines, removing empty lines
        string[] lines = levelText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        List<string> levelSections = new List<string>();
        List<string> currentSection = new List<string>();

        // Group lines into sections based on level headers
        foreach (string line in lines)
        {
            string trimmedLine = line.TrimStart();
            // Check if the line is a level header (starts with 'l' followed by digits)
            if (trimmedLine.StartsWith("l") && IsLevelHeader(trimmedLine))
            {
                // If we have a current section, save it
                if (currentSection.Count > 0)
                {
                    levelSections.Add(string.Join("\n", currentSection));
                    currentSection.Clear();
                }
            }
            currentSection.Add(line);
        }

        // Add the last section if it exists
        if (currentSection.Count > 0)
        {
            levelSections.Add(string.Join("\n", currentSection));
        }

        // Parse each valid section
        foreach (string section in levelSections)
        {
            if (!string.IsNullOrWhiteSpace(section))
            {
                allLevels.Add(ParseLevelSection(section));
            }
        }
    }

    private bool IsLevelHeader(string line)
    {
        // Ensure the line starts with 'l' followed by digits
        if (!line.StartsWith("l") || line.Length < 2)
            return false;

        string rest = line.Substring(1);
        int spaceIndex = rest.IndexOf(' ');
        if (spaceIndex >= 0)
        {
            rest = rest.Substring(0, spaceIndex);
        }

        // Check if the rest is all digits and non-empty
        return rest.Length > 0 && rest.All(char.IsDigit);
    }

    public LevelData GetLevel(int levelNumber)
    {
        return allLevels.FirstOrDefault(l => l.levelNumber == levelNumber);
    }

    private LevelData ParseLevelSection(string section)
    {
        string[] lines = section.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        string[] headerParts = lines[0].Split();

        return new LevelData
        {
            levelNumber = int.Parse(headerParts[0].Substring(1)),
            columns = headerParts.Any(p => p.StartsWith("C")) ?
                      int.Parse(headerParts.First(p => p.StartsWith("C")).Substring(1)) : 10,
            rows = headerParts.Any(p => p.StartsWith("R")) ?
                   int.Parse(headerParts.First(p => p.StartsWith("R")).Substring(1)) : 5,

            powerUpProbability = FormatNumber(int.Parse(headerParts.Last()))
        };
    }

    public bool TryGetLevel(int levelNumber, out LevelData levelData)
    {
        levelData = allLevels.FirstOrDefault(l => l.levelNumber == levelNumber);

        if (levelData == null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Formats number from binary to base 10
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>

    public static int FormatNumber(int number)
    {
        return Convert.ToInt32(number.ToString(), 2);
    }
}
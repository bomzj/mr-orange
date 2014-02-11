using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using JsonFx.Json;

namespace PushBlock.Helpers
{
    public class HighScoreEntry
    {
        public int LevelNumber;
        public int GoldStars;
        public int Score;

        public float Time;

        public static bool operator <(HighScoreEntry score1, HighScoreEntry score2)
        {
            return score1.Score < score2.Score;
        }

        public static bool operator >(HighScoreEntry score1, HighScoreEntry score2)
        {
            return score1.Score > score2.Score;
        }
    }

    class HighScore
    {
        const string scoreFile = "score.xml";

        static List<HighScoreEntry> scoreEntries = new List<HighScoreEntry>();

        static HighScore()
        {
            // Load score or create empty score list
            string json = PlayerPrefs.GetString(scoreFile);

            // Load settings or create empty settings
            if (!string.IsNullOrEmpty(json))
            {
                var obj = JsonReader.Deserialize<HighScoreEntry[]>(json);
                scoreEntries = obj.ToList();
            }
        }

        public static void Save()
        {
            string highscoreJson = JsonWriter.Serialize(scoreEntries);
            PlayerPrefs.SetString(scoreFile, highscoreJson);
            PlayerPrefs.Save();
        }

        public static HighScoreEntry GetSavedHighScore(int levelNumber)
        {
            return scoreEntries.FirstOrDefault((scoreEntry) => scoreEntry.LevelNumber == levelNumber);
        }

        public static void AddHighScore(HighScoreEntry scoreEntry)
        {
            HighScoreEntry existedScoreEntry = GetSavedHighScore(scoreEntry.LevelNumber);
            if (existedScoreEntry != null) scoreEntries.Remove(existedScoreEntry);
            scoreEntries.Add(scoreEntry);
        }

        public static HighScoreEntry CalculateScore(Level level)
        {
            HighScoreEntry scoreEntry = new HighScoreEntry();
            scoreEntry.LevelNumber = level.LevelNumber;

            // Set Gold stars achivement
            if (level.ElapsedTime.TotalSeconds <= level.AwesomeAwardTime)
            {
                scoreEntry.GoldStars = 3;
            }
            else if (level.ElapsedTime.TotalSeconds <= level.AwesomeAwardTime * 1.25f)
            {
                scoreEntry.GoldStars = 2;
            }
            else if (level.ElapsedTime.TotalSeconds <= level.AwesomeAwardTime * 1.5f)
            {
                scoreEntry.GoldStars = 1;
            }

            scoreEntry.Score = (int)(100 * level.AwesomeAwardTime / level.ElapsedTime.TotalSeconds);

            scoreEntry.Time =  (float)level.ElapsedTime.TotalSeconds;

            return scoreEntry;
        }
    }
}

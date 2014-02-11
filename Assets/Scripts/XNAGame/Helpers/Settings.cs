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
    public class Pair<K,V>
    {
        public K Key { get; set; }
        public V Value { get; set; }

        public Pair() {}
        public Pair(K x, V y)
        {
            Key = x;
            Value = y;
        }
    }
    
    static class Settings
    {
        const string settingsFile = "settings.xml";
        static List<Pair<string, string>> items = new List<Pair<string, string>>();

        static Settings()
        {
            var settingsJsonString = PlayerPrefs.GetString(settingsFile);

            // Load settings or create empty settings
            if (!string.IsNullOrEmpty(settingsJsonString))
            {
                var obj = JsonFx.Json.JsonReader.Deserialize<Pair<string, string>[]>(settingsJsonString);
                items = obj.ToList();
            }
        }
        
        public static string GetValue(string key)
        {

            var found = items.FirstOrDefault(kvp => kvp.Key == key);
            return found != null ? found.Value: null;
        }

        public static void SetValue(string key, string value)
        {
            Pair<string, string> found = items.FirstOrDefault(kvp => kvp.Key == key);
            if (found != null) items.Remove(found);
            items.Add(new Pair<string, string>(key, value));
        }

        public static void Save()
        {
            string settingsJsonString = JsonWriter.Serialize(items);
            PlayerPrefs.SetString(settingsFile, settingsJsonString);
            PlayerPrefs.Save();
        }
    }
}

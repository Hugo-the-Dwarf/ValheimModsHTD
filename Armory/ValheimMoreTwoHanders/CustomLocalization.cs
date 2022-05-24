using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ValheimHTDArmory
{
    public class CustomLocalization
    {
        string currentLanguage = "English";
        string fileSuffix = "_localization";
        public Dictionary<int, string> localizationNames = new();
        public Dictionary<int, string> localizationDescriptions = new();
        List<int> prefabHashes = new();
        List<string> localizationLines = new();

        public CustomLocalization()
        {
            string @string = PlayerPrefs.GetString("language", "");
            if (@string != "")
            {
                currentLanguage = @string;
            }
        }

        public void LoadLocalization(string bepinexConfigPath)
        {
            string path = bepinexConfigPath + fileSuffix + ".tsv";
            bool fileFound = false;

            try
            {
                using (StreamReader reader = new(path))
                {
                    ReadLocalization(reader);
                    fileFound = true;
                }
            }
            catch (IOException ioEx)
            {
                Plugin.Log.LogError($"An IO Exception was thrown. [{fileSuffix}]");
                Plugin.Log.LogError(ioEx.Message);
                Plugin.Log.LogError(ioEx.StackTrace);
            }
            if (!fileFound)
                ReadDefaultLocalization();

            WriteLocalization(path);
            ParseLocalization();
        }

        private void ParseLocalization()
        {
            int languageIndex = -1;
            bool firstPass = true;
            int currentLineIndex = 0;
            foreach (string dataString in localizationLines)
            {
                try
                {
                    string[] splitData = dataString.Split('\t');
                    if (firstPass)
                    {
                        firstPass = false;
                        for (int i = 1; i < splitData.Length; i++)//skip index 0 because that's blank anyways
                        {
                            if (splitData[i] == currentLanguage)
                            {
                                languageIndex = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (languageIndex > -1)
                        {
                            FilterData(splitData, languageIndex);
                        }
                    }
                    currentLineIndex++;
                }
                catch(Exception e)
                {
                    Plugin.Log.LogError($"Exception Thrown trying to localize line {currentLineIndex}: <{dataString}>");
                    Plugin.Log.LogError($"Exception Message: <{e.Message}>");
                }
            }
        }

        private void FilterData(string[] splitData, int languageIndex)
        {
            int prefabHash = splitData[0].Substring(0, splitData[0].IndexOf('_')).GetStableHashCode();
            prefabHashes.Remove(prefabHash);
            prefabHashes.Add(prefabHash);
            if (splitData[0].Contains("_Name"))
            {
                localizationNames[prefabHash] = splitData[languageIndex];
            }
            if (splitData[0].Contains("_Desc"))
            {
                localizationDescriptions[prefabHash] = splitData[languageIndex];
            }
        }

        private void WriteLocalization(string path)
        {
            StringBuilder csvBuilder = new();
            foreach (string line in localizationLines)
            {
                csvBuilder.AppendLine(line);
            }

            using (StreamWriter writer = new(path))
            {
                writer.Write(csvBuilder.ToString());
            }
        }

        private void ReadLocalization(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                localizationLines.Add(reader.ReadLine());
            }
        }

        private void ReadDefaultLocalization()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string manifestName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("localization.tsv"));
            using (Stream stream = assembly.GetManifestResourceStream(manifestName))
            {
                using (StreamReader reader = new(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string output = reader.ReadLine();
                        //output = output.Replace('\t', ':'); //Don't convert from .tsv to be split by ':' anymore
                        localizationLines.Add(output);
                    }
                }
            }
        }

        public void TryLocaliazeItem(string prefabName, ref ItemDrop id)
        {
            int prefabHash = prefabName.GetStableHashCode();
            var shared = id.m_itemData.m_shared;
            if (localizationNames.ContainsKey(prefabHash) && localizationNames[prefabHash].Trim() != string.Empty)
            {
                shared.m_name = localizationNames[prefabHash];
            }
            if (localizationDescriptions.ContainsKey(prefabHash) && localizationDescriptions[prefabHash].Trim() != string.Empty)
            {
                shared.m_description = localizationDescriptions[prefabHash];
            }
        }

        public void TryLocalizeStatusEffect(string prefabName, ref StatusEffect se)
        {
            int prefabHash = prefabName.GetStableHashCode();
            if (localizationNames.ContainsKey(prefabHash) && localizationNames[prefabHash].Trim() != string.Empty)
            {
                se.m_name = localizationNames[prefabHash];
            }
            if (localizationDescriptions.ContainsKey(prefabHash) && localizationDescriptions[prefabHash].Trim() != string.Empty)
            {
                se.m_tooltip = localizationDescriptions[prefabHash];
            }
        }

    }
}
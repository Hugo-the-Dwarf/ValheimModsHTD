using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ValheimMoreTwoHanders
{
    public class CustomLocalization
    {
        string currentLanguage = "English";
        string fileSuffix = "_localization";
        public Dictionary<string, string> localizationNames = new Dictionary<string, string>();
        public Dictionary<string, string> localizationDescriptions = new Dictionary<string, string>();
        List<string> prefabNames = new List<string>();
        List<string> localizationLines = new List<string>();

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
                using (StreamReader reader = new StreamReader(path))
                {
                    ReadLocalization(reader);
                    fileFound = true;
                }
            }
            catch (FileNotFoundException)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path.Replace("tsv", "cfg")))
                    {
                        ReadLocalization(reader);
                        fileFound = true;
                        reader.Close();
                        File.Delete(path.Replace("tsv", "cfg"));
                    }
                }
                catch (FileNotFoundException)
                {
                    Plugin.Log.LogWarning($"Failed to find '{Plugin.GUID + fileSuffix}'/.cfg in path '{bepinexConfigPath}' will create config of same name with default values.");
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
            foreach (string dataString in localizationLines)
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
            }
        }

        private void FilterData(string[] splitData, int languageIndex)
        {
            string prefabName = splitData[0].Substring(0, splitData[0].IndexOf('_'));
            prefabNames.Remove(prefabName);
            prefabNames.Add(prefabName);
            if (splitData[0].Contains("_Name"))
            {
                localizationNames.Remove(prefabName);
                localizationNames.Add(prefabName, splitData[languageIndex]);
            }
            if (splitData[0].Contains("_Desc"))
            {
                localizationDescriptions.Remove(prefabName);
                localizationDescriptions.Add(prefabName, splitData[languageIndex]);
            }
        }

        private void WriteLocalization(string path)
        {
            StringBuilder csvBuilder = new StringBuilder();
            foreach (string line in localizationLines)
            {
                csvBuilder.AppendLine(line);
            }

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(csvBuilder.ToString());
            }
        }

        private void ReadLocalization(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                //ToDo remove this replace feature in the future.
                //I have no idea why I just didn't leave it as a .tsv when writing
                localizationLines.Add(reader.ReadLine().Replace(':', '\t'));
            }
        }

        private void ReadDefaultLocalization()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string manifestName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("localization.tsv"));
            using (Stream stream = assembly.GetManifestResourceStream(manifestName))
            {
                using (StreamReader reader = new StreamReader(stream))
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
            var shared = id.m_itemData.m_shared;
            if (localizationNames.ContainsKey(prefabName) && localizationNames[prefabName].Trim() != string.Empty)
            {
                shared.m_name = localizationNames[prefabName];
            }
            if (localizationDescriptions.ContainsKey(prefabName) && localizationDescriptions[prefabName].Trim() != string.Empty)
            {
                shared.m_description = localizationDescriptions[prefabName];
            }
        }

    }
}
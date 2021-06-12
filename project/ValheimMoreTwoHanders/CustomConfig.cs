using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using ServerSync;

namespace ValheimMoreTwoHanders
{
    public class CustomConfig
    {
        public List<ConfigItemData> itemConfigs = new List<ConfigItemData>();
        public List<ConfigRecipeData> recipeConfigs = new List<ConfigRecipeData>();

        private Dictionary<string, ConfigItemData> itemConfigsToApply = new Dictionary<string, ConfigItemData>();
        private Dictionary<string, ConfigRecipeData> recipeConfigsToApply = new Dictionary<string, ConfigRecipeData>();

        public CustomSyncedValue<string> syncedItemConfigsToApply = new CustomSyncedValue<string>(Plugin.configSync, "itemConfigs");
        public CustomSyncedValue<string> syncedRecipeConfigsToApply = new CustomSyncedValue<string>(Plugin.configSync, "recipeConfigs");

        private bool itemConfigFound = false;
        private bool recipeConfigFound = false;

        private string itemConfigSuffix = "_itemdata";
        private string recipeConfigSuffix = "_recipeData";
        //string modConfigSuffix = "_main"; // not used for now, I'll just use BepinEx's Configmanager for the simple values

        public CustomConfig()
        {
            syncedItemConfigsToApply.ValueChanged += ItemConfigsChanged;
            syncedRecipeConfigsToApply.ValueChanged += RecipeConfigChanged;
        }

        private void ItemConfigsChanged()
        {
            itemConfigs.Clear();
            itemConfigsToApply.Clear();
            foreach (ConfigItemData itemData in DeserializeItemDataConfig(syncedItemConfigsToApply.Value))
            {
                itemConfigs.Add(itemData);
                if (!itemConfigsToApply.ContainsKey(itemData.ItemPrefab))
                    itemConfigsToApply.Add(itemData.ItemPrefab, itemData);
            }
            ItemManager.ApplySyncedItemConfigData();
            //Plugin.RebuildCustomAssetLists();
        }

        private void RecipeConfigChanged()
        {
            recipeConfigs.Clear();
            recipeConfigsToApply.Clear();
            foreach (ConfigRecipeData recipeData in DeserializeRecipeDataConfig(syncedRecipeConfigsToApply.Value))
            {
                recipeConfigs.Add(recipeData);
                if (!recipeConfigsToApply.ContainsKey(recipeData.ItemPrefab))
                    recipeConfigsToApply.Add(recipeData.ItemPrefab, recipeData);
            }
            Plugin.RebuildRecipes();
            //Plugin.RebuildCustomAssetLists();
        }

        public bool UsingDefaultItemConfig(GameObject go)
        {
            return !itemConfigFound;
        }

        public bool UsingDefaultRecipeConfig(GameObject go)
        {
            return !recipeConfigFound;
        }

        //Do the same for finding and removing from toApplyList, and recipes
        public void AddItemDataAsConfigRecord(GameObject go)
        {
            ConfigItemData newData = new ConfigItemData();
            newData.ReadConfigDataFromItem(go);
            itemConfigs.RemoveAll(ic => ic.ItemPrefab == go.name);
            itemConfigs.Add(newData);
        }

        public void ApplyItemDataFromConfigRecord(ref GameObject go)
        {
            if (itemConfigsToApply.Count > 0 && itemConfigsToApply.ContainsKey(go.name))
            {
                ItemDrop id = go.GetComponent<ItemDrop>();
                itemConfigsToApply[go.name].WriteConfigDataToItem(ref id);
                itemConfigsToApply.Remove(go.name);
            }
        }

        public void AddRecipeAsConfigRecord(RecipeHelper rh)
        {
            ConfigRecipeData newData = new ConfigRecipeData();
            var existingRecord = recipeConfigs.Where(rc => rc.ItemPrefab == rh.GetPrefabName()).FirstOrDefault();
            if (existingRecord != null)
            {
                newData.Enabled = existingRecord.Enabled;
            }            
            newData.ReadConfigFromRecipeHelper(rh);
            recipeConfigs.RemoveAll(rc => rc.ItemPrefab == rh.GetPrefabName());
            recipeConfigs.Add(newData);
        }

        public RecipeHelper ApplyRecipeHelperFromConfigRecord(GameObject go)
        {
            if (recipeConfigsToApply.Count > 0 && recipeConfigsToApply.ContainsKey(go.name))
            {
                RecipeHelper outputRH = recipeConfigsToApply[go.name].LoadConfigedRecipeHelper(go);
                recipeConfigsToApply.Remove(go.name);
                return outputRH;
            }
            return null;
        }

        public IEnumerable<ConfigItemData> DeserializeItemDataConfig(string data)
        {
            if (data.Trim().Length > 0)
            {
                data = data.Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
                string[] splitData = data.Split('@');
                if (splitData.Length > 0)
                {
                    foreach (string itemData in splitData)
                    {
                        yield return JsonUtility.FromJson<ConfigItemData>(itemData);
                    }
                }
            }
        }

        public IEnumerable<ConfigRecipeData> DeserializeRecipeDataConfig(string data)
        {
            data = data.Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
            if (data.Trim().Length > 0)
            {
                string[] splitData = data.Split('@');
                if (splitData.Length > 0)
                {
                    for (int i = 0; i < splitData.Length; i++)
                    {
                        string frontHalf = splitData[i].Substring(0, splitData[i].IndexOf("\"CraftingRequirements\":"));
                        string backHalf = splitData[i].Substring(splitData[i].IndexOf("\"CraftingRequirements\":"));
                        frontHalf += "\"CraftingRequirements\": \"\"}";
                        backHalf = backHalf.Substring(backHalf.IndexOf(":") + 1);
                        backHalf = backHalf.Remove(backHalf.Length - 1);
                        ConfigRecipeData itemData = JsonUtility.FromJson<ConfigRecipeData>(frontHalf);
                        string[] splitResourceData = backHalf.Split('#');
                        if (splitResourceData.Length > 0)
                        {
                            List<ConfigResource> res = new List<ConfigResource>();
                            for (int j = 0; j < splitResourceData.Length; j++)
                            {
                                res.Add(JsonUtility.FromJson<ConfigResource>(splitResourceData[j]));
                            }
                            itemData.CraftingRequirementsArray = res.ToArray();
                        }
                        yield return itemData;
                    }
                }
            }
        }

        public void LoadInitialConfigs(string bepinexConfigPath)
        {
            bool Load(string fileSuffix, CustomSyncedValue<string> configValue)
            {
                string path = bepinexConfigPath + fileSuffix + ".cfg";
                void consumeConfigFileEvent(object sender, FileSystemEventArgs args) => LoadConfig(path, configValue);

                FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(path), Path.GetFileName(path));
                watcher.Changed += consumeConfigFileEvent;
                watcher.Created += consumeConfigFileEvent;
                watcher.Renamed += consumeConfigFileEvent;
                watcher.IncludeSubdirectories = true;
                watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
                watcher.EnableRaisingEvents = true;

                return LoadConfig(path, configValue);
            }

            itemConfigFound = Load(itemConfigSuffix, syncedItemConfigsToApply);
            recipeConfigFound = Load(recipeConfigSuffix, syncedRecipeConfigsToApply);
        }

        public bool LoadConfig(string bepinexConfigPath, CustomSyncedValue<string> configValue)
        {
            try
            {
                using (StreamReader sr = new StreamReader(bepinexConfigPath))
                {
                    configValue.AssignLocalValue(sr.ReadToEnd());
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                Plugin.Log.LogWarning($"Failed to find '{recipeConfigSuffix}' in path '{bepinexConfigPath}' will create config of same name with default values.");
            }
            catch (IOException ioEx)
            {
                Plugin.Log.LogError($"An IO Exception was thrown. [{recipeConfigSuffix}]");
                Plugin.Log.LogError(ioEx.Message);
                Plugin.Log.LogError(ioEx.StackTrace);
            }

            return false;
        }

        public string SerializeItemDataConfig()
        {
            StringBuilder sbOutput = new StringBuilder();
            foreach (ConfigItemData cid in itemConfigs)
            {
                if (sbOutput.Length > 0) sbOutput.Append("\r\n@\r\n");
                sbOutput.Append(JsonUtility.ToJson(cid, true));
            }

            return sbOutput.ToString();
        }

        public string SerializeRecipeDataConfig()
        {
            StringBuilder sbOutput = new StringBuilder();
            foreach (ConfigRecipeData crd in recipeConfigs)
            {
                if (sbOutput.Length > 0) sbOutput.Append("\r\n@\r\n");
                if (crd.CraftingRequirementsArray.Length > 0)
                {
                    //crd.CraftingRequirements = "[";
                    bool firstResourceDone = false;
                    foreach (ConfigResource cr in crd.CraftingRequirementsArray)
                    {
                        if (firstResourceDone) crd.CraftingRequirements += "#";
                        crd.CraftingRequirements += JsonUtility.ToJson(cr);
                        firstResourceDone = true;
                    }
                    //crd.CraftingRequirements += "]";
                }
                string crdJSONString = JsonUtility.ToJson(crd, true);
                crdJSONString = crdJSONString.Replace("\"{", "\r\n\t{\r\n\t\t");
                crdJSONString = crdJSONString.Replace("}\"", "\r\n\t}");
                crdJSONString = crdJSONString.Replace(",\\\"", ",\r\n\t\t\"");
                crdJSONString = crdJSONString.Replace("\\\"", "\"");
                crdJSONString = crdJSONString.Replace("}#{", "\r\n\t}\r\n\t#\r\n\t{\r\n\t\t");
                sbOutput.Append(crdJSONString);
            }

            return sbOutput.ToString();
        }

        public void WriteConfigs(string bepinexConfigPath)
        {
            try
            {
                if (itemConfigs.Count > 0)
                {
                    using (StreamWriter sw = new StreamWriter(bepinexConfigPath + itemConfigSuffix + ".cfg")) sw.Write(SerializeItemDataConfig());
                }
            }
            catch (IOException ioEx)
            {
                Plugin.Log.LogError($"An IO Exception was thrown. [{itemConfigSuffix}]");
                Plugin.Log.LogError(ioEx.Message);
                Plugin.Log.LogError(ioEx.StackTrace);
            }

            try
            {
                if (recipeConfigs.Count > 0)
                {
                    using (StreamWriter sw = new StreamWriter(bepinexConfigPath + recipeConfigSuffix + ".cfg")) sw.Write(SerializeRecipeDataConfig());
                }
            }
            catch (IOException ioEx)
            {
                Plugin.Log.LogError($"An IO Exception was thrown. [{recipeConfigSuffix}]");
                Plugin.Log.LogError(ioEx.Message);
                Plugin.Log.LogError(ioEx.StackTrace);
            }
        }

        [Serializable]
        public class ConfigItemData
        {
            public string ItemPrefab;
            public int MaxQuality = 4;
            public float BlockAmount = 0f;
            public float BlockAmountPerLevel = 0f;
            public float DeflectionForce = 0f;
            public float DeflectionForcePerLevel = 0f;
            public float ParryBonus = 3f;
            public float Knockback_Power = 0f;
            public float Backstab_Bonus = 3f;
            public float DamageBlunt = 0f;
            public float DamageSlash = 0f;
            public float DamagePierce = 0f;
            public float DamageChop = 0f;
            public float DamagePickaxe = 0f;
            public float DamageFire = 0f;
            public float DamageFrost = 0f;
            public float DamageLightning = 0f;
            public float DamagePoison = 0f;
            public float DamageSpirit = 0f;
            public float DamageBluntPerLevel = 0f;
            public float DamageSlashPerLevel = 0f;
            public float DamagePiercePerLevel = 0f;
            public float DamageChopPerLevel = 0f;
            public float DamagePickaxePerLevel = 0f;
            public float DamageFirePerLevel = 0f;
            public float DamageFrostPerLevel = 0f;
            public float DamageLightningPerLevel = 0f;
            public float DamagePoisonPerLevel = 0f;
            public float DamageSpiritPerLevel = 0f;

            public void WriteConfigDataToItem(ref ItemDrop item)
            {
                var id = item.m_itemData.m_shared;
                //id.m_name = Plugin.AxeBlackMetalBattleName.Value;
                //id.m_description = Plugin.AxeBlackMetalBattleDescription.Value;
                id.m_maxQuality = MaxQuality;
                id.m_blockPower = BlockAmount;
                id.m_blockPowerPerLevel = BlockAmountPerLevel;
                id.m_deflectionForce = DeflectionForce;
                id.m_deflectionForcePerLevel = DeflectionForcePerLevel;
                id.m_timedBlockBonus = ParryBonus;
                id.m_attackForce = Knockback_Power;
                id.m_backstabBonus = Backstab_Bonus;
                id.m_damages = SetDamageValues();
                id.m_damagesPerLevel = SetDamagePerLevelValues();
            }

            public void ReadConfigDataFromItem(GameObject go)
            {
                var id = go.GetComponent<ItemDrop>().m_itemData.m_shared;
                ItemPrefab = go.name;
                MaxQuality = id.m_maxQuality;
                BlockAmount = id.m_blockPower;
                BlockAmountPerLevel = id.m_blockPowerPerLevel;
                DeflectionForce = id.m_deflectionForce;
                DeflectionForcePerLevel = id.m_deflectionForcePerLevel;
                ParryBonus = id.m_timedBlockBonus;
                Knockback_Power = id.m_attackForce;
                Backstab_Bonus = id.m_backstabBonus;
                GetDamageValues(id.m_damages);
                GetDamagePerLevelValues(id.m_damagesPerLevel);
            }

            private HitData.DamageTypes SetDamageValues()
            {
                HitData.DamageTypes newDamage = new HitData.DamageTypes();
                newDamage.m_blunt = DamageBlunt;
                newDamage.m_slash = DamageSlash;
                newDamage.m_pierce = DamagePierce;
                newDamage.m_chop = DamageChop;
                newDamage.m_pickaxe = DamagePickaxe;
                newDamage.m_fire = DamageFire;
                newDamage.m_frost = DamageFrost;
                newDamage.m_lightning = DamageLightning;
                newDamage.m_poison = DamagePoison;
                newDamage.m_spirit = DamageSpirit;

                return newDamage;
            }

            private HitData.DamageTypes SetDamagePerLevelValues()
            {
                HitData.DamageTypes newDamage = new HitData.DamageTypes();
                newDamage.m_blunt = DamageBluntPerLevel;
                newDamage.m_slash = DamageSlashPerLevel;
                newDamage.m_pierce = DamagePiercePerLevel;
                newDamage.m_chop = DamageChopPerLevel;
                newDamage.m_pickaxe = DamagePickaxePerLevel;
                newDamage.m_fire = DamageFirePerLevel;
                newDamage.m_frost = DamageFrostPerLevel;
                newDamage.m_lightning = DamageLightningPerLevel;
                newDamage.m_poison = DamagePoisonPerLevel;
                newDamage.m_spirit = DamageSpiritPerLevel;

                return newDamage;
            }

            private void GetDamageValues(HitData.DamageTypes damageList)
            {
                DamageBlunt = damageList.m_blunt;
                DamageSlash = damageList.m_slash;
                DamagePierce = damageList.m_pierce;
                DamageChop = damageList.m_chop;
                DamagePickaxe = damageList.m_pickaxe;
                DamageFire = damageList.m_fire;
                DamageFrost = damageList.m_frost;
                DamageLightning = damageList.m_lightning;
                DamagePoison = damageList.m_poison;
                DamageSpirit = damageList.m_spirit;
            }

            private void GetDamagePerLevelValues(HitData.DamageTypes damageList)
            {
                DamageBluntPerLevel = damageList.m_blunt;
                DamageSlashPerLevel = damageList.m_slash;
                DamagePiercePerLevel = damageList.m_pierce;
                DamageChopPerLevel = damageList.m_chop;
                DamagePickaxePerLevel = damageList.m_pickaxe;
                DamageFirePerLevel = damageList.m_fire;
                DamageFrostPerLevel = damageList.m_frost;
                DamageLightningPerLevel = damageList.m_lightning;
                DamagePoisonPerLevel = damageList.m_poison;
                DamageSpiritPerLevel = damageList.m_spirit;
            }
        }

        [Serializable]
        public class ConfigResource
        {
            public string ItemPrefab;
            public int CraftingCost = 0;
            public int UpgradePerLevelCost = 0;
        }

        [Serializable]
        public class ConfigResourceWrapper
        {
            public ConfigResource[] resources;
        }

        [Serializable]
        public class ConfigRecipeData
        {
            public string ItemPrefab;
            public bool Enabled = true;
            public string CraftingStation = "forge";
            public int MinimumStationLevel = 1;
            public ConfigResource[] CraftingRequirementsArray;
            public string CraftingRequirements;

            public RecipeHelper LoadConfigedRecipeHelper(GameObject itemToCraft)
            {
                RecipeHelper outputRH = new RecipeHelper(itemToCraft, CraftingStation, MinimumStationLevel, 1);
                outputRH.recipeEnabled = Enabled;
                foreach (ConfigResource cr in CraftingRequirementsArray)
                {
                    outputRH.AddResource(cr.ItemPrefab, cr.CraftingCost, cr.UpgradePerLevelCost);
                }
                return outputRH;
            }

            public void ReadConfigFromRecipeHelper(RecipeHelper rh)
            {
                ItemPrefab = rh.GetPrefabName();
                CraftingStation = rh.GetCraftingStation();
                //Recipe rec = rh.GetRecipeInstance();
                MinimumStationLevel = rh.GetRecipeInstance().m_minStationLevel;
                ResourceElement[] resources = rh.GetResourceElements();
                List<ConfigResource> lcr = new List<ConfigResource>();
                foreach (var re in resources)
                {
                    ConfigResource tempCR = new ConfigResource();
                    tempCR.ItemPrefab = re.prefabItemName;
                    tempCR.CraftingCost = re.amount;
                    tempCR.UpgradePerLevelCost = re.amountPerLevel;
                    lcr.Add(tempCR);
                }
                CraftingRequirementsArray = lcr.ToArray();
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text;
using System;
using System.IO;

using Common.Data;

using Newtonsoft.Json;

namespace Assets.Scripts.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        public string DataPath;
        public Dictionary<int, MapDefine> Maps = null;
        public Dictionary<int, CharacterDefine> Characters = null;
        public Dictionary<int, TeleporterDefine> Teleporters = null;
        public Dictionary<int, NpcDefine> Npcs = null;
        public Dictionary<int, ItemDefine> Items = null;
        internal Dictionary<int, ShopDefine> Shops = null;
        public Dictionary<int, Dictionary<int, ShopItemDefine>> ShopItems = null;
        public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPoints = null;


        public DataManager()
        {
            this.DataPath = "Data/";
            Debug.LogFormat("DataManager > DataManager()");
        }

        public void Load()
        {
            string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
            this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);
            //Debug.LogFormat("Load Map count: {0}", Maps.Count);

            json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
            this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);
            //Debug.LogFormat("Load Character count: {0}", Maps.Count);

            json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
            this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);
            //Debug.LogFormat("Load Teleporter count: {0}", Teleporters.Count);

            json = File.ReadAllText(this.DataPath + "NpcDefine.txt");
            this.Npcs = JsonConvert.DeserializeObject<Dictionary<int, NpcDefine>>(json);
            //Debug.LogFormat("Load NPC count: {0}", Npcs.Count);

            json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
            this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

            json = File.ReadAllText(this.DataPath + "ShopDefine.txt");
            this.Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

            json = File.ReadAllText(this.DataPath + "ShopItemDefine.txt");
            this.ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);

            //json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
            //this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>> (json);
        }


        public IEnumerator LoadData()
        {
            string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
            this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);
            Debug.LogFormat("MapLOAD");

            yield return null;

            json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
            this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);
            Debug.LogFormat("CharacterLOAD");

            yield return null;

            json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
            this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);
            Debug.LogFormat("TeleporterLOAD");

            yield return null;

            json = File.ReadAllText(this.DataPath + "NpcDefine.txt");
            this.Npcs = JsonConvert.DeserializeObject<Dictionary<int, NpcDefine>>(json);
            Debug.LogFormat("NPCLOAD");

            yield return null;

            json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
            this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);
            Debug.LogFormat("ITEMLOAD");

            yield return null;

            json = File.ReadAllText(this.DataPath + "ShopDefine.txt");
            this.Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.DataPath + "ShopItemDefine.txt");
            this.ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);
            yield return null;

            json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
            this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);
            Debug.LogFormat("SPAWNLOAD");

            yield return null;
        }

#if UNITY_EDITOR
        public void SaveTeleporters()
        {
            string json = JsonConvert.SerializeObject(this.Teleporters, Formatting.Indented);
            File.WriteAllText(this.DataPath + "TeleporterDefine.txt", json);
        }

        public void SaveSpawnPoints()
        {
            string json = JsonConvert.SerializeObject(this.SpawnPoints, Formatting.Indented);
            File.WriteAllText(this.DataPath + "SpawnPointDefine.txt", json);
        }

#endif
    }
}



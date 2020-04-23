﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager singleton;

    public Vector3 initPos;

    public bool active = true;

    private string dataPath;

    private SaveData saveData;

    private void Awake()
    {
        if (singleton == null)
        {
            DontDestroyOnLoad(gameObject);
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        dataPath = Application.persistentDataPath + "/dextra_sav_";
        if (GameController.singleton.demoBuild)
        {
            dataPath += "DEMO_";
        }
        dataPath += "v" + Application.version.Replace(".", "_") + ".dat";
    }

    public void LoadGame()
    {
        LoadGame(true);
    }

    public void LoadGame(bool initGC)
    {
        if (!active)
        {
            return;
        }

        if (File.Exists(dataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            saveData = (SaveData)bf.Deserialize(file);
            file.Close();
            if (initGC)
            {
                InitGameController();
            }
        }
        else
        {
            CreateSave();
        }
    }

    public void CreateSave()
    {
        if (!active)
        {
            return;
        }

        saveData = new SaveData();
        saveData.SetCheckpointPos(initPos);
        SaveGame(false);
    }

    public void SaveGame(bool update)
    {
        if (!active)
        {
            return;
        }

        if (update)
        {
            UpdateSceneData();
            UpdatePlayerData();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(dataPath);
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void DeleteSave()
    {
        if (!active)
        {
            return;
        }

        if (File.Exists(dataPath))
        {
            File.Delete(dataPath);
        }
    }

    public void UpdateSceneData()
    {
        if (!active)
        {
            return;
        }

        if (saveData == null)
        {
            CreateSave();
        }

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneData temp;

        NPC[] npcs = FindObjectsOfType<NPC>();
        CutsceneTrigger[] triggers = FindObjectsOfType<CutsceneTrigger>();

        if (saveData.SceneExists(buildIndex))
        {
            temp = saveData.GetSceneData(buildIndex);
        }
        else
        {
            temp = saveData.CreateSceneData(buildIndex, npcs.Length, triggers.Length, NPC.shopkeeper.boostsPurchased.Length);
        }

        foreach (NPC npc in npcs)
        {
            int ind;
            if (int.TryParse(npc.gameObject.name.Substring(7, 1), out ind))
            {
                temp.npcInteracted[ind] = npc.interacted;
            }
        }

        foreach (CutsceneTrigger trigger in triggers)
        {
            int ind = int.Parse(trigger.gameObject.name.Substring(7, 1));
            temp.cutsceneTriggerable[ind] = trigger.triggerable;
        }

        temp.boostsPurchased = NPC.shopkeeper.boostsPurchased;

        saveData.SetSceneData(buildIndex, temp);
        saveData.SetCurrentScene(buildIndex);
    }

    public void UpdateCheckpointPos(Vector3 pos)
    {
        if (!active)
        {
            return;
        }

        if (saveData == null)
        {
            CreateSave();
        }

        saveData.SetCheckpointPos(pos);
    }

    public void UpdatePlayerData()
    {
        if (!active)
        {
            return;
        }

        if (saveData == null)
        {
            CreateSave();
        }

        saveData.maxHP = GameController.singleton.maxHP;
        saveData.maxMP = GameController.singleton.maxMP;
        saveData.strength = GameController.singleton.GetStrength();
        saveData.magic = GameController.singleton.GetMagic();
        saveData.gold = GameController.singleton.GetGold();

        saveData.numUnlocked = GameController.singleton.GetNumUnlocked();
        saveData.modes = new MiniMode[GameController.singleton.modes.Length];
        for (int i = 0; i < saveData.modes.Length; i++)
        {
            saveData.modes[i].name = GameController.singleton.modes[i].name;
            saveData.modes[i].unlocked = GameController.singleton.modes[i].unlocked;
        }

        saveData.spellsUnlocked = new bool[GameController.singleton.spellList.Length];
        for (int i = 0; i < saveData.spellsUnlocked.Length; i++)
        {
            saveData.spellsUnlocked[i] = GameController.singleton.spellList[i].unlocked;
        }

        saveData.skillsUnlocked = new bool[GameController.singleton.skillList.Length];
        for (int i = 0; i < saveData.skillsUnlocked.Length; i++)
        {
            saveData.skillsUnlocked[i] = GameController.singleton.skillList[i].unlocked;
        }

        saveData.artUnlocked = new bool[GameController.singleton.artList.Length];
        for (int i = 0; i < saveData.artUnlocked.Length; i++)
        {
            saveData.artUnlocked[i] = GameController.singleton.artList[i].unlocked;
        }

        saveData.songsUnlocked = new bool[GameController.singleton.songList.Length];
        for (int i = 0; i < saveData.songsUnlocked.Length; i++)
        {
            saveData.songsUnlocked[i] = GameController.singleton.songList[i].unlocked;
        }
    }

    public void InitScene()
    {
        if (!active)
        {
            return;
        }

        if (saveData == null)
        {
            CreateSave();
        }

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneData temp;

        NPC[] npcs = FindObjectsOfType<NPC>();
        for (int i = 0; i < npcs.Length && NPC.shopkeeper == null; i++)
        {
            if (npcs[i].npcType == NPC.NPCType.shop)
            {
                NPC.shopkeeper = npcs[i];
            }
        }
        CutsceneTrigger[] triggers = FindObjectsOfType<CutsceneTrigger>();

        if (saveData.SceneExists(buildIndex))
        {
            temp = saveData.GetSceneData(buildIndex);
        }
        else
        {
            temp = saveData.CreateSceneData(buildIndex, npcs.Length, triggers.Length, NPC.shopkeeper.boostsPurchased.Length);
        }

        for (int i = 0; i < npcs.Length; i++)
        {
            int ind;
            if (int.TryParse(npcs[i].gameObject.name.Substring(7, 1), out ind))
            {
                npcs[i].interacted = temp.npcInteracted[ind];
            }
        }

        for (int i = 0; i < triggers.Length; i++)
        {
            int ind = int.Parse(triggers[i].gameObject.name.Substring(7, 1));
            triggers[i].triggerable = temp.cutsceneTriggerable[ind];
        }

        NPC.shopkeeper.boostsPurchased = temp.boostsPurchased;

        if (buildIndex == saveData.GetCurrentScene())
        {
            GameObject.Find("Player").transform.position = saveData.GetCheckpointPos();
        }
        else
        {
            GameObject.Find("Player").transform.position = GameObject.Find("Checkpoint (0)").transform.position;
        }

        Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
        foreach(Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint.transform.position == saveData.GetCheckpointPos())
            {
                if (Checkpoint.active != null)
                {
                    Checkpoint.active.Deactivate();
                }
                checkpoint.Activate();
            }
        }
    }

    public void InitGameController()
    {
        if (!active)
        {
            return;
        }

        if (saveData == null)
        {
            CreateSave();
        }

        GameController.singleton.maxHP = saveData.maxHP;
        GameController.singleton.maxMP = saveData.maxMP;
        GameController.singleton.SetStrength(saveData.strength);
        GameController.singleton.SetMagic(saveData.magic);
        GameController.singleton.SetGold(saveData.gold);

        GameController.singleton.SetNumUnlocked(saveData.numUnlocked);
        for (int i = 0; i < GameController.singleton.modes.Length; i++)
        {
            GameController.singleton.modes[i].name = saveData.modes[i].name;
            GameController.singleton.modes[i].unlocked = saveData.modes[i].unlocked;
        }
        
        for (int i = 0; i < GameController.singleton.spellList.Length; i++)
        {
            GameController.singleton.spellList[i].unlocked = saveData.spellsUnlocked[i];
        }

        for (int i = 0; i < GameController.singleton.skillList.Length; i++)
        {
            GameController.singleton.skillList[i].unlocked = saveData.skillsUnlocked[i];
        }

        for (int i = 0; i < GameController.singleton.artList.Length; i++)
        {
            GameController.singleton.artList[i].unlocked = saveData.artUnlocked[i];
        }

        for (int i = 0; i < GameController.singleton.songList.Length; i++)
        {
            GameController.singleton.songList[i].unlocked = saveData.songsUnlocked[i];
        }
    }

    public int GetCurrentScene()
    {
        if (!active)
        {
            return -1;
        }

        if (saveData == null)
        {
            CreateSave();
        }

        return saveData.GetCurrentScene();
    }

    public void SetCurrentScene(int currentSceneIndex)
    {
        if (!active)
        {
            return;
        }

        if (saveData == null)
        {
            CreateSave();
        }

        saveData.SetCurrentScene(currentSceneIndex);
    }
}

[System.Serializable]
public struct MiniMode
{
    public string name;
    public bool unlocked;
}

[System.Serializable]
public class SaveData
{
    public int maxHP;
    public int maxMP;
    public int strength;
    public int magic;
    public int gold;

    public int numUnlocked;
    public MiniMode[] modes;

    public bool[] spellsUnlocked;
    public bool[] skillsUnlocked;
    public bool[] artUnlocked;
    public bool[] songsUnlocked;

    int currentScene;
    float checkpointX, checkpointY, checkpointZ;
    Dictionary<int, SceneData> scenes;

    public SaveData()
    {
        currentScene = 1;
        scenes = new Dictionary<int, SceneData>();

        if (GameController.singleton != null)
        {
            maxHP = GameController.singleton.maxHP;
            maxMP = GameController.singleton.maxMP;
            strength = GameController.singleton.GetStrength();
            magic = GameController.singleton.GetMagic();
            gold = GameController.singleton.GetGold();

            numUnlocked = 1;
            modes = new MiniMode[GameController.singleton.modes.Length];
            for (int i = 0; i < modes.Length; i++)
            {
                modes[i].name = GameController.singleton.modes[i].name;
                modes[i].unlocked = GameController.singleton.modes[i].unlocked;
            }

            spellsUnlocked = new bool[GameController.singleton.spellList.Length];
            for (int i = 0; i < spellsUnlocked.Length; i++)
            {
                spellsUnlocked[i] = GameController.singleton.spellList[i].unlocked;
            }

            skillsUnlocked = new bool[GameController.singleton.skillList.Length];
            for (int i = 0; i < skillsUnlocked.Length; i++)
            {
                skillsUnlocked[i] = GameController.singleton.skillList[i].unlocked;
            }

            artUnlocked = new bool[GameController.singleton.artList.Length];
            for (int i = 0; i < artUnlocked.Length; i++)
            {
                artUnlocked[i] = false;
            }

            songsUnlocked = new bool[GameController.singleton.songList.Length];
            for (int i = 0; i < songsUnlocked.Length; i++)
            {
                songsUnlocked[i] = false;
            }
        }
    }

    public bool SceneExists(int index)
    {
        return scenes.ContainsKey(index);
    }

    public SceneData CreateSceneData(int index, int numEnemies, int numScenes, int numShopBoosts)
    {
        scenes.Add(index, new SceneData(index, numEnemies, numScenes, numShopBoosts));
        return scenes[index];
    }

    public SceneData GetSceneData (int index)
    {
        return scenes[index];
    }

    public void SetSceneData (int index, SceneData sceneData)
    {
        scenes[index] = sceneData;
    }

    public Vector3 GetCheckpointPos()
    {
        return new Vector3(checkpointX, checkpointY, checkpointZ);
    }

    public void SetCheckpointPos (Vector3 checkpointPos)
    {
        checkpointX = checkpointPos.x;
        checkpointY = checkpointPos.y;
        checkpointZ = checkpointPos.z;
    }

    public int GetCurrentScene()
    {
        return currentScene;
    }

    public void SetCurrentScene(int currentSceneIndex)
    {
        currentScene = currentSceneIndex;
    }
}

[System.Serializable]
public class SceneData
{
    int buildIndex;
    public bool[] npcInteracted;
    public bool[] cutsceneTriggerable;
    public bool[] boostsPurchased;

    public SceneData(int index, int numEnemies, int numCutscenes, int numBoostsInShop)
    {
        buildIndex = index;
        npcInteracted = new bool[numEnemies];
        for (int i = 0; i < numEnemies; i++)
        {
            npcInteracted[i] = false;
        }

        cutsceneTriggerable = new bool[numCutscenes];
        for (int i = 0; i < numCutscenes; i++)
        {
            cutsceneTriggerable[i] = true;
        }

        boostsPurchased = new bool[numBoostsInShop];
        for (int i = 0; i < numBoostsInShop; i++)
        {
            boostsPurchased[i] = false;
        }
    }
}
using UnityEngine;
using System.IO;

// A static class to handle all low-level file saving and loading operations using JSON.
public static class SaveSystem
{
    public static void DeleteAllSaveData()
    {
        string[] paths = {
            GetStatsSavePath(),
            GetSkillsSavePath(),
            GetQuestsSavePath(),
            GetInventorySavePath(),
            GetTitlesSavePath(),
            GetSkillbarSavePath() // Added skillbar path
        };

        foreach (string path in paths)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"Deleted save file at: {path}");
            }
        }
    }

    // --- Player Stats Save/Load ---
    private static string GetStatsSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerStats.json");
    }

    public static void SavePlayerStats(PlayerStats playerStats)
    {
        string path = GetStatsSavePath();
        PlayerStats.SaveData data = new PlayerStats.SaveData(playerStats);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static PlayerStats.SaveData LoadPlayerStats()
    {
        string path = GetStatsSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonUtility.FromJson<PlayerStats.SaveData>(json);
        }
        return null;
    }

    // --- Player Skills Save/Load ---
    private static string GetSkillsSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerSkills.json");
    }

    public static void SavePlayerSkills(PlayerSkillManager skillManager)
    {
        string path = GetSkillsSavePath();
        PlayerSkillManager.SaveData data = new PlayerSkillManager.SaveData(skillManager);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static PlayerSkillManager.SaveData LoadPlayerSkills()
    {
        string path = GetSkillsSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonUtility.FromJson<PlayerSkillManager.SaveData>(json);
        }
        return null;
    }

    // --- Player Quests Save/Load ---
    private static string GetQuestsSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerQuests.json");
    }

    public static void SavePlayerQuests(QuestLog questLog)
    {
        string path = GetQuestsSavePath();
        QuestLog.SaveData data = new QuestLog.SaveData(questLog);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static QuestLog.SaveData LoadPlayerQuests()
    {
        string path = GetQuestsSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonUtility.FromJson<QuestLog.SaveData>(json);
        }
        return null;
    }

    // --- Player Inventory Save/Load ---
    private static string GetInventorySavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerInventory.json");
    }

    public static void SavePlayerInventory(InventoryManager inventoryManager)
    {
        string path = GetInventorySavePath();
        InventoryManager.SaveData data = new InventoryManager.SaveData(inventoryManager);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static InventoryManager.SaveData LoadPlayerInventory()
    {
        string path = GetInventorySavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonUtility.FromJson<InventoryManager.SaveData>(json);
        }
        return null;
    }

    // --- Player Titles Save/Load ---
    private static string GetTitlesSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerTitles.json");
    }

    public static void SavePlayerTitles(TitleManager titleManager)
    {
        string path = GetTitlesSavePath();
        TitleManager.SaveData data = new TitleManager.SaveData(titleManager);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static TitleManager.SaveData LoadPlayerTitles()
    {
        string path = GetTitlesSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonUtility.FromJson<TitleManager.SaveData>(json);
        }
        return null;
    }

    // --- NEW: Player Skillbar Save/Load ---
    private static string GetSkillbarSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerSkillbar.json");
    }

    public static void SavePlayerSkillbar(SkillbarManager skillbarManager)
    {
        string path = GetSkillbarSavePath();
        SkillbarManager.SaveData data = new SkillbarManager.SaveData(skillbarManager);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static SkillbarManager.SaveData LoadPlayerSkillbar()
    {
        string path = GetSkillbarSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonUtility.FromJson<SkillbarManager.SaveData>(json);
        }
        return null;
    }
}
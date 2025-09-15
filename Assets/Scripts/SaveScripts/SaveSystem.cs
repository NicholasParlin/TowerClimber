using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// A static class to handle all low-level file saving and loading operations.
public static class SaveSystem
{
    // --- Player Stats Save/Load ---
    private static string GetStatsSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerStats.sav");
    }

    public static void SavePlayerStats(PlayerStats playerStats)
    {
        string path = GetStatsSavePath();
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            PlayerStats.SaveData data = new PlayerStats.SaveData(playerStats);
            formatter.Serialize(stream, data);
        }
    }

    public static PlayerStats.SaveData LoadPlayerStats()
    {
        string path = GetStatsSavePath();
        if (File.Exists(path))
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                if (stream.Length == 0) return null;
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as PlayerStats.SaveData;
            }
        }
        return null;
    }

    // --- Player Skills Save/Load ---
    private static string GetSkillsSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerSkills.sav");
    }

    public static void SavePlayerSkills(PlayerSkillManager skillManager)
    {
        string path = GetSkillsSavePath();
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            PlayerSkillManager.SaveData data = new PlayerSkillManager.SaveData(skillManager);
            formatter.Serialize(stream, data);
        }
    }

    public static PlayerSkillManager.SaveData LoadPlayerSkills()
    {
        string path = GetSkillsSavePath();
        if (File.Exists(path))
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                if (stream.Length == 0) return null;
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as PlayerSkillManager.SaveData;
            }
        }
        return null;
    }

    // --- Player Quests Save/Load ---
    private static string GetQuestsSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerQuests.sav");
    }

    public static void SavePlayerQuests(QuestLog questLog)
    {
        string path = GetQuestsSavePath();
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            QuestLog.SaveData data = new QuestLog.SaveData(questLog);
            formatter.Serialize(stream, data);
        }
    }

    public static QuestLog.SaveData LoadPlayerQuests()
    {
        string path = GetQuestsSavePath();
        if (File.Exists(path))
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                if (stream.Length == 0) return null;
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as QuestLog.SaveData;
            }
        }
        return null;
    }

    // --- Player Inventory Save/Load ---
    private static string GetInventorySavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerInventory.sav");
    }

    public static void SavePlayerInventory(InventoryManager inventoryManager)
    {
        string path = GetInventorySavePath();
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            InventoryManager.SaveData data = new InventoryManager.SaveData(inventoryManager);
            formatter.Serialize(stream, data);
        }
    }

    public static InventoryManager.SaveData LoadPlayerInventory()
    {
        string path = GetInventorySavePath();
        if (File.Exists(path))
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                if (stream.Length == 0) return null;
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as InventoryManager.SaveData;
            }
        }
        return null;
    }

    // --- Player Titles Save/Load ---

    private static string GetTitlesSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerTitles.sav");
    }

    public static void SavePlayerTitles(TitleManager titleManager)
    {
        string path = GetTitlesSavePath();
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            TitleManager.SaveData data = new TitleManager.SaveData(titleManager);
            formatter.Serialize(stream, data);
        }
    }

    public static TitleManager.SaveData LoadPlayerTitles()
    {
        string path = GetTitlesSavePath();
        if (File.Exists(path))
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                if (stream.Length == 0) return null;
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as TitleManager.SaveData;
            }
        }
        return null;
    }
}
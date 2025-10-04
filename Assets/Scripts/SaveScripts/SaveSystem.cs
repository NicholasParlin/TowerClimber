using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// A static class to handle all low-level, secure file saving and loading operations.
public static class SaveSystem
{
    // A secret key to add to our data before hashing. This makes it harder for someone
    // to generate their own valid hash for a modified save file.
    private const string HASH_KEY = "TowerClimberSecretKey!@#$";

    public static void DeleteAllSaveData()
    {
        // ... (this method remains the same)
    }

    // --- Generic Save/Load Logic ---

    private static void SaveData<T>(T data, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // 1. Serialize the data object into a memory stream
                formatter.Serialize(memoryStream, data);
                byte[] dataBytes = memoryStream.ToArray();

                // 2. Compute the hash from the data bytes and our secret key
                string hash = ComputeHash(dataBytes);

                // 3. Write the hash and then the data to the file
                byte[] hashBytes = Encoding.UTF8.GetBytes(hash);
                stream.Write(hashBytes, 0, hashBytes.Length);
                stream.Write(dataBytes, 0, dataBytes.Length);
            }
        }
    }

    private static T LoadData<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file not found at: {path}");
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            if (stream.Length == 0)
            {
                Debug.LogError($"Save file is empty: {path}");
                return null;
            }

            // 1. Read the hash (SHA256 hash is 64 characters long)
            byte[] hashBytes = new byte[64];
            if (stream.Read(hashBytes, 0, 64) != 64)
            {
                Debug.LogError($"Could not read hash from save file: {path}");
                return null;
            }
            string savedHash = Encoding.UTF8.GetString(hashBytes);

            // 2. Read the rest of the file, which is the actual data
            using (MemoryStream dataStream = new MemoryStream())
            {
                stream.CopyTo(dataStream);
                byte[] dataBytes = dataStream.ToArray();

                // 3. Compute a new hash from the data we just read
                string computedHash = ComputeHash(dataBytes);

                // 4. Compare! This is the integrity check.
                if (savedHash != computedHash)
                {
                    Debug.LogError($"SAVE FILE CORRUPTED OR TAMPERED: Hash mismatch in {path}");
                    // Here you could fire an event to show a "Corrupted Save" message to the player.
                    return null;
                }

                // Hashes match, so the data is valid. Deserialize and return it.
                using (MemoryStream deserializeStream = new MemoryStream(dataBytes))
                {
                    return formatter.Deserialize(deserializeStream) as T;
                }
            }
        }
    }

    private static string ComputeHash(byte[] data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(HASH_KEY);
            byte[] combinedBytes = new byte[keyBytes.Length + data.Length];

            // Prepend the secret key to the data before hashing
            Buffer.BlockCopy(keyBytes, 0, combinedBytes, 0, keyBytes.Length);
            Buffer.BlockCopy(data, 0, combinedBytes, keyBytes.Length, data.Length);

            byte[] hashBytes = sha256.ComputeHash(combinedBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    // --- Specific Save/Load Methods ---

    private static string GetStatsSavePath() => Path.Combine(Application.persistentDataPath, "playerStats.bin");
    public static void SavePlayerStats(PlayerStats playerStats) => SaveData(new PlayerStats.SaveData(playerStats), GetStatsSavePath());
    public static PlayerStats.SaveData LoadPlayerStats() => LoadData<PlayerStats.SaveData>(GetStatsSavePath());

    private static string GetSkillsSavePath() => Path.Combine(Application.persistentDataPath, "playerSkills.bin");
    public static void SavePlayerSkills(PlayerSkillManager skillManager) => SaveData(new PlayerSkillManager.SaveData(skillManager), GetSkillsSavePath());
    public static PlayerSkillManager.SaveData LoadPlayerSkills() => LoadData<PlayerSkillManager.SaveData>(GetSkillsSavePath());

    private static string GetQuestsSavePath() => Path.Combine(Application.persistentDataPath, "playerQuests.bin");
    public static void SavePlayerQuests(QuestLog questLog) => SaveData(new QuestLog.SaveData(questLog), GetQuestsSavePath());
    public static QuestLog.SaveData LoadPlayerQuests() => LoadData<QuestLog.SaveData>(GetQuestsSavePath());

    private static string GetInventorySavePath() => Path.Combine(Application.persistentDataPath, "playerInventory.bin");
    public static void SavePlayerInventory(InventoryManager inventoryManager) => SaveData(new InventoryManager.SaveData(inventoryManager), GetInventorySavePath());
    public static InventoryManager.SaveData LoadPlayerInventory() => LoadData<InventoryManager.SaveData>(GetInventorySavePath());

    private static string GetTitlesSavePath() => Path.Combine(Application.persistentDataPath, "playerTitles.bin");
    public static void SavePlayerTitles(TitleManager titleManager) => SaveData(new TitleManager.SaveData(titleManager), GetTitlesSavePath());
    public static TitleManager.SaveData LoadPlayerTitles() => LoadData<TitleManager.SaveData>(GetTitlesSavePath());

    private static string GetSkillbarSavePath() => Path.Combine(Application.persistentDataPath, "playerSkillbar.bin");
    public static void SavePlayerSkillbar(SkillbarManager skillbarManager) => SaveData(new SkillbarManager.SaveData(skillbarManager), GetSkillbarSavePath());
    public static SkillbarManager.SaveData LoadPlayerSkillbar() => LoadData<SkillbarManager.SaveData>(GetSkillbarSavePath());
}
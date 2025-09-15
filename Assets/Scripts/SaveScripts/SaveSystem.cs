using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// A static class to handle all low-level file saving and loading operations.
// By making the class 'static', all its methods must also be 'static'.
public static class SaveSystem
{
    // --- Player Stats Save/Load ---

    private static string GetStatsSavePath()
    {
        // Application.persistentDataPath is a reliable, OS-independent folder for save games.
        return Path.Combine(Application.persistentDataPath, "playerStats.sav");
    }

    public static void SavePlayerStats(PlayerStats playerStats)
    {
        string path = GetStatsSavePath();
        // Using a FileStream to create and write to the file.
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            // BinaryFormatter is a robust way to serialize C# classes to a file.
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
                // Ensure the file is not empty to prevent errors.
                if (stream.Length == 0)
                {
                    Debug.LogWarning("Stats save file is empty.");
                    return null;
                }
                BinaryFormatter formatter = new BinaryFormatter();
                // Deserialize the file back into our SaveData class.
                return formatter.Deserialize(stream) as PlayerStats.SaveData;
            }
        }
        else
        {
            // If no save file exists, return null.
            return null;
        }
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
                if (stream.Length == 0)
                {
                    Debug.LogWarning("Skills save file is empty.");
                    return null;
                }
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as PlayerSkillManager.SaveData;
            }
        }
        else
        {
            return null;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

// This ScriptableObject will act as a centralized database for all Title assets.
[CreateAssetMenu(fileName = "TitleDatabase", menuName = "Titles/Title Database")]
public class TitleDatabase : ScriptableObject
{
    public List<Title> allTitles = new List<Title>();
    private Dictionary<string, Title> _titleDictionary;

    // OnEnable is called when the script is loaded or a value is changed in the Inspector.
    private void OnEnable()
    {
        // Initialize the dictionary for fast lookups.
        _titleDictionary = new Dictionary<string, Title>();
        foreach (var title in allTitles)
        {
            if (title != null && !_titleDictionary.ContainsKey(title.name))
            {
                // We use title.name because it's the unique asset filename.
                _titleDictionary.Add(title.name, title);
            }
        }
    }

    /// <summary>
    /// Retrieves a Title asset from the database by its asset name.
    /// </summary>
    /// <param name="titleName">The unique name of the Title asset file.</param>
    /// <returns>The Title asset, or null if not found.</returns>
    public Title GetTitleByName(string titleName)
    {
        if (string.IsNullOrEmpty(titleName) || _titleDictionary == null)
        {
            return null;
        }

        if (_titleDictionary.TryGetValue(titleName, out Title title))
        {
            return title;
        }

        // Add a warning for easier debugging if a title is missing.
        Debug.LogWarning($"Title with name '{titleName}' not found in the database.");
        return null;
    }
}
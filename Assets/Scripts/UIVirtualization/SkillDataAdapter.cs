using UnityEngine;

// This adapter knows how to configure a SkillListingUI prefab with Skill data.
public class SkillDataAdapter : MonoBehaviour, IDataAdapter
{
    // A reference to the player's skill manager is needed to check if a passive is active.
    [SerializeField] private PlayerSkillManager playerSkillManager;

    public void Setup(GameObject uiElement, object data)
    {
        SkillListingUI skillUI = uiElement.GetComponent<SkillListingUI>();
        Skill skillData = data as Skill;

        if (skillUI != null && skillData != null)
        {
            if (playerSkillManager == null)
            {
                Debug.LogError("SkillDataAdapter is missing a reference to the PlayerSkillManager!");
                return;
            }

            bool isPassiveActive = playerSkillManager.IsPassiveActive(skillData);
            skillUI.DisplaySkill(skillData, isPassiveActive);
        }
    }
}
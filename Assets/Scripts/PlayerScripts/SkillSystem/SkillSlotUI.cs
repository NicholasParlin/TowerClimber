using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Image component that displays the skill's icon.")]
    [SerializeField] private Image skillIcon;
    [Tooltip("An image used for the 'flash' effect when a skill is used. Should be white.")]
    [SerializeField] private Image flashImage;
    [Tooltip("An image set to Fill Method: Radial 360, used for the cooldown timer.")]
    [SerializeField] private Image cooldownImage;
    [Tooltip("A Text component to display the remaining cooldown time.")]
    [SerializeField] private Text cooldownText; // NEW: Text for cooldown timer

    private void Awake()
    {
        if (skillIcon != null) skillIcon.enabled = false;
        if (flashImage != null) flashImage.color = new Color(1, 1, 1, 0);
        if (cooldownImage != null) cooldownImage.fillAmount = 0;
        if (cooldownText != null) cooldownText.enabled = false;
    }

    public void Display(Skill skill)
    {
        if (skill != null)
        {
            skillIcon.sprite = skill.icon;
            skillIcon.enabled = true;
        }
        else
        {
            skillIcon.sprite = null;
            skillIcon.enabled = false;
        }
    }

    public void TriggerFlash()
    {
        if (flashImage != null)
        {
            StopCoroutine("Flash");
            StartCoroutine(Flash());
        }
    }

    private IEnumerator Flash()
    {
        flashImage.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        flashImage.color = new Color(1, 1, 1, 0);
    }

    public void StartCooldown(float duration)
    {
        if (cooldownImage != null)
        {
            StopCoroutine("Cooldown");
            StartCoroutine(Cooldown(duration));
        }
    }

    private IEnumerator Cooldown(float duration)
    {
        if (cooldownText != null) cooldownText.enabled = true;

        float timer = 0f;
        cooldownImage.fillAmount = 1f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float remainingTime = duration - timer;

            cooldownImage.fillAmount = remainingTime / duration;
            if (cooldownText != null)
            {
                cooldownText.text = remainingTime.ToString("F1"); // Format to one decimal place
            }

            yield return null;
        }

        cooldownImage.fillAmount = 0f;
        if (cooldownText != null) cooldownText.enabled = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Skill skill;
    public List<SkillHolder> connections = new List<SkillHolder>(); //skills this depends on
    private Image icon;
    private Text points;
    public int pointsAllocated = 0;
    public int maxPoints = 1;
    public bool filled = false;
    public bool disabled = true;
    public bool refunding = false;

    void Awake()
    {
        icon = GetComponent<Image>();
        if (disabled)
            icon.color = Color.grey;
        points = GetComponentInChildren<Text>();
        UpdateHolder();
    }

    public void UpdateHolder()
    {
        UpdatePointsText();
        CheckConnections();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //invest a skill point
        if (!refunding && !disabled)
        {
            if (!filled && PlayerStats.instance.skillPoints > 0)
            {
                pointsAllocated += 1;
                if (pointsAllocated == maxPoints)
                {
                    filled = true;
                    //change icon to gold or w/e to look full
                }
                PlayerStats.instance.skillPoints -= 1;
                //add the skill's effect to the player
                skill.UnlockSkill();
                UpdateHolder();
                
            }
        }
        //refund a skill, costing a refund point
        else if (refunding && !disabled && pointsAllocated > 0 && PlayerStats.instance.refundPoints > 0)
        {
            PlayerStats.instance.skillPoints += 1;
            pointsAllocated -= 1;
            PlayerStats.instance.refundPoints -= 1;
            filled = false;
            skill.RefundSkill();
            UpdateHolder();
        }
    }

    private void CheckConnections()
    {
        bool enabled = true;
        foreach (SkillHolder skill in connections)
        {
            enabled &= skill.filled;
        }
        if (enabled)
        {
            disabled = false;
        }
        else
        {
            disabled = true;
        }
    }

    private void UpdatePointsText()
    {
        if (pointsAllocated == 0 || maxPoints == 1)
        {
            points.enabled = false;
        }
        else if (!filled)
        {
            points.enabled = true;
            points.text = pointsAllocated + "/" + maxPoints;
        }
        else
        {
            points.enabled = true;
            points.text = maxPoints.ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillToolTip.instance.hovered = this;
        SkillToolTip.instance.UpdateText();
        SkillToolTip.instance.headerObj.SetActive(true);
        SkillToolTip.instance.contentObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SkillToolTip.instance.hovered = null;
        SkillToolTip.instance.headerObj.SetActive(false);
        SkillToolTip.instance.contentObj.SetActive(false);
    }
}


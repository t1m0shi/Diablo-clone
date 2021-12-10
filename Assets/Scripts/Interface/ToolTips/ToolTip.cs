using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip <T>: MonoBehaviour
{
    //public static AbilityTooltip instance;
    //public PlayerAbilityHolder hovered;
    public T hovered;
    public GameObject headerObj;
    public GameObject contentObj;
    protected RectTransform m_RectTransform;
    public TextMeshProUGUI header;
    public TextMeshProUGUI content;

    void Awake()
    {
        //instance = this;
        Init();
    }

    void Update()
    {
        if (contentObj.activeInHierarchy)
        {
            UpdatePosition();
        }
    }

    protected virtual void Init()
    {
        m_RectTransform = GetComponent<RectTransform>();
        header = headerObj.GetComponent<TextMeshProUGUI>();
        content = contentObj.GetComponent<TextMeshProUGUI>();
    }

    public void UpdatePosition()
    {
        Vector2 pos = Input.mousePosition;

        //float pivotX = pos.x / Screen.width;
        //float pivotY = pos.y / Screen.height;
        float pivotX = pos.x;
        float pivotY = pos.y;

        float maxX = Screen.width - m_RectTransform.rect.width - 40;
        float minX = 0;
        float maxY = Screen.height - m_RectTransform.rect.height;
        float minY = 0;
        if (pos.x > maxX)
        {
            pivotX = pivotX - m_RectTransform.rect.width - 40;
        }
        if (pos.x < minX)
        {
            pivotX += m_RectTransform.rect.width;
        }
        if (pos.y > maxY)
        {
            pivotY -= m_RectTransform.rect.height;
        }
        if (pos.y < minY)
        {
            pivotY += m_RectTransform.rect.height;
        }

        m_RectTransform.position = new Vector2(pivotX, pivotY);
    }

    public virtual void UpdateText()
    {
        //Header.text = hovered.ability.name;
        //Content.text = hovered.ability.effect.description;

    }
}

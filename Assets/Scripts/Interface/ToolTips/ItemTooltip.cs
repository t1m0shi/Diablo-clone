using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ItemTooltip : MonoBehaviour
{
    public TextMeshProUGUI Header;
    public TextMeshProUGUI Content;
    public TextMeshProUGUI Rarity;
    public TextMeshProUGUI Quality;

    RectTransform m_RectTransform;
    CanvasScaler m_CanvasScaler;
    Canvas m_Canvas;
    
    void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_CanvasScaler = GetComponentInParent<CanvasScaler>();
        m_Canvas = GetComponentInParent<Canvas>();
    }

    void OnEnable()
    {
        UpdatePosition();
    }
    
    void Update()
    {
        //Debug.Log(Input.mousePosition);
        if(gameObject.activeInHierarchy)
            UpdatePosition();
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

    public void UpdateText(Item itemUsed)
    {
        Color rarity_color = Item.ConvertColor(Item.RarityColor[itemUsed.rarity]);
        this.Header.text = itemUsed.name;// + "\n"+ itemUsed.rarity.ToString();
        this.Header.color = rarity_color;
        // tooltip.Content.text = itemUsed.GetDescription();
        string t = "";
        foreach (var item in itemUsed.modifiers)
        {
            t += "+ " + item.ToString() + "\n";
        }
        this.Content.text = t;
        this.Rarity.text = itemUsed.rarity.ToString();
        this.Rarity.color = rarity_color;
        //this.Quality.text = itemUsed.quality.ToString();
        //this.Quality.color = rarity_color;
    }
}

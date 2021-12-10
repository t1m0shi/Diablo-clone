using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class StatHolderUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerStats inst;
    public Text text;


    public void OnPointerEnter(PointerEventData eventData)
    {
        StatsTooltip.instance.hovered = this;
        StatsTooltip.instance.UpdateText();
        StatsTooltip.instance.contentObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StatsTooltip.instance.hovered = null;
        StatsTooltip.instance.contentObj.SetActive(false);
    }

    public virtual void UpdateHolder()
    {

        //Debug.Log("updating holder");
        text = gameObject.GetComponent<Text>();
        //inst = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        inst = PlayerStats.instance;
    }
}

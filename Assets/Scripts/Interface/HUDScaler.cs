using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScaler : MonoBehaviour
{
    [Range(0.5f, 2.0f)]
    public float scale = 1f;
    RectTransform t;
    void Start()
    {
        t = GetComponent<RectTransform>();
    }
    public void UpdateScale()
    {
        t.localScale = new Vector3(scale, scale, scale);
        t.position = new Vector3(t.position.x, t.position.y * scale, t.position.z);
    }
}

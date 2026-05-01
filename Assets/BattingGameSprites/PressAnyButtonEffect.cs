using UnityEngine;
using TMPro;

public class PressAnyButtonEffect : MonoBehaviour
{
    public float floatAmp = 5f;
    public float speed = 2f;

    RectTransform rect;
    TMP_Text text;
    Vector3 startPos;
    Color baseColor;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponent<TMP_Text>();

        startPos = rect.anchoredPosition;
        baseColor = text.color;
    }

    void Update()
    {
        // è„â∫Ç‰ÇÍ
        float y = Mathf.Sin(Time.time * speed) * floatAmp;
        rect.anchoredPosition = startPos + new Vector3(0, y, 0);

        // ì_ñ≈
        float alpha = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        text.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }
}
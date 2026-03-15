using UnityEngine;
using TMPro;

public class CompassController : MonoBehaviour
{
    public Transform player;
    public RectTransform compassStrip;
    public float compassWidth = 2000f;

    public TextMeshProUGUI compassText;

    void Update()
    {
        float playerY = player.eulerAngles.y;

        float offset = (playerY / 360f) * compassWidth;
        compassStrip.anchoredPosition = new Vector2(-offset, 0);

        compassText.text = GetDirection(playerY);
    }

    string GetDirection(float angle)
    {
        if (angle >= 337.5f || angle < 22.5f) return "N";
        if (angle < 67.5f) return "NE";
        if (angle < 112.5f) return "E";
        if (angle < 157.5f) return "SE";
        if (angle < 202.5f) return "S";
        if (angle < 247.5f) return "SW";
        if (angle < 292.5f) return "W";
        return "NW";
    }
}
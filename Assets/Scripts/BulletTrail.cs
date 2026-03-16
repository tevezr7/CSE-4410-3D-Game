using UnityEngine;
using System.Collections;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float trailDuration = 0.05f;

    public void SpawnTrail(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        gameObject.SetActive(true);
        StartCoroutine(FadeTrail());
    }

    private IEnumerator FadeTrail()
    {
        float elapsed = 0f;
        Color startColor = lineRenderer.startColor;

        while (elapsed < trailDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / trailDuration);
            lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            lineRenderer.endColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}

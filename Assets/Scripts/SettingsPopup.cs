using UnityEngine;

public class SettingsPopup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnSubmitName(string name)
    {
        Debug.Log(name);
    }

    public void OnSpeedValue(float speed)
    {
        Debug.Log($"Speed: {speed}");
        GameEvents.SpeedChanged(speed); //replaces Messenger<float>.Broadcast
    }
}

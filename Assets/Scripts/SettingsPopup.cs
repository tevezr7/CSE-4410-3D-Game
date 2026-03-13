using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsPopup : MonoBehaviour
{
    private void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(gameObject.activeSelf)
            {
                Close();
                Time.timeScale = 1f; //unpause game when closing settings
            }
            else
            {
                Open();
                Time.timeScale = 0f; //pause game when opening settings
            }
        }
    }
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

using UnityEngine;
using UnityEngine.InputSystem;

public class BuyBox : MonoBehaviour
{
    public GameObject shopUI;
    private bool playerInside = false;

    void Update()
    {
        if (playerInside && Keyboard.current.fKey.wasPressedThisFrame)
        {
            shopUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
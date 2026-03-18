using UnityEngine;
public class Billboard : MonoBehaviour
{
    private Camera cam;
    void Start() => cam = Camera.main;
    void LateUpdate() => transform.rotation = cam.transform.rotation;
}
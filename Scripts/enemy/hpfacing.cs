using UnityEngine;

public class hpfacing : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // 讓血條正面朝向攝影機
        transform.LookAt(transform.position + cam.forward);
    }
}
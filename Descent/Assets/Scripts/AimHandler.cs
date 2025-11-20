using UnityEngine;

public class AimHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MouseLook()
    {
        float mouseDeltaX = Input.GetAxis("Mouse X") * mouseSensitivity; //Where mouse x
        float mouseDeltaY = Input.GetAxis("Mouse Y") * mouseSensitivity;
    }
}

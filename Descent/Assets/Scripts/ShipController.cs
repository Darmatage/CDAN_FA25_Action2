using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float forwardSpeed = 25f;
    public float strafeSpeed = 15f;
    public float hoverSpeed = 10f;
    public float lookRateSpeed = 90f;
    public float rollSpeed = 90f;
    public float rollAcceleration = 3.5f;
    private Vector2 lookInput, screenCenter, mouseDistance;

    private float activeforwardSpeed,activestrafeSpeed,activehoverSpeed;

    public float forwardAcceleration = 2.5f;
    public float strafeAcceleration = 2f;

    public float hoverAcceleration = 2f;
    private float rollInput;


    void Start()
    {
        screenCenter.x = Screen.width *.5f;
        screenCenter.y = Screen.height *.5f;
    }

    // Update is called once per frame
    void Update()
    {
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance,1f);

        rollInput = Mathf.Lerp(rollInput,Input.GetAxisRaw("Roll"), rollAcceleration*Time.deltaTime);

        transform.Rotate(-mouseDistance.y*lookRateSpeed*Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput*rollSpeed*Time.deltaTime, Space.Self);

        activeforwardSpeed = Mathf.Lerp(activeforwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration*Time.deltaTime);
        activestrafeSpeed = Mathf.Lerp(activestrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, strafeAcceleration*Time.deltaTime);
        activehoverSpeed = Mathf.Lerp(activehoverSpeed, Input.GetAxisRaw("Up/Down") * hoverSpeed,hoverAcceleration*Time.deltaTime);

        transform.position += transform.forward * activeforwardSpeed * Time.deltaTime;
        transform.position += (transform.right * activestrafeSpeed*Time.deltaTime) + (transform.up*activehoverSpeed*Time.deltaTime);

    }
}

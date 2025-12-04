using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShoot : MonoBehaviour
{
    public GameObject cam;
    // Update is called once per frame
    
    
    
    
    void LateUpdate()
    {
        transform.rotation = cam.transform.rotation; //aim matches camera rotation
    }

}

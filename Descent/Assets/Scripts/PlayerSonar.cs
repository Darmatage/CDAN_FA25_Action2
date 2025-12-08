using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class PlayerSonar : MonoBehaviour
{
    public KeyCode sonarKey = KeyCode.T;
    public float sonarTime = 4f;
    public UniversalRendererData drawBehind;
    public float detectionDistanceMin = 10f;
    public Transform player;
    public Transform gate;
    public AudioSource SFX_Ping;

    void Start()
    {
        drawBehind.rendererFeatures[^2].SetActive(false);

        if (GameObject.FindWithTag("Door")!=null){
            gate = GameObject.FindWithTag("Door").GetComponent<Transform>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance (player.position,gate.position);
        if (Input.GetKeyDown(sonarKey))
        {
            if (distance < detectionDistanceMin)
            {
            StopAllCoroutines();
            StartCoroutine(Sonar());
            }
        }
        if (SFX_Ping.isPlaying == false){
                        SFX_Ping.Play();
                }
    }

    IEnumerator Sonar()
    {
        drawBehind.rendererFeatures[^2].SetActive(true);
        yield return new WaitForSeconds(sonarTime);
        drawBehind.rendererFeatures[^2].SetActive(false);

    }
}

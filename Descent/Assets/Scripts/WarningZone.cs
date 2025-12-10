using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class WarningZone : MonoBehaviour
{

    [Header("Visuals")]
    public GameObject preWarningCanvas;
    public TMP_Text preWarningText;
    public Image screenOverlay;
    public Color overlayColor = new Color(0.5f, 0f, 0f, 0.5f);//color
    public float fadeSpeed = 2f; //Overlay fading in/out

    //private bool playerInBounds = false;
    private static int activeWarningZones = 0;
    private Color transparentColor;

    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        if (preWarningCanvas == null)
        {
            GameObject canvasObj = GameObject.FindGameObjectWithTag("PreOOB_WarnCanvas");
            if (canvasObj != null) preWarningCanvas = canvasObj;
        }

        if (preWarningText == null)
        {
            GameObject textObj = GameObject.FindGameObjectWithTag("PreOOB_WarnText");
            if (textObj != null) preWarningText = textObj.GetComponent<TMP_Text>();
        }

        if (screenOverlay == null)
        {
            GameObject panelObj = GameObject.FindGameObjectWithTag("PreOOB_WarnPanel");
            if (panelObj != null) screenOverlay = panelObj.GetComponent<Image>();
        }
        transparentColor = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);

        preWarningCanvas.SetActive(false);
        screenOverlay.color = transparentColor;

    }

    // Update is called once per frame
    void Update()
    {
        if (screenOverlay != null)
        {
            if (activeWarningZones > 0) //Fade in overlay
            {
                screenOverlay.color = Color.Lerp(screenOverlay.color, overlayColor, fadeSpeed * Time.deltaTime);
            }
            else
            {
                screenOverlay.color = Color.Lerp(screenOverlay.color, transparentColor, fadeSpeed * Time.deltaTime);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //playerInBounds = true;
            activeWarningZones++;

            if (preWarningCanvas != null) //Show warning
            {
                preWarningCanvas.SetActive(true);
            }


        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //playerInBounds = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //playerInBounds = false;
            activeWarningZones--;


            if (preWarningCanvas != null)
            {
                preWarningCanvas.SetActive(false);
            }
        }
    }



}

using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class OutOfBounds : MonoBehaviour
{
    [Header("Damage Settings")]
    public bool usePercentageDamage = true;
    public float damagePercent = 0.05f; //% of max hp as tick
    public int outOfBoundsDamage = 10;//Damage per tick
    public float damageInterval = 1f; //How often damage is applied
    private bool playerInBounds = false;

    [Header("Visuals")]
    public GameObject warningCanvas;
    public TMP_Text warningText;
    public Image screenOverlay;
    public Color overlayColor = new Color(0.5f, 0f, 0f, 0.5f);//color
    public float fadeSpeed = 2f; //Overlay fading in/out

    private GameHandler gameHandler;
    private Color transparentColor;

    private static int activeZoneCount = 0;
    private static float sharedDamageTimer = 0f;
    private static bool damageAppliedThisTick = false; //These prevent overlapping zones from dealing damage too much

    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        gameHandler = FindAnyObjectByType<GameHandler>();

        if (warningCanvas == null)
        {
            GameObject canvasObj = GameObject.FindGameObjectWithTag("OOB_WarnCanvas");
            if (canvasObj != null) warningCanvas = canvasObj;
        }

        if (warningText == null)
        {
            GameObject textObj = GameObject.FindGameObjectWithTag("OOB_WarnText");
            if (textObj != null) warningText = textObj.GetComponent<TMP_Text>() ;
        }

        if (screenOverlay == null)
        {
            GameObject panelObj = GameObject.FindGameObjectWithTag("OOB_WarnPanel");
            if (panelObj != null) screenOverlay = panelObj.GetComponent<Image>();
        }

        transparentColor = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);

        warningCanvas.SetActive(false);
        screenOverlay.color = transparentColor;

    }

    // Update is called once per frame
    void Update()
    {
        sharedDamageTimer += Time.deltaTime;

        if (sharedDamageTimer >= damageInterval)
        {
            damageAppliedThisTick = false;
        }
        if (playerInBounds)
        {

            if (sharedDamageTimer >= damageInterval && !damageAppliedThisTick)
            {
                if (gameHandler != null)
                {
                    int damageToApply;

                    if (usePercentageDamage)
                    {
                        damageToApply = Mathf.RoundToInt(GameHandler.playerMaxHealth * damagePercent);
                    }
                    else
                    {
                        damageToApply = outOfBoundsDamage;
                    }

                    gameHandler.playerOutOfBounds(damageToApply);
                    damageAppliedThisTick = true;
                    sharedDamageTimer = 0f;
                }
                
            }


            if (screenOverlay != null) //Fade in overlay
            {
                if (activeZoneCount > 0) //if player in a zone
                {
                    screenOverlay.color = Color.Lerp(screenOverlay.color, overlayColor, fadeSpeed * Time.deltaTime);

                }
                else //player not in zone
                {
                    screenOverlay.color = Color.Lerp(screenOverlay.color, transparentColor, fadeSpeed * Time.deltaTime);
                }
            }
        }   
        else
        if (screenOverlay != null) //Fade out overlay
        {
            screenOverlay.color = Color.Lerp(screenOverlay.color, transparentColor, fadeSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.CompareTag("Player"))
        {
            playerInBounds = true;
            activeZoneCount++;
            

            if (warningCanvas != null) //Show warning
            {
                warningCanvas.SetActive(true);
            }


        }
    }

    void OnTriggerStay(Collider other)
    {
       if (other.gameObject.CompareTag("Player"))
       {
           playerInBounds = true;
       }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInBounds = false;
            activeZoneCount--;

            if (warningCanvas != null)
            {
                warningCanvas.SetActive(false);
            }
        }
    }



}

using UnityEngine;

public class UnderwaterLightParticleShafts : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Light directionalLight;
    public Transform surfaceMarker;
    public Transform bottomMarker;

    [Header("Settings")]
    public int shaftCount = 5;
    public float maxDistance = 30f;
    public float shaftWidth = 3f;
    public float updateInterval = 2f;

    [Header("Depth Fade")]
    public float surfaceShaftIntensity = 1f;
    public float deepShaftIntensity = 0f;
    public float fadeStartDepth = 0f;
    public float fadeEndDepth = 0.5f;

    private GameObject[] shafts;
    private ParticleSystem[] particleSystems;
    private float updateTimer;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (directionalLight == null)
        {
            directionalLight = RenderSettings.sun;
        }

        CreateLightShafts();
    }

    
    void CreateLightShafts()
    {
        shafts = new GameObject[shaftCount];

        for (int i = 0; i < shaftCount; i++)
        {
            GameObject shaft = new GameObject($"LightShaft_{i}");
            shaft.transform.parent = transform;

            ParticleSystem ps = shaft.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 3f;
            main.startSpeed = 0.5f;
            main.startSize = 0.05f;
            main.startColor = new Color(1f, 1f, 1f, 0.3f);
            main.maxParticles = 200;
            main.loop = true;

            var emission = ps.emission;
            emission.rateOverTime = 50;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(shaftWidth, 0.1f, shaftWidth);

            var velocityOverLifetime = ps.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
            velocityOverLifetime.y = -1f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) }, new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.5f, 0.2f), new GradientAlphaKey(0f, 1f) });
            colorOverLifetime.color = grad;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Stretch;
            renderer.lengthScale = 2f;
            renderer.material = CreateLightShaftMaterial();

            shafts[i] = shaft;
            particleSystems[i] = ps;
        }
        PositionShafts();
    }

    Material CreateLightShaftMaterial()
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        mat.SetColor("_BaseColor", new Color(0.9f, 0.95f, 1f, 0.5f));
        mat.SetFloat("_Surface", 1); //Transparent
        mat.SetFloat("_Blend", 0);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        return mat;
    }

    void Update()
    {
        if (player ==null || directionalLight == null) return;

        float normalizedDepth = CalculateDepth();
        UpdateShaftIntensity(normalizedDepth);

        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            PositionShafts();
        }

        Vector3 lightDir = directionalLight.transform.forward;
        Quaternion lightRotation = Quaternion.LookRotation(lightDir, Vector3.up);

        foreach(GameObject shaft in shafts)
        {
            if (shaft != null)
            {
                shaft.transform.rotation = lightRotation;
            }
        }
    }

    float CalculateDepth()
    {
        if (surfaceMarker != null && bottomMarker != null)
        {
            float surfaceHeight = surfaceMarker.position.y;
            float bottomheight = bottomMarker.position.y;
            float totalDepth = surfaceHeight - bottomheight;
            if (totalDepth > 0)
            {
                float currentDepth = surfaceHeight - player.position.y;
                return Mathf.Clamp01(currentDepth / totalDepth);

            }
        }

        return Shader.GetGlobalFloat("_WaterDepth");
    }

    void UpdateShaftIntensity(float depth)
    {
        float fadeAmount = Mathf.InverseLerp(fadeStartDepth, fadeEndDepth, depth);
        float intensity = Mathf.Lerp(surfaceShaftIntensity, deepShaftIntensity, depth);

        for (int i = 0;i<particleSystems.Length;i++)
        {
            if (particleSystems[i] != null)
            {
                var emission = particleSystems[i].emission;
                emission.rateOverTime = 60 * intensity;

                var main = particleSystems[i].main;
                Color baseColor = new Color(1f, 1f, 1f, 0.4f * intensity);
                main.startColor = baseColor;
            }
        }
    }
    void PositionShafts()
    {
        if (player == null) return;

        

        for (int i = 0;i < shafts.Length;i++)
        {
            if (shafts[i] == null) continue; 

            float angle = (i / (float)shaftCount) * Mathf.PI * 2f + Random.Range(-0.3f,0.3f);
            float distance = Random.Range(10f, maxDistance);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);

            Vector3 position = player.position + offset;
            position.y = player.position.y + Random.Range(15f, 25f);

            shafts[i].transform.position = position;
           
        }

    }

    void OnDrawGizmos()
    {
        

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        foreach (GameObject shaft in shafts)
        {
            if (shaft != null)
            {
                Gizmos.DrawWireSphere(shaft.transform.position, 1f);
                Gizmos.DrawLine(shaft.transform.position, shaft.transform.position + shaft.transform.forward * 10f);
            }
        }
    }

    void OnDestroy()
    {
        if (shafts !=null)
        {
            foreach(GameObject shaft in shafts)
            {
                if (shaft != null)
                {
                    DestroyImmediate(shaft);
                }
            }
        }
    }
}

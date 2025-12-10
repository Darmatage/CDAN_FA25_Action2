using UnityEngine;

public class PhysicalWall : MonoBehaviour
{
    public bool blockPlayer = true;
    private Collider wallCollider;
    private MeshRenderer meshRenderer;
    void Start()
    {
        wallCollider = GetComponent<Collider>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        if (wallCollider!= null)
        {
            wallCollider.isTrigger = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (wallCollider != null)
        {
            wallCollider.enabled = blockPlayer;
        }
    }
}

using Assets.Scripts.Managers;
using UnityEngine;

public class MapController : MonoBehaviour
{

    public BoxCollider minimapBoundingBox;

    void Start()
    {
        MinimapManager.Instance.UpdateMinimap(minimapBoundingBox);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

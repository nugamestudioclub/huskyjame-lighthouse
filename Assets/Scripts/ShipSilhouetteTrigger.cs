using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSilhouetteTrigger : MonoBehaviour
{
    [SerializeField]
    Material[] clearMat;
    [SerializeField]
    Material[] silhouetteMat;
    MeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = gameObject.GetComponent<MeshRenderer>();
        triggerSilhouette();
    }

    public void triggerSilhouette()
    {
        mesh.materials = silhouetteMat;
    }
    public void triggerClear()
    {
        mesh.materials = clearMat;
    }
}

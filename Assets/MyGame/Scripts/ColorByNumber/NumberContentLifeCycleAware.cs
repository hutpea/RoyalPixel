using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberContentLifeCycleAware : MonoBehaviour
{
    private void OnDestroy()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null) {
            Destroy(renderer.sharedMaterial.mainTexture);
            Destroy(renderer.sharedMaterial);
        }
    }
}

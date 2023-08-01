using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsing : MonoBehaviour
{
    private bool coroutineAllowed;
    Vector2 startLocal = new Vector2(1, 1);
    private void Awake()
    {
        startLocal = transform.localScale;
    }
    private void Start()
    {
        coroutineAllowed = true;
    }
    private void OnMouseOver()
    {
        if (coroutineAllowed)
        {
            StartCoroutine("StartPulsing");
        }
    }
    private void OnEnable()
    {
        transform.localScale = startLocal;
    }
    public IEnumerator StartPulsing()
    {
        coroutineAllowed = false;
        for (float i = 0; i < 1f; i += 0.08f)
        {
            transform.localScale = new Vector3(
                Mathf.Lerp(transform.localScale.x, transform.localScale.x + 0.01f, Mathf.SmoothStep(0, 1, i)),
                Mathf.Lerp(transform.localScale.y, transform.localScale.y + 0.01f, Mathf.SmoothStep(0, 1, i)),
                Mathf.Lerp(transform.localScale.z, transform.localScale.z + 0.01f, Mathf.SmoothStep(0, 1, i)));
            yield return new WaitForSeconds(0.01f);
        }
        for (float i = 0; i < 1f; i += 0.08f)
        {
            transform.localScale = new Vector3(
                Mathf.Lerp(transform.localScale.x, transform.localScale.x - 0.01f, Mathf.SmoothStep(0, 1, i)),
                Mathf.Lerp(transform.localScale.y, transform.localScale.y - 0.01f, Mathf.SmoothStep(0, 1, i)),
                Mathf.Lerp(transform.localScale.z, transform.localScale.z - 0.01f, Mathf.SmoothStep(0, 1, i)));
            yield return new WaitForSeconds(0.01f);

        }
        coroutineAllowed = true;
    }
}

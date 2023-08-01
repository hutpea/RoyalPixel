using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTut : MonoBehaviour
{
    [SerializeField] Transform content;
    [SerializeField] List<Vector3> childs;
    Vector3 posContent;
    private void Start()
    {
        posContent = content.transform.position;
        childs = new List<Vector3>();
        foreach(Transform child in content)
        {
            childs.Add(child.transform.position);
        }
    }
    public void SetPos()
    {
        content.position = posContent;
    }
}

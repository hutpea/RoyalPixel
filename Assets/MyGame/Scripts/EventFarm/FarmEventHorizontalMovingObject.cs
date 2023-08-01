using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FarmEventHorizontalMovingObject : MonoBehaviour
{
    public float speed;
    public Transform leftPos;
    public Transform rightPos;
    public bool currentDirection; //false is left, true is right
    public bool isRightFirst;
    public bool enableMoving;

    private void Awake()
    {
        currentDirection = isRightFirst;
    }

    private void Start()
    {
        enableMoving = true;
        MoveTo();
    }

    private void MoveTo()
    {
        if (!enableMoving) return;
        Transform currentTransform = currentDirection ? rightPos : leftPos;
        transform.DOMove(currentTransform.position, speed).OnComplete(() =>
        {
            currentDirection = !currentDirection;
            MoveTo();
        });
    }
}

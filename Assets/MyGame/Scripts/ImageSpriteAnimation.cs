using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageSpriteAnimation : MonoBehaviour
{
    public List<Sprite> sprites;
    public float animationFrameDuration;

    private Image image;

    public bool enablePlayAnimation;

    private float timer;
    private int currentSpriteIndex;
    
    private void Awake()
    {
        image = GetComponent<Image>();
        timer = 0f;
        animationFrameDuration = 0.4f;
        animationFrameDuration = (float)UnityEngine.Random.Range(animationFrameDuration - 0.1f, animationFrameDuration + 0.1f);
        currentSpriteIndex = 0;
    }

    private void Start()
    {
        if (sprites.Count != 0)
        {
            enablePlayAnimation = true;
        }
        else
        {
            enablePlayAnimation = false;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= animationFrameDuration)
        {
            timer = 0;
            if (!enablePlayAnimation) return;
            image.sprite = sprites[currentSpriteIndex];
            currentSpriteIndex++;
            if(currentSpriteIndex > sprites.Count - 1)
                currentSpriteIndex = 0;
        }
    }

    [Button]
    public void SetRectSizeToOriginalSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Sprite imageSprite = GetComponent<Image>().sprite;
        rectTransform.sizeDelta = new Vector2(imageSprite.rect.width, imageSprite.rect.height);
    }
}

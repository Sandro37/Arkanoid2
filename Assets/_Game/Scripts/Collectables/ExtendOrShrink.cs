using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendOrShrink : Collectable
{
    [SerializeField] private float newWidth = 2.5f;
    protected override void ApplyEffect()
    {
        if(Paddle.Instance != null && !Paddle.Instance.PaddleIsTransforming)
        {
            Paddle.Instance.StartWidthAnimation(newWidth);
        }
    }
}

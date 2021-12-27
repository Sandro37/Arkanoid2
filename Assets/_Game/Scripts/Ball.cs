using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool isLightningBall;
    private SpriteRenderer sr;
    [SerializeField] ParticleSystem ParticleSystem;
    [SerializeField] private float lightningBallDuration = 10f;
    public static event Action<Ball> OnBallDeath;
    public static event Action<Ball> OnLightningBallEnable;
    public static event Action<Ball> OnLightningBallDisable;
    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }
    public void Die()
    {
        OnBallDeath?.Invoke(this);
        Destroy(this.gameObject, 1f);
    }

    internal void StartLightningBall()
    {
        if (!this.isLightningBall)
        {
            this.isLightningBall = true;
            this.sr.enabled = false;
            ParticleSystem.gameObject.SetActive(true);
            StartCoroutine(StopLigthtningBallAfterTime(this.lightningBallDuration));

            OnLightningBallEnable?.Invoke(this);
        }
    }

    IEnumerator StopLigthtningBallAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StopLightningBall();
    }

    private void StopLightningBall()
    {
        if(this.isLightningBall)
        {
            this.isLightningBall = false;
            this.sr.enabled = true;
            ParticleSystem.gameObject.SetActive(false);
            OnLightningBallDisable?.Invoke(this);
        }
    }
}

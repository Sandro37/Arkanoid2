using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Brick : MonoBehaviour
{
    [SerializeField] int hitPoints = 1;
    [SerializeField] ParticleSystem destroyEffect;
    private BoxCollider2D boxCollider;
    private SpriteRenderer sr;
    public static event Action<Brick> OnBrickDestruction;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        Ball.OnLightningBallEnable += OnLightningBallEnable;
        Ball.OnLightningBallDisable += OnLightningBallDisable;
    }

    private void OnLightningBallDisable(Ball obj)
    {
        if (this != null)
        {
            this.boxCollider.isTrigger = false ;
        }
    }
    private void OnLightningBallEnable(Ball obj)
    {
        if (this != null)
        {
            this.boxCollider.isTrigger = true;
        }
    }

    private void OnDisable()
    {
        Ball.OnLightningBallEnable -= OnLightningBallEnable;
        Ball.OnLightningBallDisable -= OnLightningBallDisable;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        ApplyCollisionLogic(ball);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            ApplyCollisionLogic(ball);
        }
    }
    private void ApplyCollisionLogic(Ball ball)
    {
        this.hitPoints--;
        if (this.hitPoints <= 0 || (ball != null && ball.isLightningBall))
        {
            BricksManager.Instance.RemainingBricks.Remove(this);
            OnBrickDestruction?.Invoke(this);
            OnBrickDestroy();
            Destroy(this.gameObject);
            SpanwDestroyEffect();
        }
        else
        {
            this.sr.sprite = BricksManager.Instance.Sprites[this.hitPoints - 1];
        }
    }

    private void OnBrickDestroy()
    {
        float buffsSpawnChance = UnityEngine.Random.Range(0, 100f);
        float deBuffsSpawnChance = UnityEngine.Random.Range(0, 100f);
        bool alreadySpawned = false;

        if (buffsSpawnChance <= CollectablesManager.Instance.GetBuffChance())
        {
            alreadySpawned = true;
            Collectable newBuff = this.SpawnCollectable(true);
        }

        if(deBuffsSpawnChance <= CollectablesManager.Instance.GetDeBuffChance() && !alreadySpawned)
        {
            Collectable NewDebuffChance = this.SpawnCollectable(false);
        }
    }

    private Collectable SpawnCollectable(bool isBuff)
    {
        List<Collectable> collectables;
        if (isBuff)
        {
            collectables = CollectablesManager.Instance.AvailableBuffs;
        }
        else
        {
            collectables = CollectablesManager.Instance.AvailableDeBuffs;
        }

        int buffIndex = UnityEngine.Random.Range(0, collectables.Count);

        Collectable prefab = collectables[buffIndex];
        Collectable newCollectable = Instantiate(prefab,this.transform.position, Quaternion.identity) as Collectable;
        return newCollectable;
    }

    private void SpanwDestroyEffect()
    {
        Vector3 brickPos = gameObject.transform.position;
        Vector3 spawnPosition = new Vector3(brickPos.x, brickPos.y, brickPos.z - 0.2f);
        GameObject effect = Instantiate(destroyEffect.gameObject, spawnPosition, Quaternion.identity);

        MainModule mm = effect.GetComponent<ParticleSystem>().main;
        mm.startColor = this.sr.color;

        Destroy(effect, destroyEffect.main.startLifetime.constant);

    }

    public void Init(Transform containerTransform, Sprite sprite, Color color, int hitPoints)
    {
        this.transform.SetParent(containerTransform);
        this.sr.sprite = sprite;
        sr.color = color;
        this.hitPoints = hitPoints;
    }
}

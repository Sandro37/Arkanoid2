using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    private Camera mainCamera;
    private float paddleInitalY;
    private float defaultPaddleWidthInPixels = 200f;
    private SpriteRenderer sr;
    private float defaultLeftClamp = 135f;
    private float defaultRightClamp = 410f;
    private BoxCollider2D boxCollider2D;
    private bool isPadleShooting;
    [SerializeField] private float extendShrinkDuration = 10f;
    [SerializeField] private float paddleWidth = 2f;
    [SerializeField] private float paddleHeight = 0.28f;
    [SerializeField] private GameObject LeftMuzle;
    [SerializeField] private GameObject RightMuzle;
    [SerializeField] private Projectile ProjectilePrefab;


    #region Singleton
    private static Paddle _instance;
    public static Paddle Instance => _instance;

    public bool PaddleIsTransforming { get; set; }

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
            _instance = this;
    }
    #endregion

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        sr = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        paddleInitalY = this.transform.position.y;
    }
    private void Update()
    {
        PaddleMovement();
        UpdateMuzzlePosition();
    }

    private void UpdateMuzzlePosition()
    {
        LeftMuzle.transform.position = new Vector3(transform.position.x - (sr.size.x/2) + 0.1f, this.transform.position.y + 0.2f, this.transform.position.z);
        RightMuzle.transform.position = new Vector3(transform.position.x + (sr.size.x / 2) - 0.153f, this.transform.position.y + 0.2f, this.transform.position.z);
    }

    public void StartWidthAnimation(float newWidth)
    {
        StartCoroutine(AnimatePaddleWidth(newWidth));
    }

    private IEnumerator AnimatePaddleWidth(float width)
    {
        this.PaddleIsTransforming = true;
        this.StartCoroutine(ResetPaddleWidthAfterTime(this.extendShrinkDuration));

        if(width > this.sr.size.x)
        {
            float currentWidth = this.sr.size.x;
            while (currentWidth  < width)
            {
                currentWidth += Time.deltaTime * 2;
                this.sr.size = new Vector2(currentWidth, paddleHeight);
                boxCollider2D.size = new Vector2(currentWidth, boxCollider2D.size.y);
                yield return null;
            }
        }
        else
        {
            float currentWidth = this.sr.size.x;
            while (currentWidth > width) 
            {
                currentWidth -= Time.deltaTime * 2;
                this.sr.size = new Vector2(currentWidth, paddleHeight);
                boxCollider2D.size = new Vector2(currentWidth, boxCollider2D.size.y);
                yield return null;
            }
        }

        this.PaddleIsTransforming = false;  
    }

    private IEnumerator ResetPaddleWidthAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.StartWidthAnimation(this.paddleWidth);
    }

    private void PaddleMovement()
    {
        float paddleShift = (defaultPaddleWidthInPixels - (defaultPaddleWidthInPixels / 2)* this.sr.size.x);
        float leftClamp = defaultLeftClamp - paddleShift;
        float rightClamp = defaultRightClamp + paddleShift;
        float mousePositionPixels = Mathf.Clamp(Input.mousePosition.x, leftClamp, rightClamp);
        float mousePositionWorldX = mainCamera.ScreenToWorldPoint(new Vector3(mousePositionPixels, 0, 0)).x;
        this.transform.position = new Vector3(mousePositionWorldX, paddleInitalY, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();

            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 paddleCenter = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0);

            ballRb.velocity = Vector2.zero;

            float difference = paddleCenter.x - hitPoint.x;

            if (hitPoint.x < paddleCenter.x)
            {
                ballRb.AddForce(new Vector2(-(Mathf.Abs(difference * 200f)), BallsManager.Instance.InitialBallSpeed));
            }
            else
            {
                ballRb.AddForce(new Vector2((Mathf.Abs(difference * 200f)), BallsManager.Instance.InitialBallSpeed));
            }
        }
    }

    public void StartShooting()
    {
        if (!this.isPadleShooting)
        {
            this.isPadleShooting = true;
            StartCoroutine(StartShootingCoroutine());
        }
    }

    IEnumerator StartShootingCoroutine()
    {
        float fireCoolDown = .5f;
        float fireColldDownLeft = 0f;

        float shootingDuration = 10f;
        float shootingDurationLeft = shootingDuration; 

        while(shootingDurationLeft >= 0)
        {
            fireColldDownLeft -= Time.deltaTime;
            shootingDurationLeft -= Time.deltaTime;

            if(fireColldDownLeft <= 0f)
            {
                this.Shoot();
                fireColldDownLeft = fireCoolDown;
            }
            yield return null;
        }
        this.isPadleShooting = false;
        LeftMuzle.SetActive(false);
        RightMuzle.SetActive(false);
    }

    private void Shoot()
    {
        LeftMuzle.SetActive(false);
        RightMuzle.SetActive(false);
        
        LeftMuzle.SetActive(true);
        RightMuzle.SetActive(true);

        this.SpawnBullet(LeftMuzle);
        this.SpawnBullet(RightMuzle);
    }

    private void SpawnBullet(GameObject muzzle)
    {
        Vector3 spawnPosition = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y + 0.2f, muzzle.transform.position.z);
        Projectile bullet = Instantiate(ProjectilePrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(new Vector2(0, 450f));
    }
}

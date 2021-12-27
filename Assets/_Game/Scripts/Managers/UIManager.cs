using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Text TargetText;
    public Text ScoreText;
    public Text LivesText;

    public int Score { get; set; }
    private void Start()
    {
        Brick.OnBrickDestruction += OnBrickDestruction;
        BricksManager.OnLevelLoaded += OnLevelLoaded;
        GameManager.OnLiveLost += OnLiveLost;
        OnLiveLost(GameManager.Instance.AvailableLives);
        OnLevelLoaded();
    }

    private void OnLiveLost(int remainingLives)
    {
        LivesText.text = $@"LIVES: {remainingLives}";
    }

    private void OnLevelLoaded()
    {
        UpdateRemainingBricksText();
        UpdateScoreText(0);
    }

    private void UpdateScoreText(int increment)
    {
        this.Score += increment;
        string scoreString = this.Score.ToString().PadLeft(5, '0');
        ScoreText.text = $@"Score:{scoreString}";
    }

    private void OnBrickDestruction(Brick obj)
    {
        UpdateRemainingBricksText();
        UpdateScoreText(10);
    }

    private void UpdateRemainingBricksText()
    {
        TargetText.text = $@"Target:
{BricksManager.Instance.RemainingBricks.Count} / {BricksManager.Instance.InitBricksCount}";
    }

    private void OnDisable()
    {
        Brick.OnBrickDestruction -= OnBrickDestruction;
        BricksManager.OnLevelLoaded -= OnLevelLoaded;
        GameManager.OnLiveLost -= OnLiveLost;
    }
    private void OnEnable()
    {
        Brick.OnBrickDestruction -= OnBrickDestruction;
        BricksManager.OnLevelLoaded -= OnLevelLoaded;
        GameManager.OnLiveLost -= OnLiveLost;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesManager : MonoBehaviour
{
    #region Singleton
    private static CollectablesManager _instance;
    public static CollectablesManager Instance => _instance;
    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
            _instance = this;
    }
    #endregion


    public List<Collectable> AvailableBuffs;
    public List<Collectable> AvailableDeBuffs;

    [Range(0,100)]
    [SerializeField] private float buffChance;
    [Range(0,100)]
    [SerializeField] private  float debuffChance;

    public float GetBuffChance() => buffChance;
    public float GetDeBuffChance() => debuffChance;
}

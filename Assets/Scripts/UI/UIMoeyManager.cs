using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMoeyManager : MonoBehaviour
{
    public static UIMoeyManager Instance;

    [SerializeField] private TextMeshProUGUI moneyText;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        moneyText.text = SQLManager.Instance.GetUserMoney().ToString();
    }

    public void UpdateMoney()
    {
        moneyText.text = SQLManager.Instance.GetUserMoney().ToString();
    }

}

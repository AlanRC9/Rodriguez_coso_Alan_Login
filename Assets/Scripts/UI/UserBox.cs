using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserBox : MonoBehaviour
{

    private TextMeshProUGUI userNameText;
    private int userId;

    private void Awake()
    {
        userNameText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetUserName(string dbUserName)
    {
        userNameText.text = dbUserName;
    }

    public void SetUserId(int dbUserId)
    {
        userId = dbUserId;
    }

    public void DeleteUser()
    {
        SQLManager.Instance.DeleteUser(userId);
        Destroy(this.gameObject);
    }

}

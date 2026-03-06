using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsersList : MonoBehaviour
{
    [SerializeField] private GameObject UserBoxPrefab;

    private void Start()
    {
        List<User> usersList = SQLManager.Instance.GetUsersFromDB();

        foreach (User user in usersList)
        {
            GameObject currentUserBox = Instantiate(UserBoxPrefab, transform);
            currentUserBox.GetComponent<UserBox>().SetUserName(user.userName);
            currentUserBox.GetComponent<UserBox>().SetUserId(user.userId);
        }
    }

}

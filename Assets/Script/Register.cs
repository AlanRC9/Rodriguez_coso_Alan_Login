using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_Text errorText;
    public void TryRegister()
    {
        bool success;
        success = SQLManager.Instance.Register(username.text, password.text);

        if (success) SceneManager.LoadScene("Lobby");
        else StartCoroutine(ShowAlert());
    }

    private IEnumerator ShowAlert()
    {
        errorText.text = "Error: This may be due to one of the following reasons:\n" +
                 "- Some fields are missing.\n" +
                 "- The password does not have the minimum of 8 characters.\n" +
                 "- The username is already taken.";

        yield return new WaitForSeconds(5);

        errorText.text = "";
    }
}

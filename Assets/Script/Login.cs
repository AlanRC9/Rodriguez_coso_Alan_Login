using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Login : MonoBehaviour
{

    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_Text errorText;

    public void TryLogIn()
    {
        bool success;
        success = SQLManager.Instance.Login(username.text, password.text);

        if (success) SceneManager.LoadScene("Lobby");
        else StartCoroutine(ShowAlert());
        
    }

    private IEnumerator ShowAlert()
    {
        errorText.text = "Error: This may be due to one of the following reasons:\n" +
                "- Some fields are missing.\n" +
                "- User does not exist\n" +
                "- Incorrect credentials\n";

        yield return new WaitForSeconds(5);

        errorText.text = "";
    }

}

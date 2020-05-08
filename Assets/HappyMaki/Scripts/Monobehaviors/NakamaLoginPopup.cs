using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NakamaLoginPopup : MonoBehaviour
{
    public TMP_InputField _inputFieldAccount;
    public TMP_InputField _inputFieldPassword;

    NakamaApi nakama;

    private void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();

    }

    public void Login()
    {
        nakama.Login(_inputFieldAccount.text, _inputFieldPassword.text);
    }

}

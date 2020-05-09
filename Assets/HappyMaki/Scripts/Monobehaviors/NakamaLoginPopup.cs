using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gamevanilla;

public class NakamaLoginPopup : MonoBehaviour
{
    public TMP_InputField _inputFieldAccount;
    public TMP_InputField _inputFieldPassword;
    public Popup parentPopup;
    public PopupOpener debugPopupOpener;

    NakamaApi nakama;

    private void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();
        EventManager.onLoginAttempt.AddListener(HandleOnLoginAttempt);

    }
    private void HandleOnLoginAttempt(AccountLoginResolution resolution)
    {
        switch (resolution)
        {
            case AccountLoginResolution.SUCCESS:
                parentPopup.Close();
                debugPopupOpener.OpenPopup();
                break;

            case AccountLoginResolution.FAILED:
                debugPopupOpener.OpenPopup();
                break;

            default:
                throw new System.Exception("AccountCreationResolution not implemented: " + resolution.ToString());
        }

    }

    public void Login()
    {
        nakama.Login(_inputFieldAccount.text, _inputFieldPassword.text);
    }

}

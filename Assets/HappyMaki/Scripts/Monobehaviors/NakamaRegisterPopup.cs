﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gamevanilla;
using Nakama;

public class NakamaRegisterPopup : MonoBehaviour
{
    public TMP_InputField _inputFieldName;
    public TMP_InputField _inputFieldEmail;
    public TMP_InputField _inputFieldPassword;
    public TMP_InputField _inputFieldPassword2;

    public Popup parentPopup;
    public PopupOpener debugPopupOpener;

    NakamaApi nakama;

    private void Start()
    {
        nakama = FindObjectOfType<NakamaApi>();
        EventManager.onAccountCreation.AddListener(HandleOnAccountCreation);
    }

    private void HandleOnAccountCreation(AccountCreationResolution resolution)
    {
        switch (resolution)
        {
            case AccountCreationResolution.SUCCESS:
                parentPopup.Close();
                debugPopupOpener.OpenPopup();
                break;

            case AccountCreationResolution.FAILED:
                debugPopupOpener.OpenPopup();
                break;

            default:
                throw new System.Exception("AccountCreationResolution not implemented: " + resolution.ToString());
        }
        
    }

    public void Register()
    {
        if (_inputFieldEmail.text.Length == 0 || _inputFieldPassword.text.Length == 0 || _inputFieldPassword2.text.Length == 0 || _inputFieldName.text.Length == 0)
        {
            DebugInfo.SetToast("Error", "Form incomplete. Please fill out all the fields.");
            debugPopupOpener.OpenPopup();
            return;
        }

        if (_inputFieldPassword.text.Length < 8)
        {
            DebugInfo.SetToast("Error", "Password must be at least 8 characters.");
            debugPopupOpener.OpenPopup();
            return;
        }

        if (_inputFieldPassword.text != _inputFieldPassword2.text)
        {
            DebugInfo.SetToast("Error", "Password doesn't match. Please try again.");
            debugPopupOpener.OpenPopup();
            return;
        }


        nakama.Register(_inputFieldName.text, _inputFieldEmail.text, _inputFieldPassword.text);
        
    }


}

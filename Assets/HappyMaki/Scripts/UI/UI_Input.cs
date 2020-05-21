﻿using Gamevanilla;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Input : MonoBehaviour
{
    MakiPopupOpener popupOpener;
    bool isOpened = false; 

    private void Start()
    {
        popupOpener = GetComponent<MakiPopupOpener>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (isOpened)
            {
                popupOpener.ClosePopup();
            }
            else
            {
                popupOpener.OpenPopup();
            }
            isOpened = !isOpened;
        }
    }
}

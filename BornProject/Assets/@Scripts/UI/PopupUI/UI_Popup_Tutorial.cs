using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup_Tutorial : UI_Popup
{
    public GameObject tutorialWindow; // 튜토리얼 창을 가리키는 게임 오브젝트

    private void Start()
    {
        // 게임 시작 시 튜토리얼 창을 활성화합니다.
        tutorialWindow.SetActive(true);
    }

    private void Update()
    {
        // 마우스 버튼이 클릭되었을 때 튜토리얼 창을 비활성화합니다.
        if (Input.GetMouseButtonDown(0))
        {
            tutorialWindow.SetActive(false);
        }
    }


}

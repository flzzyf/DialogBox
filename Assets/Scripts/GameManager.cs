using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    string s = "蔷薇asz123xcz\n阿萨德qwe2424wqeq\nasdasdasqwewqeq123123weasdda";

    TextBox lastTextBox;

    public static System.Action onClick;

    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            lastTextBox = TextBoxManager.instance.CreateTextBox(Vector2.one);
            lastTextBox.SetStyle(new TextBoxStyle
            {
                fontSize = 10,
                alignment = TextBoxAlignment.Center,
                fadeIn = true,
                fadeInOffset = 3,
                fadeOut = true,
                fadeOutOffset = 3,

                textColor = Color.red,
                textBGColor = Color.green,

                textBoxFadeOutMethod = TextBoxFadeOutMethod.OnClick,
            });

            lastTextBox.ShowText(s);
        }
        if (Input.GetKeyDown("g"))
        {
            lastTextBox = TextBoxManager.instance.CreateTextBox(Vector2.one);
            lastTextBox.SetStyle(new TextBoxStyle
            {
                fontSize = 10,
                alignment = TextBoxAlignment.Center,
                fadeIn = true,
                fadeInOffset = 3,
                fadeOut = true,
                fadeOutOffset = 3,
                playSpeedPerChar = 0,

                textColor = Color.black,
                textBGColor = Color.white,

                textBoxFadeOutMethod = TextBoxFadeOutMethod.DelayTime,
                fadeOutDelayTime = 1,
            });

            lastTextBox.ShowText(s);
        }
        if (Input.GetKeyDown("e"))
        {
            lastTextBox = TextBoxManager.instance.CreateTextBox(Vector2.one);
            lastTextBox.SetStyle(new TextBoxStyle
            {
                fontSize = 10,
                alignment = TextBoxAlignment.Left,
                fadeIn = true,
                fadeInOffset = 3,
                fadeOut = true,
                fadeOutOffset = 3,
                playSpeedPerChar = 0,
                textBoxFadeOutMethod = TextBoxFadeOutMethod.OnClick,
            });

            lastTextBox.ShowText(s);
        }
        if (Input.GetKeyDown("r"))
        {
            lastTextBox = TextBoxManager.instance.CreateTextBox(Vector2.one);
            lastTextBox.SetStyle(new TextBoxStyle
            {
                fontSize = 10,
                alignment = TextBoxAlignment.Left,
                fadeIn = true,
                fadeInOffset = 3,
                fadeOut = true,
                fadeOutOffset = 3,
                playSpeedPerChar = 0,
            });

            lastTextBox.ShowText(s);
        }

        if (Input.GetKeyDown("t"))
        {
            lastTextBox.HideText();
            //textBox.ShowCurrentTextImmediately();
        }

        if(Input.GetMouseButtonDown(0))
        {
            onClick?.Invoke();

            onClick = null;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    string s = "蔷薇asz123xcz\n阿萨德qwe2424wqeq\nasdasdasqwewqeq123123weasdda";
    string s2 = "蔷薇asz123xcz\n阿萨德qwe2424wqeq\nasdasdasqwewqeq123123weasdda\nasdaswqeq123123weasdda";

    public TextBox textBox;

    void Update()
    {
        if (Input.GetKeyDown("d"))
        {
            textBox.StopShowText();

            TextBoxStyle style = new TextBoxStyle
            {
                fontSize = 10,
                alignment = TextBoxAlignment.Center,
            };
            textBox.ShowText(s, style);
        }
        if (Input.GetKeyDown("f"))
        {
            textBox.StopShowText();

            TextBoxStyle style = new TextBoxStyle
            {
                fontSize = 10,
                alignment = TextBoxAlignment.Center,
                playSpeedPerChar = 0,
            };
            textBox.ShowText(s, style);
        }

        if (Input.GetKeyDown("e"))
        {
            textBox.StopShowText();

            textBox.ShowText(s2);
        }
        if (Input.GetKeyDown("r"))
        {
            textBox.StopShowText();

            TextBoxStyle style = new TextBoxStyle
            {
                fontSize = 10,
                alignment = TextBoxAlignment.Left,
                playSpeedPerChar = 0,
            };
            textBox.ShowText(s2, style);
        }

        if (Input.GetKeyDown("g"))
        {
            textBox.ClearText();
        }
    }

}

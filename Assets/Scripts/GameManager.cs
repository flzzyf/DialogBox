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
            textBox.ShowText(s, TextBoxAlignment.Center);
        }
        if (Input.GetKeyDown("f"))
        {
            textBox.StopShowText();
            textBox.ShowText(s, TextBoxAlignment.Center, 0);
        }

        if (Input.GetKeyDown("e"))
        {
            textBox.StopShowText();
            textBox.ShowText(s2);
        }
        if (Input.GetKeyDown("r"))
        {
            textBox.StopShowText();
            textBox.ShowText(s2, TextBoxAlignment.Left, 0);
        }

        if (Input.GetKeyDown("g"))
        {
            textBox.ClearText();
        }
    }

}

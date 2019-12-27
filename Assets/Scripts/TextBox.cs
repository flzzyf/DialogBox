using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    public Text text;

    void Start()
    {
        SetText("阿萨德晴雯");
    }

    void Update()
    {
        if(Input.GetKeyDown("d"))
        {

        }
    }

    //立即设置文本框的文本
    public void SetText(string str)
    {
        text.text = str;


    }

    //获取指定字符在该文本框中的尺寸
    Vector2 GetCharSizeInText(char ch, Text text)
    {
        string last = text.text;

        text.text = ch.ToString();
        Vector2 size = new Vector2(text.preferredWidth, text.preferredHeight);
        text.text = last;

        return size;
    }
}
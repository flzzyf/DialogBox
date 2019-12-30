using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBoxManager : Singleton<TextBoxManager>
{
    Canvas canvas;

    void Awake()
    {
        GameObject prefab_TextBG = Resources.Load("Prefabs/TextBG") as GameObject;
        SimpleObjectPool.instance.NewPool("TextBG", prefab_TextBG, 5, new GameObject("TextBGParent").transform);

        GameObject prefab_TextBox = Resources.Load("Prefabs/TextBox") as GameObject;
        SimpleObjectPool.instance.NewPool("TextBox", prefab_TextBox, 1, new GameObject("TextBoxParent").transform);

        canvas = FindObjectOfType<Canvas>();
    }

    //创建文本框
    public TextBox CreateTextBox(Vector2 worldPos)
    {
        TextBox textBox = SimpleObjectPool.instance.SpawnFromPool("TextBox").GetComponent<TextBox>();
        textBox.transform.SetParent(canvas.transform);
        textBox.transform.localScale = Vector3.one;

        textBox.SetPos(worldPos);

        textBox.AddOnHideCallback(() =>
        {
            SimpleObjectPool.instance.PutBackObject("TextBox", textBox.gameObject);
        });

        return textBox;
    }
}
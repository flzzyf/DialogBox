using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TextBoxAlignment { Left, Center}

//对话框，文本会有个背景。目前只有左对齐的文本能显示文本打出效果，居中的只能立即显示
public class TextBox : MonoBehaviour
{
    public Text text;
    public Transform textBGParent;

    //每个字符播放间隔
    public float defaultDurationPerChar = .01f;

    IEnumerator setTextCor;

    List<GameObject> textBGList;

    #region Unity回调

    void Awake()
    {
        GameObject prefab_TextBG = Resources.Load("Prefabs/TextBG") as GameObject;

        SimpleObjectPool.instance.NewPool("TextBG", prefab_TextBG, 5, new GameObject("TextBGParent").transform);

        textBGList = new List<GameObject>();
    }

    #endregion

    //打出显示文本
    public void ShowText(string str, TextBoxAlignment alignment = default, float durationPerChar = -1)
    {
        setTextCor = SetTextCor(str, alignment, durationPerChar);
        StartCoroutine(setTextCor);
    }

    //停止显示文本
    public void StopShowText()
    {
        if(setTextCor != null)
            StopCoroutine(setTextCor);

        ClearText();
    }

    //清除文本，回收组件
    public void ClearText()
    {
        text.text = "";

        //回收文本背景
        foreach (var item in textBGList)
        {
            SimpleObjectPool.instance.PutBackObject("TextBG", item);
        }
        textBGList = new List<GameObject>();
    }

    IEnumerator SetTextCor(string str, TextBoxAlignment alignment, float durationPerChar = -1)
    {
        text.text = "";

        //设置默认播放速度
        if (durationPerChar == -1)
        {
            durationPerChar = defaultDurationPerChar;
        }

        //设置文本框尺寸
        Vector2 size = GetStringSizeInText(str, text);
        text.rectTransform.sizeDelta = size;

        //按换行符切割文本
        string[] lineStrs = str.Split('\n');

        //为每一行文本创建背景块
        Vector2 startingPos = default;
        startingPos.y = text.rectTransform.rect.yMax;

        foreach (var lineStr in lineStrs)
        {
            //不是首行，加上换行
            if(text.text != "")
            {
                text.text += "\n";
            }

            Vector2 lineSize = GetStringSizeInText(lineStr, text);

            //如果是居中，添加空格
            if(alignment == TextBoxAlignment.Center)
            {
                //为了居中应该添加的空格数量
                int spaceCount = Mathf.CeilToInt ((size.x - lineSize.x) / 2 / GetCharSizeInText(' ', text).x);
                for (int i = 0; i < spaceCount; i++)
                {
                    text.text += " ";
                }
            }

            //获取这行文本的左上角坐标
            //如果是居中
            if (alignment == TextBoxAlignment.Center)
            {
                startingPos.x = -lineSize.x / 2;
            }
            else
            {
                startingPos.x = -size.x / 2;
            }

            //如果是瞬间显示
            if (durationPerChar == 0)
            {
                text.text += lineStr;

                //生成文本背景
                GenerateLineTextBG(startingPos, lineStr);
            }
            else
            {
                //慢慢打出
                Vector2 firstCharSize = GetCharSizeInText(lineStr[0], text);

                Vector2 lineStartingPos = startingPos;
                lineStartingPos.x += firstCharSize.x / 2;
                lineStartingPos.y -= firstCharSize.y / 2;

                foreach (var ch in lineStr)
                {
                    text.text += ch;

                    //生成文本背景
                    Vector2 charSize = GetCharSizeInText(ch, text);
                    GenerateTextBG(lineStartingPos, charSize);

                    lineStartingPos.x += charSize.x;

                    yield return new WaitForSeconds(durationPerChar);
                }
            }

            startingPos.y -= lineSize.y * text.lineSpacing;
        }
    }

    //生成一行文本的背景，初始位置为文本左上角
    void GenerateLineTextBG(Vector2 startingPos, string str)
    {
        Vector2 firstCharSize = GetCharSizeInText(str[0], text);

        startingPos.x += firstCharSize.x / 2;
        startingPos.y -= firstCharSize.y / 2;

        foreach (var ch in str)
        {
            Vector2 charSize = GetCharSizeInText(ch, text);
            GenerateTextBG(startingPos, charSize);

            startingPos.x += charSize.x;
        }
    }

    //生成文字背景块
    void GenerateTextBG(Vector2 pos, Vector2 size)
    {
        GameObject textBG = SimpleObjectPool.instance.SpawnFromPool("TextBG");
        textBGList.Add(textBG);

        textBG.transform.SetParent(textBGParent);
        textBG.transform.localScale = Vector3.one;

        textBG.GetComponent<RectTransform>().anchoredPosition = new Vector3(pos.x, pos.y, 0);

        textBG.GetComponent<RectTransform>().sizeDelta = size;
    }

    #region 帮助方法

    //获得指定字符串在文本框中的尺寸
    Vector2 GetStringSizeInText(string str, Text text)
    {
        string last = text.text;

        text.text = str;
        Vector2 size = new Vector2(text.preferredWidth, text.preferredHeight);
        text.text = last;

        return size;
    }

    //获取指定字符在文本框中的尺寸
    Vector2 GetCharSizeInText(char ch, Text text)
    {
        string last = text.text;

        text.text = ch.ToString();
        Vector2 size = new Vector2(text.preferredWidth, text.preferredHeight);
        text.text = last;

        return size;
    }

    #endregion
}
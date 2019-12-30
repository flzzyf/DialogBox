using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public enum TextBoxAlignment { Left, Center}
public enum TextBoxFadeOutMethod { Null, OnClick, DelayTime}

//文本框样式
public class TextBoxStyle
{
    public int fontSize = 6;

    //靠左或者居中对齐
    public TextBoxAlignment alignment;

    //文本和文本背景的颜色，默认黑底白字
    public Color textColor = Color.white;
    public Color textBGColor = Color.black;

    //文本播放速度（每个字符时间）
    public float playSpeedPerChar = .01f;

    //是否淡入淡出以及相应时长
    public bool fadeIn;
    public bool fadeOut;
    public float fadeInDuration = .8f;
    public float fadeOutDuration = .7f;

    //淡入和淡出时的位移，单位是该Text行高
    public float fadeInOffset;
    public float fadeOutOffset;

    //淡出方式
    public TextBoxFadeOutMethod textBoxFadeOutMethod;
    //淡入完成后，自动淡出延迟时间
    public float fadeOutDelayTime = 1;
}

//对话框，文本会有个背景。目前只有左对齐的文本能显示文本打出效果，居中的只能立即显示
public class TextBox : MonoBehaviour
{
    public Text text;
    public Transform textBGParent;

    IEnumerator setTextCor;

    //文本背景块列表
    List<GameObject> textBGList;

    CanvasGroup canvasGroup;
    Outline textOutline;
    RectTransform rect;

    TextBoxStyle style;

    //被隐藏或者淡出结束
    Action onHide;

    //文本播放完成
    Action onShowComplete;

    #region Unity回调

    void Awake()
    {
        textBGList = new List<GameObject>();

        canvasGroup = GetComponent<CanvasGroup>();
        textOutline = text.GetComponent<Outline>();
        rect = GetComponent<RectTransform>();
    }

    #endregion

    #region 方法

    //打出显示文本
    public void ShowText(string str)
    {
        //如果没有输入样式，设置一个默认的
        if(style == null)
        {
            style = new TextBoxStyle();
        }

        //设置字体大小
        text.fontSize = style.fontSize;

        //设置颜色
        text.color = style.textColor;
        textOutline.effectColor = style.textColor;

        //如果是点击淡出的样式，注册点击事件
        if (style.textBoxFadeOutMethod == TextBoxFadeOutMethod.OnClick)
        {
            GameManager.onClick += () =>
            {
                HideText();
            };
        }
        else if(style.textBoxFadeOutMethod == TextBoxFadeOutMethod.DelayTime)
        {
            onShowComplete += () =>
            {
                StartCoroutine(DelayHideCor(style.fadeOutDelayTime));

                onShowComplete = null;
            };
        }

        //如果淡入
        if(style.fadeIn)
        {
            FadeIn();
        }

        setTextCor = SetTextCor(str);
        StartCoroutine(setTextCor);
    }
    IEnumerator SetTextCor(string str)
    {
        text.text = "";

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

            //这行文本的尺寸
            Vector2 lineSize = GetStringSizeInText(lineStr, text);

            //如果是居中
            if (style.alignment == TextBoxAlignment.Center)
            {
                //如果是居中，添加空格
                int spaceCount = Mathf.CeilToInt((size.x - lineSize.x) / 2 / GetCharSizeInText(' ', text).x);
                for (int i = 0; i < spaceCount; i++)
                {
                    text.text += " ";
                }

                //文本背景偏移
                float bgOffsetX = GetCharSizeInText(' ', text).x * spaceCount - Mathf.CeilToInt((size.x - lineSize.x) / 2);

                //获取这行文本的左上角坐标
                startingPos.x = -lineSize.x / 2;
                startingPos.x += bgOffsetX;
            }
            else
            {
                //获取这行文本的左上角坐标
                startingPos.x = -size.x / 2;
            }

            //如果是瞬间显示
            if (style.playSpeedPerChar == 0)
            {
                text.text += lineStr;

                //生成文本背景
                GenerateLineTextBG(startingPos, lineStr);
            }
            else
            {
                //慢慢打出
                Vector2 firstCharSize = GetCharSizeInText(lineStr[0], text);

                //创建文本背景
                Vector2 lineStartingPos = startingPos;
                lineStartingPos.y -= firstCharSize.y / 2;

                foreach (var ch in lineStr)
                {
                    text.text += ch;

                    Vector2 charSize = GetCharSizeInText(ch, text);
                    lineStartingPos.x += charSize.x;
                    Vector2 bgPos = new Vector2(lineStartingPos.x - charSize.x / 2, lineStartingPos.y);

                    //生成文本背景
                    GenerateTextBG(bgPos, charSize);

                    yield return new WaitForSeconds(style.playSpeedPerChar);
                }
            }

            startingPos.y -= lineSize.y * text.lineSpacing;
        }

        //播放完成
        onShowComplete?.Invoke();
    }

    //隐藏文本
    public void HideText(Action onComplete = null)
    {
        //隐藏完成后进行的动作
        Action action = () =>
        {
            onComplete?.Invoke();

            ClearText();

            onHide?.Invoke();
        };

        if (style.fadeOut)
        {
            FadeOut(() =>
            {
                action();
            });
        }
        else
        {
            action();
        }
    }

    //停止显示文本
    public void StopShowText()
    {
        if (setTextCor != null)
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

    //立即完成当前文本的显示
    public void ShowCurrentTextImmediately()
    {
        style.playSpeedPerChar = 0;
    }

    IEnumerator moveToGameObjectCor;

    //附着到游戏物体上
    public void AttachToGameObject(GameObject target)
    {
        moveToGameObjectCor = MoveToGameObject(target);
        StartCoroutine(moveToGameObjectCor);
    }
    //一直移动到游戏物体位置
    IEnumerator MoveToGameObject(GameObject target)
    {
        while (target != null)
        {
            Vector2 pos = Camera.main.WorldToViewportPoint (target.transform.position);

            GetComponent<RectTransform>().anchoredPosition = pos;

            yield return new WaitForFixedUpdate();
        }
    }
    public void StopAttachToGameObject()
    {
        StopCoroutine(moveToGameObjectCor);
    }

    public void SetPos(Vector2 pos)
    {
        transform.position = pos;
    }

    public void SetStyle(TextBoxStyle style)
    {
        this.style = style;
    }

    public void AddOnHideCallback(Action onHide)
    {
        this.onHide = () =>
        {
            onHide?.Invoke();

            onHide = null;
        };
    }

    IEnumerator DelayHideCor(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        HideText();
    }

    #region 生成文字背景块

    //生成一行文本的背景，初始位置为文本左上角
    void GenerateLineTextBG(Vector2 startingPos, string str)
    {
        Vector2 firstCharSize = GetCharSizeInText(str[0], text);

        startingPos.y -= firstCharSize.y / 2;

        for (int i = 0; i < str.Length; i++)
        {
            Vector2 charSize = GetCharSizeInText(str[i], text);

            startingPos.x += charSize.x;

            Vector2 bgPos = new Vector2(startingPos.x - charSize.x / 2, startingPos.y);
            GenerateTextBG(bgPos, charSize);
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

        //设置背景块颜色
        textBG.GetComponentInChildren<Image>().color = style.textBGColor;
    }

    #endregion

    #region 渐变

    void FadeIn()
    {
        canvasGroup.DOKill();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, style.fadeInDuration);

        textOutline.DOKill();
        Color outlineColor = textOutline.effectColor;
        outlineColor.a = 0;
        textOutline.effectColor = outlineColor;
        textOutline.DOFade(1, style.fadeInDuration);

        //位移
        if (style.fadeInOffset != default)
        {
            float originY = rect.anchoredPosition.y;
            rect.DOKill();

            //设置初始位置
            Vector2 pos = rect.anchoredPosition;
            pos.y = rect.anchoredPosition.y - style.fontSize * style.fadeInOffset;
            rect.anchoredPosition = pos;

            rect.DOAnchorPosY(originY, style.fadeInDuration);
        }
    }

    void FadeOut(Action onComplete = null)
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(1, 0);
        canvasGroup.DOFade(0, style.fadeInDuration).OnComplete(() =>
        {
            onComplete?.Invoke();
        });

        textOutline.DOKill();
        textOutline.DOFade(1, 0);
        textOutline.DOFade(0, style.fadeInDuration);

        //位移
        if (style.fadeInOffset != default)
        {
            rect.DOKill();
            rect.DOAnchorPosY(rect.anchoredPosition.y + style.fontSize * style.fadeInOffset, style.fadeOutDuration);
        }
    }

    #endregion

    #endregion

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
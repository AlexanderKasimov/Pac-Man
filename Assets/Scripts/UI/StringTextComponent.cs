using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringTextComponent : MonoBehaviour
{
    public StringVariable variable;
    public Color textColor;    
    Text textComponent;

    // Use this for initialization
    void Start()
    {
        textComponent = GetComponent<Text>();
        textComponent.text = "<color=#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + variable.runtimeValue + " </color>";
    }

    // Update is called once per frame
    void Update()
    {
        textComponent.text = "<color=#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + variable.runtimeValue + " </color>";
    }
}

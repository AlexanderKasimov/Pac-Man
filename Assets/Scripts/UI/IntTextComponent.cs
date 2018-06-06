using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntTextComponent : MonoBehaviour
{

    public UintVariable variable;
    public Color textColor;
    public string parameterName;
    Text textComponent;

    // Use this for initialization
    void Start()
    {
        textComponent = GetComponent<Text>();
        textComponent.text = "<color=#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + parameterName + ": " + variable.runtimeValue.ToString() + " </color>";
    }

    // Update is called once per frame
    void Update()
    {
        textComponent.text = "<color=#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + parameterName + ": " + variable.runtimeValue.ToString() + " </color>";
    }
}

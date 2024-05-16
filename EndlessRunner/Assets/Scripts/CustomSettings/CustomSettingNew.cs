using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomSettingNew : MonoBehaviour
{
    public TMP_InputField[] _speedFields;
    public TMP_InputField _repetitionField;
    public TMP_InputField _maxObstacleField;
    public TMP_InputField _noiseField;

    public void Start()
    {
        foreach(TMP_InputField speedField in _speedFields)
        {
            speedField.onValueChanged.AddListener(delegate { ValidateIntegers(speedField, 1, 35); });
        }
        _repetitionField.onValueChanged.AddListener(delegate { ValidateIntegers(_repetitionField, 1, 99); });
        _maxObstacleField.onValueChanged.AddListener(delegate { ValidateIntegers(_maxObstacleField, 0, 999); });
        _noiseField.onEndEdit.AddListener(delegate { ValidateFloat(_noiseField, 0f, 100f); });
    }

    private void ValidateIntegers(TMP_InputField inputField, int min, int max)
    {
        int parsedInt = 0;
        if (int.TryParse(inputField.text, out parsedInt))
        {
            parsedInt = Mathf.Clamp(parsedInt, min, max);
            inputField.text = parsedInt.ToString();
        }
        else
        {
            inputField.text = "";
        }
    }

    private void ValidateFloat(TMP_InputField inputField, float min, float max)
    {
        double parsedFloat = 0f;
        if (double.TryParse(inputField.text, out parsedFloat))
        {
            parsedFloat = System.Math.Round(Mathf.Clamp((float)parsedFloat, min, max), 2);
            inputField.text = parsedFloat.ToString();
        }
        else
        {
            inputField.text = "";
        }
    }

    public void PlusOneToText(TMP_InputField field)
    {
        float parsedFloat = 0f;
        int parsedInt = 0;
        if (float.TryParse(field.text, out parsedFloat))
        {
            parsedFloat += 1f;
            field.text = parsedFloat.ToString();
        }
        else if (int.TryParse(field.text, out parsedInt))
        {
            parsedInt += 1;
            field.text = parsedInt.ToString();
        }
    }

    public void MinusOneToText(TMP_InputField field)
    {
        float parsedFloat = 0f;
        int parsedInt = 0;
        if (float.TryParse(field.text, out parsedFloat))
        {
            parsedFloat -= 1f;
            field.text = parsedFloat.ToString();
        }
        else if (int.TryParse(field.text, out parsedInt))
        {
            parsedInt -= 1;
            field.text = parsedInt.ToString();
        }
    }
}

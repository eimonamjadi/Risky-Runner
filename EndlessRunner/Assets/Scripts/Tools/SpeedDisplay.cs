using UnityEngine;
using UnityEngine.UI;

public class SpeedDisplay : MMSingleton<SpeedDisplay>
{
    Text textField;

    protected override void Awake()
    {
        base.Awake();
        textField = GetComponent<Text>();
    }

    public void UpdateDisplayValue(float val)
    {
        textField.text = val.ToString("00 mi/h");
    }
}

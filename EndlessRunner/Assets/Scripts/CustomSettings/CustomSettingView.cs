using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomSettingView : MonoBehaviour
{
    protected CustomSetting customSetting;
    public CustomSettingData GetCustomData { get { return customSetting.Data; } }
    [SerializeField] protected TMP_Text[] _speedFields;
    [SerializeField] protected TMP_Text _repetitionField;
    [SerializeField] protected TMP_Text _maxObstacleField;
    [SerializeField] protected TMP_Text _noiseField;
    public int ID = 0;
    public GameObject Rim;


    public void UpdateView(in CustomSetting newSetting)
    {
        if (newSetting.Data.IsAssigned)
        {
            gameObject.SetActive(true);
            customSetting = newSetting;
            // populate speeds
            int count = CustomSetting.speedCount;
            for (int i=0; i< count; i++)
            {
                _speedFields[i].text = customSetting.Data.Speeds[i].ToString();
            }
            // populate other fields
            _repetitionField.text = customSetting.Data.NumRepetitions.ToString();
            _maxObstacleField.text = customSetting.Data.MaxObstacles.ToString();
            _noiseField.text = customSetting.Data.NoiseLevel.ToString("F2") + "%";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void HightLightOn()
    {
        if (Rim) Rim.SetActive(true);
    }

    public void HightLightOff()
    {
        if (Rim) Rim.SetActive(false);
    }
}

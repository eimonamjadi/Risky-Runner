using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSettingManager : MMSingleton<CustomSettingManager>
{
    public List<CustomSetting> customSettings;
    public int _highLightedIndex = 0;
    public bool SettingSelected
    {
        get { return _highLightedIndex != -1; }
    }
    [Tooltip("Index 0 is the Default setting")]
    protected CustomSettingView[] _views;
    protected const int _settingsCount = 5;
    protected int _availableIndex = 0;
    public bool CanCreateNew
    {
        get
        {
            return _availableIndex < _settingsCount && _availableIndex >= 0;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Initialization();
    }

    public void Initialization()
    {
        _views = new CustomSettingView[_settingsCount];
        CustomSettingView[] viewsInTheScene = FindObjectsOfType<CustomSettingView>();
        foreach(CustomSettingView view in viewsInTheScene)
        {
            view.ID = Mathf.Clamp(view.ID, 0, _settingsCount - 1);
            _views[view.ID] = view;
        }
        LoadAllSettings();
        ResetHighLight();
    }

    protected void UpdateSettingsView()
    {
        _availableIndex = 0;
        for (int i=0; i<_views.Length; i++)
        {
            _views[i].UpdateView(customSettings[i]);
            if (customSettings[i].Data != null && customSettings[i].Data.IsAssigned) _availableIndex++;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="newSetting"></param>
    /// <returns>Successfully Created</returns>
    public bool AddNewCustomSetting(CustomSettingNew newSetting)
    {
        if (CanCreateNew)
        {
            int parseInt = 0;
            float parseFloat = 0f;
            string tempString = "";
            bool bParseSuccess = true;
            customSettings[_availableIndex].Data.Speeds = new int[CustomSetting.speedCount];
            for (int i=0; i<CustomSetting.speedCount; i++)
            {
                tempString = newSetting._speedFields[i].text;
                if (int.TryParse(tempString, out parseInt))
                {
                    customSettings[_availableIndex].Data.Speeds[i] = parseInt;
                }
                else bParseSuccess = false;
            }
           
            tempString = newSetting._noiseField.text;
            if (bParseSuccess && float.TryParse(tempString, out parseFloat))
            {
                customSettings[_availableIndex].Data.NoiseLevel = parseFloat;
            }
            else bParseSuccess = false;

            tempString = newSetting._maxObstacleField.text;
            if (bParseSuccess && int.TryParse(tempString, out parseInt))
            {
                customSettings[_availableIndex].Data.MaxObstacles = parseInt;
            }
            else bParseSuccess = false;

            tempString = newSetting._repetitionField.text;
            if (bParseSuccess && int.TryParse(tempString, out parseInt))
            {
                customSettings[_availableIndex].Data.NumRepetitions = parseInt;
            }
            else bParseSuccess = false;

            if (bParseSuccess)
            {
                customSettings[_availableIndex].Data.IsAssigned = true;
                SetHightLight(_availableIndex);
                UpdateSettingsView();
                
                SaveSettings();
            }
            else
            {
                /// restore the values maybe
            }

            return bParseSuccess;
        }
        return false;
    }

    public CustomSettingData GetSettingData()
    {
        if (SettingSelected) return _views[_highLightedIndex].GetCustomData;
        return null;
    }

    public CustomSettingData GetDefaultData()
    {
        if (_views.Length > 0) return _views[0].GetCustomData;
        return null;
    }

    public void SetHightLight(CustomSettingView view)
    {
        if (_highLightedIndex == view.ID)
        {
            view.HightLightOff();
            _highLightedIndex = -1;
        }
        else
        {
            if (_highLightedIndex != -1) _views[_highLightedIndex].HightLightOff();
            _highLightedIndex = view.ID;
            _views[_highLightedIndex].HightLightOn();
        }
    }

    public void SetHightLight(int index)
    {
        if (index >= 0 && index < _settingsCount)
        {
            if (_highLightedIndex != -1) _views[_highLightedIndex].HightLightOff();
            _highLightedIndex = index;
            _views[_highLightedIndex].HightLightOn();
        }
    }

    public void ResetHighLight()
    {
        _highLightedIndex = 0;
        foreach(CustomSettingView view in _views)
        {
            if (view.ID == _highLightedIndex)
            {
                view.HightLightOn();
            }
            else
            {
                view.HightLightOff();
            }
        }
    }

    public void DeleteCustomSetting(int id)
    {
        /// the default one can't be deleted
        if (id != 0 && id < _settingsCount)
        {
            customSettings[id].Data = new CustomSettingData(false);
            SaveSystem.Save(customSettings[id].Data, customSettings[id].name);
        }
        UpdateSettingsView();
        ResetHighLight();
    }

#region SAVE_LOAD
    /// <summary>
    /// Save All Five Of the Settings
    /// </summary>
    public void SaveSettings()
    {
        foreach(CustomSetting setting in customSettings)
        {
            SaveSystem.Save(setting.Data, setting.name);
        }
    }

    public void LoadAllSettings()
    {
        foreach(CustomSetting setting in customSettings)
        {
            CustomSettingData data = SaveSystem.Load<CustomSettingData>(setting.name);
            if (data != default) setting.Data = data;
        }
        UpdateSettingsView();
    }

    public void LoadSettingsAtIndex(int index)
    {
        if (index >= 0 && index < _settingsCount)
        {
            CustomSettingData data = SaveSystem.Load<CustomSettingData>(customSettings[index].name);
            UpdateSettingsView();
        }
    }

#endregion

}

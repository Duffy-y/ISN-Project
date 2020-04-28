﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Rendering.HighDefinition;

[System.Serializable]
public class KeyCodeSetting
{
    public string settingKey;
    public KeyCode key;
}

[System.Serializable]
public class BooleanSetting
{
    public string settingKey;
    public bool state;
}

[System.Serializable]
public class FloatSetting
{
    public string settingKey;
    public float value;
}


public class Settings : MonoBehaviour
{
    public static Settings Instance;

    [SerializeField] private KeyCodeSetting[] m_defaultKeybind;
    [SerializeField] private BooleanSetting[] m_defaultSetting;
    [SerializeField] private FloatSetting[] m_defaultFloat;

    private Dictionary<string, dynamic> m_settings;
    private Dictionary<string, KeyCode> m_keybinds;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
        LoadSettings();
    }

    public void ApplySettings()
    {
        SaveSettings();
    }

    private void SaveSettings()
    {
        string settingJson = JsonConvert.SerializeObject(m_settings);
        string keybindJson = JsonConvert.SerializeObject(m_keybinds);

        GameFile.Instance.WriteFile("keybinds.json", keybindJson);
        GameFile.Instance.WriteFile("settings.json", settingJson);
    }

    private void LoadSettings()
    {
        string settingJson = GameFile.Instance.ReadFile("settings.json");
        string keybindJson = GameFile.Instance.ReadFile("keybinds.json");

        m_settings = settingJson == null ? null : JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(settingJson);
        m_keybinds = keybindJson == null ? null : JsonConvert.DeserializeObject<Dictionary<string, KeyCode>>(keybindJson);

        if (m_settings == null)
        {
            Debug.Log("m_settings");
            m_settings = new Dictionary<string, dynamic>();
            foreach (var item in m_defaultSetting)
            {
                m_settings.Add(item.settingKey, item.state);
            }

            foreach (var item in m_defaultFloat)
            {
                m_settings.Add(item.settingKey, item.value);
            }
        }
        if (m_keybinds == null)
        {
            Debug.Log("m_keybinds");
            m_keybinds = new Dictionary<string, KeyCode>();
            foreach (var item in m_defaultKeybind)
            {
                m_keybinds.Add(item.settingKey, item.key);
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    #region Get/Set|Methods
    public void SetSettings<T>(string key, T value)
    {
        Debug.Log("trying to set : " + key + "| Setting : " + value.ToString());
        if (m_settings.ContainsKey(key))
        {
            m_settings[key] = value;
        }
        else
        {
            m_settings.Add(key, value);
        }
    }

    public dynamic GetSettings(string key)
    {
        Debug.Log("trying to access : " + key + "| Returning : " + m_settings[key]);
        if (m_settings.ContainsKey(key))
        {
            return m_settings[key];
        }
        else
        {
            return null;
        }
    }

    public void SetKey(string key, KeyCode keyCode)
    {
        if (m_keybinds.ContainsKey(key))
        {
            m_keybinds[key] = keyCode;
        }
        else
        {
            m_keybinds.Add(key, keyCode);
        }
    }

    public KeyCode GetKey(string key)
    {
        try
        {
            return m_keybinds[key];
        }
        catch
        {
            return KeyCode.None;
        }
    }
    #endregion
}

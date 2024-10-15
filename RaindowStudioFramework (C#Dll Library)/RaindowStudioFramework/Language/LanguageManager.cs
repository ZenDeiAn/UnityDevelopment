using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaindowStudio.Language
{
    public class LanguageManager : MonoBehaviour
    {
        public int dataSetIndex = 0;
        public string usingDataSet = string.Empty;
        public LanguageType languagePreview = LanguageType.KO;
        public List<LanguageComponentObjects> languageObjects = new List<LanguageComponentObjects>();

        public static LanguageType language = LanguageType.KO;
        // 1. using data set(ScriptableObject name)
        // 2. LanguageType
        // 3. Language data key
        // 4. Language data
        public static Dictionary<string, Dictionary<LanguageType, Dictionary<string, object>>> languageDatas = new Dictionary<string, Dictionary<LanguageType, Dictionary<string, object>>>();
        public static Dictionary<string, string[]> dataSets = new Dictionary<string, string[]>();
        public static event Action<LanguageType> LanguageChangedEvent;

        public static object GetLanguageData(string dataSet, string key)
        {
            return languageDatas[dataSet][language][key];
        }

        public static object GetLanguageData(string dataSet, LanguageType type, string key)
        {
            return languageDatas[dataSet][type][key];
        }

        public static void ChangeLanguage(LanguageType language)
        {
            if (language != LanguageManager.language)
            {
                LanguageManager.language = language;
                LanguageChangedEvent?.Invoke(language);
            }
        }

        public void ApplyLanguageModifacation(LanguageType language)
        {
            for (int i = 0; i < languageObjects.Count; ++i)
            {
                object languageObject = languageDatas[usingDataSet][language][languageObjects[i].key];
                List<GameObject> gos = languageObjects[i].componentObjects;

                for (int j = 0; j < gos.Count; ++j)
                {
                    GameObject go = gos[j];
                    // Text
                    if (go.TryGetComponent(out Text txt))
                    {
                        txt.text = languageObject as string;
                    }
                    // TextMeshPro
                    else if (go.TryGetComponent(out TextMeshProUGUI tmp))
                    {
                        tmp.SetText(languageObject as string);
                    }
                    // Image
                    else if (go.TryGetComponent(out Image img))
                    {
                        img.sprite = languageObject as Sprite;
                    }
                    // Sprite Renderer
                    else if (go.TryGetComponent(out SpriteRenderer sr))
                    {
                        sr.sprite = languageObject as Sprite;
                    }
                }
            }
        }

        public static void ReloadResourceData()
        {
            dataSets.Clear();
            languageDatas.Clear();
            Array ltArray = Enum.GetValues(typeof(LanguageType));
            LanguageDataObject[] datasArray = Resources.LoadAll<LanguageDataObject>("Language");
            for (int m = 0; m < datasArray.Length; ++m)
            {
                LanguageDataObject datas = datasArray[m];
                string[] keys = new string[0];
                languageDatas[datas.name] = new Dictionary<LanguageType, Dictionary<string, object>>();
                Dictionary<LanguageType, Dictionary<string, object>> current = languageDatas[datas.name];
                for (int i = 0; i < datas.languageDatas.Count; ++i)
                {
                    LanguageData data = datas.languageDatas[i];
                    Array.Resize(ref keys, keys.Length + 1);
                    keys[keys.Length - 1] = data.key;
                    for (int j = 0; j < ltArray.Length; ++j)
                    {
                        LanguageType lt = (LanguageType)ltArray.GetValue(j);
                        if (!current.ContainsKey(lt))
                            current.Add(lt, new Dictionary<string, object>());
                        current[lt].Add(data.key, data.List[(int)lt]);
                    }
                }
                dataSets[datas.name] = keys;
            }
        }

        void Awake()
        {
            if (languageDatas.Count == 0)
            {
                ReloadResourceData();
            }
            LanguageChangedEvent += (t) => ApplyLanguageModifacation(t);
            //CheckNLoadData();
            ApplyLanguageModifacation(language);
        }
    }
}

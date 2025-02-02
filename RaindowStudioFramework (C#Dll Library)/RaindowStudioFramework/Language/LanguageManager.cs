using RaindowStudio.DesignPattern;
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
        public static Dictionary<string, Dictionary<LanguageType, Dictionary<string, object>>> languageDataDic = new Dictionary<string, Dictionary<LanguageType, Dictionary<string, object>>>();
        public static Dictionary<string, Dictionary<LanguageType, TMP_FontAsset>> defaultFontData = new Dictionary<string, Dictionary<LanguageType, TMP_FontAsset>>();
        public static Dictionary<string, string[]> dataSets = new Dictionary<string, string[]>();
        public static event Action<LanguageType> LanguageChangedEvent;

        public static object GetLanguageData(string dataSet, string key)
        {
            return languageDataDic[dataSet][language][key];
        }

        public static object GetLanguageData(string dataSet, LanguageType type, string key)
        {
            return languageDataDic[dataSet][type][key];
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
                if (!(languageDataDic.ContainsKey(usingDataSet) &&
                    languageDataDic[usingDataSet].ContainsKey(language) &&
                    languageDataDic[usingDataSet][language].ContainsKey(languageObjects[i].key)))
                    return;

                object languageObject = languageDataDic[usingDataSet][language][languageObjects[i].key];
                List<GameObject> gos = languageObjects[i].componentObjects;

                for (int j = 0; j < gos.Count; ++j)
                {
                    GameObject go = gos[j];
                    // Text
                    if (go.TryGetComponent(out Text txt))
                    {
                        txt.text = (languageObject as LanguageTextData).text;
                    }
                    // TextMeshPro
                    else if (go.TryGetComponent(out TextMeshProUGUI tmp))
                    {
                        LanguageTextData data = languageObject as LanguageTextData;
                        if (data.font != null)
                        {
                            tmp.font = data.font;
                            tmp.UpdateFontAsset();
                        }
                        else if (defaultFontData[usingDataSet].TryGetValue(language, out TMP_FontAsset font))
                        {
                            tmp.font = font;
                            tmp.UpdateFontAsset();
                        }
                        tmp.SetText(data.text);
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

        public static void ReloadResourceData(LanguageDataObject[] languageData = null)
        {
            dataSets.Clear();
            languageDataDic.Clear();
            Array ltArray = Enum.GetValues(typeof(LanguageType));
            LanguageDataObject[] dataArray =
                languageData == null ?
                Resources.LoadAll<LanguageDataObject>("Language") :
                languageData;
            for (int m = 0; m < dataArray.Length; ++m)
            {
                LanguageDataObject datas = dataArray[m];
                string[] keys = new string[0];
                languageDataDic[datas.name] = new Dictionary<LanguageType, Dictionary<string, object>>();
                Dictionary<LanguageType, Dictionary<string, object>> current = languageDataDic[datas.name];
                for (int i = 0; i < datas.languageData.Count; ++i)
                {
                    LanguageData data = datas.languageData[i];
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

                defaultFontData[datas.name] = new Dictionary<LanguageType, TMP_FontAsset>();
                for (int i = 0; i < datas.defaultFont.Count; ++i)
                {
                    defaultFontData[datas.name][(LanguageType)ltArray.GetValue(i)] = datas.defaultFont[i];
                }
            }
        }

        void Awake()
        {
            if (languageDataDic.Count == 0)
            {
                ReloadResourceData();
            }
            LanguageChangedEvent += (t) => ApplyLanguageModifacation(t);
            //CheckNLoadData();
            ApplyLanguageModifacation(language);
        }
    }
}

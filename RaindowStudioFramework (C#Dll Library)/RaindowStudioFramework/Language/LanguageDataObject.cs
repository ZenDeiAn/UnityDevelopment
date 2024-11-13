using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RaindowStudio.Language
{
    [CreateAssetMenu(fileName = "LanguageData", menuName = "LanguageData", order = 1)]
    public class LanguageDataObject : ScriptableObject
    {
        public List<TMP_FontAsset> defaultFont = new List<TMP_FontAsset>();
        public List<LanguageData> languageDatas = new List<LanguageData>();
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace RaindowStudio.Language
{
    [CreateAssetMenu(fileName = "LanguageData", menuName = "LanguageData", order = 1)]
    public class LanguageDataObject : ScriptableObject
    {
        public List<LanguageData> languageDatas = new List<LanguageData>();
    }
}

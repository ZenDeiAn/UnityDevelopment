using RaindowStudio.DesignPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace RaindowStudio.Language
{
    [Serializable]
    public class LanguageData
    {
        public string key = "";
        public LanguageDataType dataType = LanguageDataType.Text;
        public List<LanguageTextData> textData = new List<LanguageTextData>();
        public List<Sprite> spriteData = new List<Sprite>();

        public List<object> List
        {
            get
            {
                switch (dataType)
                {
                    case LanguageDataType.Text:
                        return textData.ToList<object>();
                    case LanguageDataType.Sprite:
                        return spriteData.ToList<object>();
                    default: return null;
                }
            }
        }
    }
}
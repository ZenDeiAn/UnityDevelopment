using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RaindowStudio.Language
{
    [Serializable]
    public class LanguageData
    {
        public string key = "";
        public LanguageDataType dataType = LanguageDataType.Text;
        public List<string> textData = new List<string>();
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
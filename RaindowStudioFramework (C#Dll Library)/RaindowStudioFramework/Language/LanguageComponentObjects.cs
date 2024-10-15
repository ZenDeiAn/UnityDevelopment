using System.Collections.Generic;
using System;
using UnityEngine;

namespace RaindowStudio.Language
{
    [Serializable]
    public class LanguageComponentObjects
    {
        public int indexOfKey;
        public string key;
        public List<GameObject> componentObjects = new List<GameObject>();
    }
}
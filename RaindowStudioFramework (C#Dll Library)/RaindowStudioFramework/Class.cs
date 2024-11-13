using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace RaindowStudio.DesignPattern
{
    public class KeyOperator<T>
    {
        private Dictionary<T, Action> actions = new Dictionary<T, Action>();

        public List<T> Keys => actions.Keys.ToList();

        public void Register(T _enum, Action _action)
        {
            if (actions.ContainsKey(_enum))
            {
                actions[_enum] = _action;
            }
            else
            {
                actions.Add(_enum, _action);
            }
        }

        public void Operate(T _enum)
        {
            if (actions.ContainsKey(_enum))
            {
                actions[_enum]?.Invoke();
            }
        }
    }
}

namespace RaindowStudio.Language
{
    [Serializable]
    public class LanguageTextData
    {
        public string text;
        public TMP_FontAsset font;
    }
}
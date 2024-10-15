using System;
using UnityEngine;

namespace RaindowStudio.Attribute
{
    #region UneditableField
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class UneditableFieldAttribute : PropertyAttribute
    {
        public ShowOnlyOption Option { get; set; } = ShowOnlyOption.always;
        public UneditableFieldAttribute() { }
        public UneditableFieldAttribute(ShowOnlyOption option) => Option = option;
    }
    #endregion
}
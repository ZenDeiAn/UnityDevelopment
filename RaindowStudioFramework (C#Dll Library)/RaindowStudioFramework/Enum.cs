using System;

namespace RaindowStudio.DesignPattern
{
    public enum ProcessorStateTriggerType
    {
        Activate,
        Deactivate,
        Update,
        FixedUpdate,
        LateUpdate
    }
}

namespace RaindowStudio.Attribute
{
    public enum ShowOnlyOption
    {
        always,
        editMode,
        playMode
    }
}

namespace RaindowStudio.AudioManager
{
    // Keep master, bgm, sfx exsit cause AudioManager class is referencing it.
    public enum AudioVolumeType
    {
        MASTER,
        BGM,
        SFX,
    }
}

namespace RaindowStudio.Language
{
    [Serializable]
    public enum LanguageType
    {
        KO,
        EN,
        CN,
        JA,
        TW,
        ES,
        FR,
        DE,
        RU,
        IT,
        PT,
        NL
    }

    [Serializable]
    public enum LanguageDataType
    {
        Text,
        Sprite
    }
}
using System;
using UnityEngine;
using RaindowStudio.DesignPattern;
using System.Collections.Generic;
using RaindowStudio.Attribute;
using System.IO;
using System.Linq;
using RaindowStudio.Utility;

namespace RaindowStudio.AudioManager
{
    public abstract class AudioManagerBase : SingletonUnityEternal<AudioManagerBase> 
    {
        private const string DEFINED_ENUM_NAME = "AudioClipKey";

        [UneditableField] public List<AudioClip> audioClips = new List<AudioClip>();

        public event Action<AudioVolumeType> OnVolumeChangedEvent;

        private Dictionary<AudioVolumeType, float> _volumes = new Dictionary<AudioVolumeType, float>();

        private GameObject _audioObject;

        private ObjectPool _OP_BGM { get; set; }
        private ObjectPool _OP_SFX { get; set; }

        public float GetVolume(AudioVolumeType type)
        {
            return _volumes[type];
        }

        public void SetVolume(AudioVolumeType type, float value)
        {
            _volumes[type] = value;
            OnVolumeChangedEvent?.Invoke(type);
        }

        public AudioObject PlaySFX(AudioClip audioClip)
        {
            AudioObject ao = _OP_SFX.GetObject().GetComponent<AudioObject>();

            ao.PlayAudio(audioClip, AudioVolumeType.SFX, false);

            return ao;
        }

        public AudioObject PlayBGM(AudioClip audioClip, float fadeTime = 0)
        {
            AudioObject ao = _OP_BGM.GetObject(audioClip.name).GetComponent<AudioObject>();

            if (fadeTime == 0)
            { 
                ao.PlayAudio(audioClip, AudioVolumeType.BGM, true);
            }
            else
            {
                ao.PlayAudioFade(audioClip, AudioVolumeType.BGM, true, fadeTime);
            }

            return ao;
        }

        public AudioObject StopBGM(AudioClip audioClip, float fadeTime = 0)
        {
            string key = audioClip.name;

            if (!_OP_BGM.ContainsActiveObject(key))
                return null;

            AudioObject ao = _OP_BGM.GetObject(key).GetComponent<AudioObject>();

            if (fadeTime == 0)
            {
                ao.StopAudio();
            }
            else
            {
                ao.StopAudioFade(fadeTime);
            }

            return ao;
        }

        public AudioObject PauseBGM(AudioClip audioClip, float fadeTime)
        {
            string key = audioClip.name;

            if (!_OP_BGM.ContainsActiveObject(key))
                return null;

            AudioObject ao = _OP_BGM.GetObject(key).GetComponent<AudioObject>();

            if (fadeTime == 0)
            {
                ao.PauseAudio();
            }
            else
            {
                ao.PauseAudioFade(fadeTime);
            }

            return ao;
        }

        public AudioObject ResumeBGM(AudioClip audioClip, float fadeTime)
        {
            string key = audioClip.name;

            if (!_OP_BGM.ContainsActiveObject(key))
                return null;

            AudioObject ao = _OP_BGM.GetObject(key).GetComponent<AudioObject>();

            if (fadeTime == 0)
            { 
                ao.ResumeAudio();
            }
            else
            {
                ao.ResumeAudioFade(fadeTime);
            }

            return ao;
        }

        public void StopAllBGM(float fadeTime = 0)
        {
            foreach (var po in _OP_BGM.ActivedObjects)
            {
                if (fadeTime == 0)
                {
                    po.GetComponent<AudioObject>().StopAudio();
                }
                else
                {
                    po.GetComponent<AudioObject>().StopAudioFade(fadeTime);
                }
            }
        }


        public void ReloadAudioClips()
        {
            audioClips = Resources.LoadAll<AudioClip>("AudioClip").ToList();
            Utility.Utility.UpdateEnumInDynamicCodeScript(
                DEFINED_ENUM_NAME,
                audioClips.Select(clip => clip.name).ToArray(),
                d => audioClips = audioClips.OrderBy(c => d[c.name]).ToList());
        }

        private ObjectPool InitializeObjectPool(string name)
        {
            ObjectPool op = new GameObject(name).AddComponent<ObjectPool>();

            op.poolingObject = _audioObject;
            op.transform.SetParent(transform);

            return op;
        }

        protected override void Initialization()
        {
            base.Initialization();

            _audioObject = new GameObject("AudioObject").AddComponent<AudioObject>().gameObject;
            _audioObject.transform.SetParent(transform);

            foreach (AudioVolumeType type in Enum.GetValues(typeof(AudioVolumeType)))
            {
                _volumes.Add(type, 1.0f);
            }

            // Create ObjectPools for AudioSource.
            _OP_BGM = InitializeObjectPool("OP_BGM");
            _OP_SFX = InitializeObjectPool("OP_SFX");
        }
    }
}

using UnityEngine;
using RaindowStudio.DesignPattern;
using RaindowStudio.Utility;

namespace RaindowStudio.AudioManager
{
    public class AudioObject : PoolObject
    {
        public AudioSource audioSource;

        private AudioVolumeType volumeType;
        private Coroutine coroutine;

        public AudioSource PlayAudio(AudioClip audioClip, AudioVolumeType volumeType, bool loop)
        {
            if (audioSource == null && !TryGetComponent(out audioSource))
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }

            this.volumeType = volumeType;

            audioSource.clip = audioClip;
            audioSource.volume = AudioManagerBase.Instance.GetVolume(volumeType);
            audioSource.loop = loop;

            AudioManagerBase.Instance.OnVolumeChangedEvent += VolumeChanged;

            if (loop == false)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                coroutine = this.WaitUntilToDo(() => audioSource.time >= audioSource.clip.length, () => StopAudio());
            }

            audioSource.Play();

            return audioSource;
        }

        public AudioSource PlayAudioFade(AudioClip audioClip, AudioVolumeType volumeType, bool loop, float fadeTime)
        {
            PlayAudio(audioClip, volumeType, loop);

            float targetVolume = AudioManagerBase.Instance.GetVolume(volumeType);

            audioSource.volume = 0;

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = this.LoopUntil(() => audioSource.volume >= targetVolume,
                () => audioSource.volume += targetVolume * (Time.deltaTime / fadeTime),
                0,
                () => audioSource.volume = targetVolume);

            audioSource.Play();

            return audioSource;
        }

        public AudioSource StopAudio()
        {
            audioSource.Stop();

            Recycle();

            return audioSource;
        }

        public AudioSource StopAudioFade(float fadeTime)
        {
            float targetVolume = AudioManagerBase.Instance.GetVolume(volumeType);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = this.LoopUntil(() => audioSource.volume <= 0,
                () => audioSource.volume -= targetVolume * (Time.deltaTime / fadeTime),
                0,
                () =>
                {
                    audioSource.Stop();

                    Recycle();
                });

            return audioSource;
        }

        public AudioSource PauseAudio()
        {
            audioSource.Pause();

            return audioSource;
        }

        public AudioSource PauseAudioFade(float fadeTime)
        {
            float targetVolume = AudioManagerBase.Instance.GetVolume(volumeType);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = this.LoopUntil(() => audioSource.volume <= 0,
                () => audioSource.volume -= targetVolume * (Time.deltaTime / fadeTime),
                0,
                () => audioSource.Pause());

            return audioSource;
        }

        public AudioSource ResumeAudio()
        {
            audioSource.UnPause();

            return audioSource;
        }

        public AudioSource ResumeAudioFade(float fadeTime)
        {
            float targetVolume = AudioManagerBase.Instance.GetVolume(volumeType);

            audioSource.UnPause();

            audioSource.volume = 0;

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = this.LoopUntil(() => audioSource.volume >= targetVolume,
                () => audioSource.volume += targetVolume * (Time.deltaTime / fadeTime),
                0,
                () => audioSource.volume = targetVolume);

            return audioSource;
        }

        public override void Recycle()
        {
            base.Recycle();

            AudioManagerBase.Instance.OnVolumeChangedEvent -= VolumeChanged;
        }

        private void VolumeChanged(AudioVolumeType type)
        {
            audioSource.volume = AudioManagerBase.Instance.GetVolume(volumeType);
        }
    }
}

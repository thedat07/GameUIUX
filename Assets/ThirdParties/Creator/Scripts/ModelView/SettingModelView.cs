using Solo.MOST_IN_ONE;
using UnityEngine;
using DG.Tweening;
using Lean.Pool;
using System.Collections.Generic;
using System.Linq;

public class SettingModelView : MonoBehaviour, IInitializable
{
    private SettingModel m_SettingData;
    public LeanGameObjectPool audioSound;

    public SoAudio soAudio;

    [Header("DOTween")]
    public bool autoKillMode;
    public bool useSafeMode;
    public LogBehaviour logBehaviour;

    // Giữ AudioSource cho Music (loop)
    public AudioSource musicAudio;

    // Cooldown cho sound effect
    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();
    private Dictionary<TypeAudio, AudioSource> loopingSounds = new Dictionary<TypeAudio, AudioSource>();
    private float soundDelay = 0.1f; // 100ms tránh spam click

    public void Initialize()
    {
        DOTween.Init(autoKillMode, useSafeMode, logBehaviour);
        m_SettingData = GameManager.Instance.GetSettingData();
    }

    public void ToggleSound()
    {
        m_SettingData.SetSound(!m_SettingData.Sound.Value);
        if (!m_SettingData.Sound.Value)
        {
            audioSound.DespawnAll();
            soundCooldowns.Clear();
        }
    }

    public void ToggleMusic()
    {
        m_SettingData.SetMusic(!m_SettingData.Music.Value);
        if (!m_SettingData.Music.Value)
        {
            if (musicAudio != null) musicAudio.Stop();
        }
        else
        {
            PlayMusic();
        }
    }

    public void ToggleVibration()
    {
        m_SettingData.SetVibration(!m_SettingData.Vibration.Value);
    }

    public void PlaySound(AudioClip clip)
    {
        if (m_SettingData.Sound.Value && clip != null)
        {
            if (audioSound.Spawn(transform).TryGetComponent<AudioSource>(out AudioSource audio))
            {
                audio.clip = clip;
                PlayPool(ref audio);
            }
        }
    }

    public void PlaySound(TypeAudio type, float delay = 0)
    {
        if (m_SettingData.Sound.Value)
        {
            if (delay <= 0)
            {
                Spawn();
            }
            else
            {
                UnityTimer.Timer.Register(delay, Spawn, autoDestroyOwner: this);
            }
        }

        void Spawn()
        {
            if (audioSound.Spawn(transform).TryGetComponent<AudioSource>(out AudioSource audio))
            {
                if (type == TypeAudio.ButtonClick)
                {
                    audio.clip = soAudio.click;
                }
                else
                {
                    var data = soAudio.vfx.FirstOrDefault(x => x.type == type);
                    if (data != null && data.clip != null)
                        audio.clip = data.clip;
                }

                PlayPool(ref audio);
            }
        }
    }

    public void PlaySoundJump()
    {
        if (m_SettingData.Sound.Value)
        {
            TypeAudio[] typeAudios = new TypeAudio[] { TypeAudio.Char_Jump1, TypeAudio.Char_Jump2, TypeAudio.Char_Jump3, TypeAudio.Char_Jump4 };
            TypeAudio type = typeAudios[Random.Range(0, typeAudios.Length)];
            PlaySound(type);
        }
    }

    public void PlayMusic(int index = 0)
    {
        if (musicAudio != null)
        {
            if (index == 0)
                musicAudio.clip = soAudio.sf;
            else
                musicAudio.clip = soAudio.sf2;
        }

        if (!m_SettingData.Music.Value) return;

        musicAudio.Play();
    }

    public void PlaySoundClick(AudioClip clip = null)
    {
        if (clip == null)
        {
            PlaySound(soAudio.click);
        }
        else
        {
            PlaySound(clip);
        }
    }

    private void PlayPool(ref AudioSource audio)
    {
        if (audio.clip != null)
        {
            string key = audio.clip.name;

            // check cooldown
            if (soundCooldowns.TryGetValue(key, out float lastTime))
            {
                if (Time.time - lastTime < soundDelay)
                {
                    LeanPool.Despawn(audio.gameObject); // hủy audio bị spam
                    return;
                }
            }
            soundCooldowns[key] = Time.time;

            // Nếu audio đang play thì không phát lại (tránh chồng chéo khó chịu)
            if (audio.isPlaying)
            {
                LeanPool.Despawn(audio.gameObject);
                return;
            }

            audio.name = key;
            audio.Stop();
            audio.Play();

            // despawn khi phát xong
            LeanPool.Despawn(audio, audio.clip.length);
        }
    }

    public void TapSelectionHaptic()
    {
        if (!m_SettingData.Vibration.Value) return;
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.SoftImpact);
    }

    public void PlayHaptic(Most_HapticFeedback.HapticTypes type)
    {
        if (!m_SettingData.Vibration.Value) return;
        Most_HapticFeedback.Generate(type);
    }

    public void PlaySoundLoop(TypeAudio type)
    {
        if (!m_SettingData.Sound.Value) return;

        AudioClip clip = null;

        if (type == TypeAudio.ButtonClick)
        {
            clip = soAudio.click;
        }
        else
        {
            var data = soAudio.vfx.FirstOrDefault(x => x.type == type);
            if (data != null) clip = data.clip;
        }

        if (clip == null) return;

        // Nếu clip này đã loop thì bỏ qua
        if (loopingSounds.ContainsKey(type) && loopingSounds[type] != null && loopingSounds[type].isPlaying)
            return;

        if (audioSound.Spawn(transform).TryGetComponent<AudioSource>(out AudioSource audio))
        {
            audio.clip = clip;
            audio.loop = true;
            audio.name = clip.name;
            audio.volume = 0.5f;
            audio.Play();

            loopingSounds[type] = audio;
        }
    }

    public void StopSoundLoop(TypeAudio type)
    {
        if (loopingSounds.TryGetValue(type, out AudioSource audio))
        {
            if (audio != null)
            {
                audio.Stop();
                audio.loop = false;
                LeanPool.Despawn(audio.gameObject);
            }
            loopingSounds.Remove(type);
        }
    }
}

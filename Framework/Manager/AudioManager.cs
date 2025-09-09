using System.Collections;
using UnityEngine;

/// <summary>
/// 音频管理器
/// </summary>
public class AudioManager : SingletonPatternMonoAutoBase_DontDestroyOnLoad<AudioManager>
{
    //各个声道的AudioSource组件
    AudioSource bgmAudioSource;
    AudioSource bgsAudioSource;
    AudioSource voiceAudioSource;

    //各个声道的游戏对象
    GameObject bgmController;
    GameObject soundController;
    GameObject sound2D;
    GameObject sound3D;
    GameObject bgsController;
    GameObject msController;
    GameObject voiceController;

    //控制器的名字
    string bgmControllerName = "BgmController";
    string soundControllerName = "SoundController";
    string soundPool2DName = "SoundPool2D";
    string soundClip2DName = "SoundClip2D";
    string soundPool3DName = "SoundPool3D";
    string soundClip3DName = "SoundClip3D";
    string bgsControllerName = "BgsController";
    string msControllerName = "MsController";
    string msClipName = "MsClip";
    string voiceControllerName = "VoiceController";

    /// <summary>
    /// 播放BGM
    /// </summary>
    public void PlayBGM(AudioClip bgm, bool loop = true, float volume = 1)
    {
        if (bgm == null)
        {
            Debug.LogWarning("播放BGM失败！要播放的BGM为null");
            return;
        }

        bgmAudioSource.loop = loop;
        bgmAudioSource.clip = bgm;
        bgmAudioSource.volume = volume;
        bgmAudioSource.Play();
    }

    /// <summary>
    /// 暂停BGM
    /// </summary>
    public void PauseBGM()
    {
        bgmAudioSource.Pause();
    }

    /// <summary>
    /// 取消暂停BGM
    /// </summary>
    public void UnPauseBGM()
    {
        bgmAudioSource.UnPause();
    }

    /// <summary>
    /// 停止BGM
    /// </summary>
    public void StopBGM()
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = null;
    }

    /// <summary>
    /// 播放2D音效
    /// </summary>
    public void PlaySound(AudioClip sound)
    {
        if (sound == null)
        {
            Debug.LogWarning("播放Sound失败！要播放的Sound为null");
            return;
        }

        //临时的空物体，用来播放音效。
        GameObject go = null;

        for (int i = 0; i < sound2D.transform.childCount; i++)
        {
            //如果对象池中有，则从对象池中取出来用。
            if (!sound2D.transform.GetChild(i).gameObject.activeSelf)
            {
                go = sound2D.transform.GetChild(i).gameObject;
                go.SetActive(true);
                break;
            }
        }

        //如果对象池中没有，则创建一个游戏对象。
        if (go == null)
        {
            go = new GameObject(soundClip2DName);
            go.transform.SetParent(sound2D.transform);
        }

        //如果该游戏对象身上没有AudioSource组件，则添加AudioSource组件并设置参数。
        if (!go.TryGetComponent<AudioSource>(out AudioSource audioSource))
        {
            audioSource = go.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        //设置要播放的音效
        audioSource.clip = sound;

        //播放音效
        audioSource.Play();

        //每隔1秒检测一次，如果该音效播放完毕，则销毁音效的游戏对象。
        StartCoroutine(DestroyWhenFinished());

        //每隔1秒检测一次，如果该音效播放完毕，则销毁音效的游戏对象。
        IEnumerator DestroyWhenFinished()
        {
            do
            {
                yield return new WaitForSeconds(1);

                if (go == null || audioSource == null) yield break; //如果播放音频的游戏对象，或者AudioSource组件被销毁了，则直接跳出协程。
            } while (audioSource != null && audioSource.time > 0);

            if (go != null)
            {
                audioSource.clip = null;
                go.SetActive(false);
            }
        }
    }


    /// <summary>
    /// 播放3D音效
    /// </summary>
    public void PlaySound(AudioClip sound, GameObject target)
    {
        if (sound == null)
        {
            Debug.LogWarning("播放Sound失败！要播放的Sound为null");
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("播放Sound失败！无法在目标对象身上播放Sound，因为目标对象为null");
            return;
        }

        //临时的空物体，用来播放音效。
        GameObject go = null;

        for (int i = 0; i < sound3D.transform.childCount; i++)
        {
            //如果对象池中有，则从对象池中取出来用。
            if (!sound3D.transform.GetChild(i).gameObject.activeSelf)
            {
                go = sound3D.transform.GetChild(i).gameObject;
                go.SetActive(true);
                break;
            }
        }

        //如果对象池中没有，则创建一个游戏对象。
        if (go == null)
        {
            go = new GameObject(soundClip3DName);
        }

        //把用于播放音效的游戏对象放到目标物体之下，作为它的子物体
        go.transform.SetParent(target.transform);
        go.transform.localPosition = Vector3.zero;

        //如果该游戏对象身上没有AudioSource组件，则添加AudioSource组件并设置参数。
        if (!go.TryGetComponent<AudioSource>(out AudioSource audioSource))
        {
            audioSource = go.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 1f; //3D效果，近大远小。
        }

        //设置要播放的音频
        audioSource.clip = sound;

        //播放音频
        audioSource.Play();

        //每隔1秒检测一次，如果该音频播放完毕，则销毁游戏对象。
        StartCoroutine(DestoryWhenFinisied());

        //每隔1秒检测一次，如果该音频播放完毕，则销毁游戏对象。
        IEnumerator DestoryWhenFinisied()
        {
            do
            {
                yield return new WaitForSeconds(1);

                if (go == null || audioSource == null) yield break; //如果播放音频的游戏对象，或者AudioSource组件被销毁了，则直接跳出协程。
            } while (audioSource != null && audioSource.time > 0);

            if (go != null)
            {
                //放入对象池
                go.transform.SetParent(sound3D.transform);
                go.transform.localPosition = Vector3.zero;
                audioSource.clip = null;
                go.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 在世界空间中指定的位置播放3D音效
    /// </summary>
    public void PlaySound(AudioClip sound, Vector3 worldPosition, Transform parent = null)
    {
        if (sound == null)
        {
            Debug.LogWarning("播放Sound失败！要播放的Sound为null");
            return;
        }

        //临时的空物体，用来播放音效。
        GameObject go = null;

        for (int i = 0; i < sound3D.transform.childCount; i++)
        {
            //如果对象池中有，则从对象池中取出来用。
            if (!sound3D.transform.GetChild(i).gameObject.activeSelf)
            {
                go = sound3D.transform.GetChild(i).gameObject;
                go.SetActive(true);
                break;
            }
        }

        //如果对象池没有，则创建一个游戏对象。
        if (go == null)
        {
            go = new GameObject(soundClip3DName);
        }

        go.transform.position = worldPosition;
        go.transform.SetParent(parent);


        //如果该游戏对象身上没有AudioSource组件，则添加AudioSource组件并设置参数。
        if (!go.TryGetComponent<AudioSource>(out AudioSource audioSource))
        {
            audioSource = go.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 1f; //3D效果，近大远小。
        }

        //设置要播放的音频
        audioSource.clip = sound;

        //播放音频
        audioSource.Play();

        //每隔一秒检测一次，如果该音频播放完毕，则销毁该游戏对象。
        StartCoroutine(DestroyWhenFinished());

        IEnumerator DestroyWhenFinished()
        {
            do
            {
                yield return new WaitForSeconds(1);

                if (go == null || audioSource == null) yield break; //如果播放音频的游戏对象，或者AudioSource组件被销毁了，则直接跳出协程。
            } while (audioSource != null && audioSource.time > 0);

            if (go != null)
            {
                //放入对象池
                go.transform.SetParent(sound3D.transform);
                go.transform.localPosition = Vector3.zero;
                audioSource.clip = null;
                go.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 播放环境音效
    /// </summary>
    public void PlayBGS(AudioClip bgs, bool loop = true)
    {
        if (bgs == null)
        {
            Debug.LogWarning("播放BGS失败！要播放的BGS为null");
            return;
        }

        bgsAudioSource.loop = loop;
        bgsAudioSource.clip = bgs;
        bgsAudioSource.Play();
    }

    /// <summary>
    /// 暂停环境音效
    /// </summary>
    public void PauseBGS()
    {
        bgsAudioSource.Pause();
    }

    /// <summary>
    /// 取消暂停环境音效
    /// </summary>
    public void UnPauseBGS()
    {
        bgsAudioSource.UnPause();
    }

    /// <summary>
    /// 停止BGS
    /// </summary>
    public void StopBGS()
    {
        bgsAudioSource.Stop();
        bgsAudioSource.clip = null;
    }

    /// <summary>
    /// 播放提示音效
    /// </summary>
    /// <param name="ms"></param>
    public void PlayMS(AudioClip ms)
    {
        if (ms == null)
        {
            Debug.LogWarning("播放MS失败！要播放的MS为null");
            return;
        }

        //临时的空物体，用来播放提示音效
        GameObject go = null;

        for (int i = 0; i < msController.transform.childCount; i++)
        {
            //如果对象池中有，则从对象池中取出来用。
            if (!msController.transform.GetChild(i).gameObject.activeSelf)
            {
                go = msController.transform.GetChild(i).gameObject;
                go.SetActive(true);
                break;
            }
        }

        //如果对象池中没有，则创建一个游戏对象。
        if (go == null)
        {
            go = new GameObject(msClipName);
            go.transform.SetParent(msController.transform);
        }

        //如果该游戏对象身上有AudioSource组件，则添加AudioSource组件并设置参数。
        if (!go.TryGetComponent<AudioSource>(out AudioSource audioSource))
        {
            audioSource = go.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        //设置要播放的音频
        audioSource.clip = ms;

        //播放提示音效
        audioSource.Play();

        //每隔一秒检测一次，如果该提示音效播放完毕，则销毁。
        StartCoroutine(DestroyWhenFinished());

        //每隔一秒检测一次，如果该提示音效播放完毕，则销毁。
        IEnumerator DestroyWhenFinished()
        {
            do
            {
                yield return new WaitForSeconds(1);

                if (go == null || audioSource == null) yield break; //如果播放音频的游戏对象，或者AudioSource组件被销毁了，则直接跳出协程。
            } while (audioSource != null && audioSource.time > 0);

            if (go != null)
            {
                //放入对象池
                go.transform.SetParent(msController.transform);
                audioSource.clip = null;
                go.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 播放角色语音
    /// </summary>
    public AudioSource PlayVoice(AudioClip voice)
    {
        voiceAudioSource.clip = voice;
        voiceAudioSource.Play();
        return voiceAudioSource;
    }

    /// <summary>
    /// 停止角色语音
    /// </summary>
    public void StopVoice()
    {
        voiceAudioSource.Stop();
    }


    void Awake()
    {
        //创建并设置背景音乐的控制器
        bgmController = CreateController(bgmControllerName, transform);
        bgmAudioSource = bgmController.AddComponent<AudioSource>();
        bgmAudioSource.playOnAwake = false;
        bgmAudioSource.loop = true;

        //创建音效控制器
        soundController = CreateController(soundControllerName, transform);
        sound2D = CreateController(soundPool2DName, soundController.transform);
        sound3D = CreateController(soundPool3DName, soundController.transform);

        //创建并设置环境音效的控制器
        bgsController = CreateController(bgsControllerName, transform);
        bgsAudioSource = bgsController.AddComponent<AudioSource>();
        bgsAudioSource.playOnAwake = false;
        bgsAudioSource.loop = true;

        //创建提示音效的控制器
        msController = CreateController(msControllerName, transform);

        //创建并设置角色语音的控制器
        voiceController = CreateController(voiceControllerName, transform);
        voiceAudioSource = voiceController.AddComponent<AudioSource>();
        voiceAudioSource.playOnAwake = false;
        voiceAudioSource.loop = false;
    }

    GameObject CreateController(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        return go;
    }


    public void Print()
    {
        Debug.Log("AudioManager");
    }
}
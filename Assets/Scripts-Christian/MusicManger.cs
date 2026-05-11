using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance; // Singleton

    [Header("Audio Sources")]
    public AudioSource ambienceSource;
    public AudioSource chaseSource;

    [Header("Settings")]
    public float fadeDuration = 1.5f;

    void Awake()
    {
        // Singleton setup so any script can call MusicManager.Instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persists between scenes
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        ambienceSource.loop = true;
        chaseSource.loop = true;
        ambienceSource.Play();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        chaseSource.Stop();
        chaseSource.volume = 1f;
        if (!ambienceSource.isPlaying)
        {
            ambienceSource.volume = 1f;
            ambienceSource.Play();
        }
    }

    public void StartChase()
    {
        if (chaseSource.isPlaying) return; // don't restart if already playing

        StopAllCoroutines();
        StartCoroutine(FadeOut(ambienceSource, fadeDuration));
        StartCoroutine(FadeIn(chaseSource, fadeDuration));
    }

    public void EndChase()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut(chaseSource, fadeDuration));
        StartCoroutine(FadeIn(ambienceSource, fadeDuration));
    }

    IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
        source.Stop();
        source.volume = startVolume;
    }

    IEnumerator FadeIn(AudioSource source, float duration)
    {
        source.volume = 0;
        source.Play();
        while (source.volume < 1)
        {
            source.volume += Time.deltaTime / duration;
            yield return null;
        }
        source.volume = 1;
    }
}

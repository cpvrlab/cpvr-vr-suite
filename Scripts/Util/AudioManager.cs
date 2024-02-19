using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public void PlaySoundClip(SoundClip clip, Transform parent)
    {
        var go = Instantiate(new GameObject("SoundFX"), parent);
        var source = go.AddComponent<AudioSource>();
        source.clip = clip.clip;
        source.volume = clip.volume;
        source.pitch = clip.pitch + Random.Range(-0.025f, 0.025f);
        source.Play();
        Destroy(go, clip.clip.length * 1.1f);
    }
}

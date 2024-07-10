using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundClip
{
    public AudioClip clip;
    [Range(0, 1)] public float volume;
    [Range(0, 1)] public float pitch;
}

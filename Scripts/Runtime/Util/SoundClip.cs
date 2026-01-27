using System;
using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.Util
{
    [Serializable]
    public class SoundClip
    {
        public AudioClip clip;
        [Range(0, 1)] public float volume;
        [Range(0, 1)] public float pitch;
    }
}

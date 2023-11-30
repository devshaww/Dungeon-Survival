using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    public string soundEffectName;
    public GameObject soundPrefab;
    public AudioClip soundEffectClip;
    [Range(.1f, 1.5f)]
    public float soundEffectPitchVariationMin = .8f;
    [Range(.1f, 1.5f)]
    public float soundEffectPitchVariationMax = 1.2f;
    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;
}

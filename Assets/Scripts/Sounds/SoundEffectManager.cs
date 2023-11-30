using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager>
{
    public int soundVolume = 8;

	private void Start()
	{
		SetSoundVolume(soundVolume);
	}

	private void SetSoundVolume(int soundVolume)
	{
		float muteDecibels = -80f;

		if (soundVolume == 0) {
			GameResources.Instance.soundMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
		} else {
			GameResources.Instance.soundMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibels(soundVolume));
		}
	}

	public void PlaySoundEffect(SoundEffectSO soundEffectSO)
	{
		SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffectSO.soundPrefab, Vector3.zero, Quaternion.identity);
		sound.SetSound(soundEffectSO);
		sound.gameObject.SetActive(true);
		StartCoroutine(DisableSound(sound, soundEffectSO.soundEffectClip.length));
	}

	private IEnumerator DisableSound(SoundEffect soundEffect, float length)
	{
		yield return new WaitForSeconds(length);
		soundEffect.gameObject.SetActive(false);
	}

}

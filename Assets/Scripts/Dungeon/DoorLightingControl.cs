using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

	private void Awake()
	{
		door = GetComponentInParent<Door>();
	}

	public void FadeInDoor(Door door)
	{
		Material material = new Material(GameResources.Instance.variableLitShader);

		if (!isLit)
		{
			SpriteRenderer[] spriteRenderers = GetComponentsInParent<SpriteRenderer>();

			foreach (SpriteRenderer renderer in spriteRenderers)
			{
				StartCoroutine(FadeIndoorRoutine(renderer, material));
			}
			isLit = true;
		}
	}

	private IEnumerator FadeIndoorRoutine(SpriteRenderer renderer, Material material)
	{
		renderer.material = material;

		for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
		{
			material.SetFloat("Alpha_Slider", i);
			yield return null;  // wait for the nexe frame to continue
		}

		renderer.material = GameResources.Instance.litMaterial;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		FadeInDoor(door);
	}
}

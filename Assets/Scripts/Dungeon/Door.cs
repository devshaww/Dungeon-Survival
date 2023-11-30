using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    [SerializeField] private BoxCollider2D doorCollider;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;

    [HideInInspector] public bool isBossRoomDoor = false;


	private void Awake()
	{
        doorCollider.enabled = false;
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
        {
            OpenDoor();
        }
    }

    // TODO: Might not need this. Have a test on this later.
	private void OnEnable()
	{
        animator.SetBool(Settings.open, isOpen);
	}

	private void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;

            animator.SetBool(Settings.open, true);
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenSoundEffect);
        }
    }

    public void LockDoor()
    {
        isOpen = false;
        // enable collision to block player
        doorCollider.enabled = true;
		// trigger to open the door, set to false so that OnTriggerEnter2D don't get called and neither do OpenDoor
		doorTrigger.enabled = false;

        animator.SetBool(Settings.open, false);
    }

	public void UnlockDoor()
	{
		// enable collision to block player
		doorCollider.enabled = false;
		// trigger to open the door, set to false so that OnTriggerEnter2D don't get called and neither do OpenDoor
		doorTrigger.enabled = true;

        if (previouslyOpened)
        {
            isOpen = false;     // might be ok to delete this line
            OpenDoor();
        }
	}

}

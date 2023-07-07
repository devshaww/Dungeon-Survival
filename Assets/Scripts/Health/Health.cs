using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

	private int maxHealth;
	private int currentHealth;
	private Player player;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMaxHealth(int maxHealth)
    {
		this.maxHealth = maxHealth;
		currentHealth = maxHealth;
	}

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}

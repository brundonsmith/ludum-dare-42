using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetersController : MonoBehaviour {

    public GameObject healthBar;
    public GameObject manaBar;
    public GameObject staminaBar;
    public int max = 10; // Must be at least 1. More than MAX units of any stat will not be displayed.
    private BackpackController backpackController;

    // Use this for initialization
    void Start () {
        UpdateMeters();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // to be called whenever the contents of the backpack change
    public void UpdateMeters()
    {
        if(backpackController == null) {
            backpackController = FindObjectOfType<BackpackController>();
        }
        UpdateMeter(healthBar,  backpackController.TotalHealth());
        UpdateMeter(manaBar,    backpackController.TotalMana());
        UpdateMeter(staminaBar, backpackController.TotalStamina());
    }

    private void UpdateMeter(GameObject meter, int totalStatValue)
    {
        string debugString = "Updating " + meter + " to " + totalStatValue;
        if (totalStatValue > max)
        {
            debugString = debugString + " (capped at " + max + ")";
            totalStatValue = max;
        }
        //Debug.Log(debugString);
        meter.transform.localScale = X11((float) totalStatValue / max);
    }

    // @todo promote to utilities
    public Vector3 X11(float x) {
        // @return A Vector3 of the form (x, 1.0, 1.0)
        return new Vector3(x, 1, 1);
    }
}

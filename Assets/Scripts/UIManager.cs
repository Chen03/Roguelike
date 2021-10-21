using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    /*  UI Update List
        1. HealthBar
        2. EnergyBar
    */
    GameObject player;
    HealthSystem playerHealthSystem;
    HealthData playerHealth;

    RectTransform healthBar;
    float maxHealthWidth;
    
    RectTransform energyBar;
    float maxEnergyWidth;

    Text healthText;
    Text energyText;

    void Start()
    {
        player = GameObject.Find("Player");
        playerHealthSystem = player.GetComponent<HealthSystem>();
        foreach (RectMask2D m in GetComponentsInChildren<RectMask2D>()) {
            if (m.name == "HealthBar")  healthBar = m.rectTransform;
            if (m.name == "EnergyBar")  energyBar = m.rectTransform;
        }
        foreach (Text t in GetComponentsInChildren<Text>()) {
            if (t.name == "HealthText") healthText = t;
            if (t.name == "EnergyText") energyText = t;
        }

        //HealthBar
        maxHealthWidth = healthBar.rect.width;
        //EnergyBar
        maxEnergyWidth = energyBar.rect.width;
    }

    void OnGUI()
    {
        playerHealth = playerHealthSystem.health;
        //HealthBar
        healthBar.SetSizeWithCurrentAnchors(0,
            maxHealthWidth * (playerHealth.HP / playerHealth.MaxHP));
        //EnergyBar
        energyBar.SetSizeWithCurrentAnchors(0,
            maxEnergyWidth * (playerHealth.EP / playerHealth.MaxEP));

        healthText.text = playerHealth.HP + "/" + playerHealth.MaxHP;
        energyText.text = playerHealth.EP + "/" + playerHealth.MaxEP;
    }
}

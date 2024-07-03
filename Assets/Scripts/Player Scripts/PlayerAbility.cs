using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    private bool canSwitch = true;

    [SerializeField] private float switchCooldown;
    private float lastSwitchUsed;

    #region Objects being turned on and off
    [Header("Switch Platforms")]
    [SerializeField] GameObject whitePlatforms;
    [SerializeField] GameObject blackPlatforms;

    [Space(3)]
    [Header("Switch Enemies")]
    [SerializeField] GameObject whiteEnemies;
    [SerializeField] GameObject blackEnemies;

    [Space(3)]
    [Header("Switch Spikes")]
    [SerializeField] GameObject whiteSpikes;
    [SerializeField] GameObject blackSpikes;
    #endregion

    #region Player Sprite Changer
    [Space(20)]
    [Header("Color Change")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Sprite playerWhiteChange;
    [SerializeField] private Sprite playerBlackChange;
    #endregion

    void Update()
    {
        Switch();
    }

    //Gets when the player uses the Switch Mechanic and calls the DisableObject
    void Switch()
    {
        if(Input.GetButtonDown("Switch") && Time.time > lastSwitchUsed)
        {
            DisableObjects();

            lastSwitchUsed = Time.time + switchCooldown;
        }
    }

    //Used to check which objects are supposed to be on and off so the scene gets altered
    public void DisableObjects()
    {
        if (canSwitch)
        {
            canSwitch = false;

            playerSprite.sprite = playerBlackChange;

            whitePlatforms.SetActive(true);
            blackPlatforms.SetActive(false);

            whiteEnemies.SetActive(true);
            blackEnemies.SetActive(false);

            whiteSpikes.SetActive(true);
            blackSpikes.SetActive(false);
        }
        else
        {
            canSwitch = true;

            playerSprite.sprite = playerWhiteChange;

            whitePlatforms.SetActive(false);
            blackPlatforms.SetActive(true);

            whiteEnemies.SetActive(false);
            blackEnemies.SetActive(true);

            whiteSpikes.SetActive(false);
            blackSpikes.SetActive(true);
        }
    }
}

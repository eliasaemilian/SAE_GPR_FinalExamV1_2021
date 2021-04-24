using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private SpellCastingController spellCastingController;
    [SerializeField] private DropCollector dropCollector;

    [SerializeField] private Image spellIcon;
    [SerializeField] private TMPro.TMP_Text spellCooldownText;
    [SerializeField] private Image spellIconSpecial;
    [SerializeField] private TMPro.TMP_Text spellCooldownTextSpecial;

    [SerializeField] private GameObject collectUIObject;
    [SerializeField] private GameObject pickupUIObject;
    [SerializeField] private TMPro.TMP_Text pickUpText;
    [SerializeField] private float pickUpWaitTime = 2.5f;

    private bool playPickUpTextAnimation;
    private float pickUpTimer;
    private Animation pickUpAnimation;

    private void Start()
    {
        Debug.Assert(spellCastingController != null, "SpellCastingController reference is null");
        Debug.Assert(dropCollector != null, "DropCollector reference is null");

        spellIcon.sprite = spellCastingController.SimpleAttackSpellDescription.SpellIcon;
        if (spellCastingController.HasSpecialAbility ) spellIconSpecial.sprite = spellCastingController.SpecialAttackSpellDescription.SpellIcon;
        else
        {
            spellIconSpecial.sprite = null;
        }

        dropCollector.DropsInRangeChanged += OnDropsInRangeChanged;
        dropCollector.DropCollected += OnDropCollected;
        dropCollector.DropAbilityCollected += OnDropAbilityCollected;

        pickUpAnimation = pickupUIObject.GetComponent<Animation>();
        pickupUIObject.SetActive( false );

    }

    private void OnDropsInRangeChanged()
    {
        collectUIObject.SetActive(dropCollector.DropsInRangeCount > 0);
    }

    private void OnDropCollected(Drop drop)
    {
        pickUpText.text = drop.OnPickUpPhrase;
        pickupUIObject.SetActive( true );
        playPickUpTextAnimation = true;
    }

    private void OnDropAbilityCollected(DropAbility ability)
    {
        // if none enable icon
        spellIconSpecial.sprite = spellCastingController.SpecialAttackSpellDescription.SpellIcon;
        // change icon to new ability
    }


    private void Update()
    {
        float cooldown = spellCastingController.GetSimpleAttackCooldown();
        if (cooldown > 0)
        {
            spellCooldownText.text = cooldown.ToString("0.0");
            spellIcon.color = new Color(0.25f, 0.25f, 0.25f, 1);
        }
        else
        {
            spellCooldownText.text = "";
            spellIcon.color = Color.white;
        }


        cooldown = spellCastingController.GetSpecialAttackCooldown();
        if ( cooldown > 0 )
        {
            spellCooldownTextSpecial.text = cooldown.ToString( "0.0" );
            spellIconSpecial.color = new Color( 0.25f, 0.25f, 0.25f, 1 );
        }
        else
        {
            spellCooldownTextSpecial.text = "";
            spellIconSpecial.color = Color.white;
        }

        if (playPickUpTextAnimation)
        {
            playPickUpTextAnimation = false;
            if ( pickUpAnimation.isPlaying ) pickUpAnimation.Rewind();
            pickUpAnimation.Play();
            pickUpTimer = 0;
        }

        pickUpTimer += Time.deltaTime;

        if (pickUpTimer > pickUpWaitTime)
        {
            pickupUIObject.SetActive( false );
        }
    }

}

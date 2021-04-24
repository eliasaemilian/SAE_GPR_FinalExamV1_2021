using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAction
{
    bool IsInAction();
}

public class SpellCastingController : MonoBehaviour, IPlayerAction
{
    public event System.Action<DropAbility> DropAbilityUpdated;

    [SerializeField] private DropCollector dropCollector;


    [SerializeField] private Animator animator;
    [SerializeField] private Transform castLocationTransform;
    [SerializeField] private ProjectileSpellDescription simpleAttackSpell;
    [SerializeField] private SpellDescription specialAttackSpell;

    private bool _hasSpecialAbility;
    public bool HasSpecialAbility { get { return _hasSpecialAbility; } }


    private bool inAction;
    private float lastSimpleAttackTimestamp = -100, lastSpecialAttackTimestamp = -100;

    public SpellDescription SimpleAttackSpellDescription { get => simpleAttackSpell; }
    public SpellDescription SpecialAttackSpellDescription { get => specialAttackSpell; }

    private List<DropAbility> collectedAbilities = new List<DropAbility>();


    private void Start()
    {
        Debug.Assert(simpleAttackSpell, "No spell assigned to SpellCastingController.");
        Debug.Assert( dropCollector != null, "DropCollector reference is null" );

        dropCollector.DropAbilityCollected += OnSpecialAbilityCollected;

    }

    void Update()
    {
        bool simpleAttack = Input.GetButtonDown("Fire1");
        bool specialAttack = Input.GetButtonDown("Fire2");

        if (!inAction)
        {
            if (simpleAttack && GetSimpleAttackCooldown() == 0)
            {
                StartCoroutine( SimpleAttackRoutine());
            }
            else if (HasSpecialAbility && specialAttack && GetSpecialAttackCooldown() == 0 )
            {
                Debug.Log("Trigger special attack");
               if (specialAttackSpell is ProjectileSpellDescription)  StartCoroutine( SpecialAttackRoutineProjectile() );
               else if (specialAttackSpell is RuneSpellDescription) StartCoroutine( SpecialAttackRoutineRune() );
            }
        }
    }

    private IEnumerator SpecialAttackRoutineProjectile()
    {
        ProjectileSpellDescription spell = specialAttackSpell as ProjectileSpellDescription;
        inAction = true;
        animator.SetTrigger( specialAttackSpell.AnimationVariableName);

        yield return new WaitForSeconds( spell.ProjectileSpawnDelay);

        Instantiate( spell.ProjectilePrefab, castLocationTransform.position, castLocationTransform.rotation);

        yield return new WaitForSeconds( specialAttackSpell.Duration - spell.ProjectileSpawnDelay);
        lastSpecialAttackTimestamp = Time.time;
        inAction = false;
    }

    private IEnumerator SpecialAttackRoutineRune()
    {
        RuneSpellDescription spell = specialAttackSpell as RuneSpellDescription;
        inAction = true;
        animator.SetTrigger( specialAttackSpell.AnimationVariableName );

        Instantiate( spell.RunePrefab, castLocationTransform.position, castLocationTransform.rotation );

        yield return new WaitForSeconds( specialAttackSpell.Duration - spell.RuneSpawnDelay );
        lastSpecialAttackTimestamp = Time.time;
        inAction = false;
    }

    private IEnumerator SimpleAttackRoutine()
    {
        inAction = true;
        animator.SetTrigger( simpleAttackSpell.AnimationVariableName );

        yield return new WaitForSeconds( simpleAttackSpell.ProjectileSpawnDelay );

        Instantiate( simpleAttackSpell.ProjectilePrefab, castLocationTransform.position, castLocationTransform.rotation );

        yield return new WaitForSeconds( simpleAttackSpell.Duration - simpleAttackSpell.ProjectileSpawnDelay );
        lastSimpleAttackTimestamp = Time.time;

        inAction = false;
    }

    public bool IsInAction()
    {
        return inAction;
    }

    public float GetSimpleAttackCooldown()
    {
        return Mathf.Max(0, lastSimpleAttackTimestamp + simpleAttackSpell.Cooldown - Time.time);
    }

    public float GetSpecialAttackCooldown()
    {
        return Mathf.Max( 0, lastSpecialAttackTimestamp + specialAttackSpell.Cooldown - Time.time );
    }

    private void OnSpecialAbilityCollected( DropAbility ability )
    {
        _hasSpecialAbility = true;
        collectedAbilities.Add( ability );
        specialAttackSpell = ability.SpellDescription;
        DropAbilityUpdated?.Invoke(ability);
        Debug.Log( "Set new Special Ability" );
    }

    public bool HasAbilityCollected( DropAbility ability )
    {
        return collectedAbilities.Contains( ability );
    }

}

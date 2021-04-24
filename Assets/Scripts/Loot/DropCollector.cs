using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DropCollector : MonoBehaviour
{
    List<RuntimeDropInstance> dropsInRange = new List<RuntimeDropInstance>();

    public event System.Action DropsInRangeChanged;
    public event System.Action<Drop> DropCollected;
    public event System.Action<DropAbility> DropAbilityCollected;

    public int DropsInRangeCount { get => dropsInRange.Count; }

    [SerializeField] private SpellCastingController spellCastingController;

    private void Start()
    {
        Debug.Assert( spellCastingController != null, "SpellCastingController reference is null" );

    }

    private void Update()
    {
        if (dropsInRange.Count > 0)
        {
            if (Input.GetButtonDown("Collect"))
            {
                var dropInstance = GetClosestDrop();
                if(dropInstance != null)
                {
                    Collect(dropInstance);
                }
            }
        }
    }

    private void Collect(RuntimeDropInstance drop)
    {
        Debug.Log($"Collected: {drop.GetDrop().DropName}");
        dropsInRange.Remove(drop);
        Destroy(drop.gameObject);
        DropsInRangeChanged?.Invoke();
        DropCollected?.Invoke(drop.GetDrop());

        DropAbility ability = drop.GetDrop() as DropAbility;
        if ( ability != null && !spellCastingController.HasAbilityCollected(ability))
        {
            Debug.Log( "Picked up new Ability" );
            DropAbilityCollected?.Invoke( ability );
        }
    }

    private RuntimeDropInstance GetClosestDrop()
    {
        if (dropsInRange.Count == 0)
            return null;

        return dropsInRange.OrderBy((x) => Vector3.Distance(transform.position, x.transform.position)).First();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out RuntimeDropInstance runtimeDropInstance))
        {
            dropsInRange.Add(runtimeDropInstance);
            DropsInRangeChanged?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out RuntimeDropInstance runtimeDropInstance))
        {
            dropsInRange.Remove(runtimeDropInstance);
            DropsInRangeChanged?.Invoke();
        }
    }
}

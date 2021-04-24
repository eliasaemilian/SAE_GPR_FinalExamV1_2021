using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float selfDestructTime;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float damage;
    [SerializeField] private float explosionDelayTime;


    private float timer, explosionTimer;
    private bool isTriggered;
    private Vector3 onTriggerPos;
    private GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > selfDestructTime)
        {
            Destroy( gameObject );
        }

        if (isTriggered)
        {
            if (explosion == null)
            {
                explosion = Instantiate( explosionPrefab, transform.position, Quaternion.identity );
                DamageEnemiesInRange();
            }

            explosionTimer += Time.deltaTime;

            if (explosionTimer > explosionDelayTime)
            {
                isTriggered = false;
                Destroy( explosion );
                Destroy( gameObject );
            }


        }
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( other.TryGetComponent( out IDamagable damagable ) )
        {
            isTriggered = true;
            onTriggerPos = other.transform.position;
        }
    }

    private void DamageEnemiesInRange()
    {
        Collider[] colliders = Physics.OverlapSphere( onTriggerPos, explosionRadius );
        for ( int i = 0; i < colliders.Length; i++ )
        {
            if ( colliders[i].TryGetComponent( out IDamagable damagable ) )
            {
                damagable.TakeDamage( damage );
                Debug.Log( "Damaged Enemy from Fire Trap" );
            }
        }

    }

}

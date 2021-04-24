using System.Collections;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float velocity;
    [SerializeField] private float damage;
    [SerializeField] private float selfdestructTime = 10;
    [SerializeField] private GameObject damageEffectPrefab;
    [SerializeField] private float damageEffectDelayTime = 2;
    [SerializeField] private GameObject projectileEffect;

    private GameObject go;
    private float timer;
    private bool spawned, playEffect;

    private void Start()
    {
        _rigidbody.velocity = transform.forward * velocity;
        Destroy(gameObject, selfdestructTime);
    }

    private void Update()
    {
       if (playEffect)
        {
            if ( !spawned )
            {
                go = Instantiate( damageEffectPrefab, transform.position, Quaternion.identity );
                spawned = true;
            }

            timer += Time.deltaTime;

            if (timer > damageEffectDelayTime)
            {
                playEffect = false;
                Destroy( go );
                Destroy( gameObject );
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(damage);
            playEffect = true;
            _rigidbody.isKinematic = true;
            projectileEffect.SetActive( false );
        }
        
    }

}

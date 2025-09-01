using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public GameObject CollectedParticleEffectObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void HasBeenCollected()
    {
        Debug.Log("Collected");
        // Spawn a PFX at this objects position.
        if (CollectedParticleEffectObject is not null)
        {
            GameObject tempGameObject = Instantiate(CollectedParticleEffectObject, transform.position, Quaternion.identity);

            if (tempGameObject.TryGetComponent<ParticleSystem>(out ParticleSystem collectedPFX))
            {
                Destroy(tempGameObject, collectedPFX.startLifetime); // Destroy the PFX when it has finished
            }
            
        }

        // Destroy this GameObject now
        Destroy(this.gameObject);
    }
}

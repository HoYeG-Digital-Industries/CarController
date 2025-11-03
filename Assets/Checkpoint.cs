using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    private CheckPointManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<CheckPointManager>();
        GetComponent<Collider>().isTrigger = true;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.OnCheckpointReached(this);
        }
    }
}

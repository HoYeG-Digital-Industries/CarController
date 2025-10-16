using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint : MonoBehaviour
{
    [SerializeField] private string selectedTag;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(selectedTag))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }
}

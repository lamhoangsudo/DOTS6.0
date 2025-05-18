using UnityEngine;

public class RagdollDeadTimer : MonoBehaviour
{
    private float timer = 6f;
    private bool hasCollided = true;
    private void Update()
    {
        timer -= Time.deltaTime;
        if (hasCollided && timer <= 3)
        {
            foreach (CharacterJoint characterJoint in GetComponentsInChildren<CharacterJoint>())
            {
                Destroy(characterJoint);
            }
            foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
            {
                Destroy(rigidbody);
            }
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                Destroy(collider);
            }
            hasCollided = false;
        }
        if (timer <= 1f)
        {
            transform.position += Vector3.down * Time.deltaTime;
        }
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}

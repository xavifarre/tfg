using UnityEngine;
using System.Collections;

public class BarrelTrigger : MonoBehaviour
{
    private BarrelProximity barrel;
    private bool activated;
    public float rotationSpeed;

    private void Start()
    {
        barrel = transform.parent.GetComponent<BarrelProximity>();
        transform.localScale = Vector3.one * barrel.triggerRadius;

        activated = false;
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !activated)
        {
            activated = true;
            barrel.StartCountdown();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class SwordPickUp : MonoBehaviour, IInteractuableObject
{
    private Player player;
    public Attack swordAttack;
    public Light2D light2DLevel;
    public Light2D light2DSword;

    public float minRadius, maxRadius;
    public float innerRadius;
    public float duration;
    private float invultTime;

    public RainObject rainObject;

    private bool active = true;
    private void Start()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(ILightAnim());
    }
    public void Interact()
    {
        ToriiLevelController.instance.PickSword();
        GetComponent<Animator>().SetTrigger("PickUp");
        GetComponent<Collider2D>().enabled = false;
        invultTime = player.invulnerableTime;
        GameManager.instance.Shake(13f, 0.2f, 1f);
        player.invulnerableTime = 0;
    }

    public void DestroySword()
    {
        player.invulnerableTime = invultTime;
        light2DLevel.intensity = 1;
        active = false;
        Destroy(gameObject);
    }

    public void PickSword()
    {
        player.PickSword();
    }

    public void HitPlayer()
    {
        player.Hit(swordAttack);
    }

    public void EndRain()
    {
        light2DLevel.intensity = 1;
        rainObject.EndRain();
    }

    private IEnumerator ILightAnim()
    {
        float t = 0;

        while (active)
        {
            while (t < duration)
            {
                t += Time.deltaTime;
                if (duration == 0) duration = 1;
                light2DSword.pointLightInnerRadius = Mathf.Lerp(minRadius, maxRadius, t / duration) * innerRadius;
                light2DSword.pointLightOuterRadius = Mathf.Lerp(minRadius, maxRadius, t / duration);
                yield return null;
            }
            t = 0;

            while (t < duration)
            {
                t += Time.deltaTime;
                if (duration == 0) duration = 1;
                light2DSword.pointLightInnerRadius = Mathf.Lerp(maxRadius, minRadius, t / duration)* innerRadius;
                light2DSword.pointLightOuterRadius = Mathf.Lerp(maxRadius, minRadius, t / duration);
                yield return null;
            }
            t = 0;
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricitySummon : MonoBehaviour
{
    public float delay1, delay2;

    public Animator animator1;
    public Animator animator2;

    public SpriteRenderer sp1;
    public SpriteRenderer sp2;

    public List<Material> materialsColors;


    public void PlayAnim(int type1, int type2)
    {
        sp1.material = materialsColors[type1];
        sp2.material = materialsColors[type2];

        StartCoroutine(IPlayElectricity1());
        StartCoroutine(IPlayElectricity2());
    }

    private IEnumerator IPlayElectricity1()
    {
        yield return new WaitForSeconds(delay1);
        animator1.SetTrigger("Show");
    }

    private IEnumerator IPlayElectricity2()
    {
        yield return new WaitForSeconds(delay2);
        animator2.SetTrigger("Show");
    }
}

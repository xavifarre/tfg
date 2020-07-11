using UnityEngine;
using System.Collections;

public class Sentinel : Minion
{
    [Header("Attack")]
    //Charge
    public float chargeTime = 1f;

    //Attack
    public float attackDistance = 10f;
    public Laser projectile;

    [Header("Movement")]
    //Move
    public float minDistanceMove = 2f;
    public float maxDistanceMove = 10f;
    public float minDistancePlayer = 5f;

    private RectArea areaMove;

    protected override void Init()
    {
        base.Init();
        areaMove = GameObject.Find("MovementPoints").transform.Find("3").GetComponent<RectArea>();
    }

    //Move
    protected override void UpdateMove()
    {
        Vector3 direction = (endActionPoint - realPos).normalized;
        realPos = realPos + speed * direction * Time.deltaTime;

        if(Vector3.Distance(endActionPoint, realPos) < 0.3f)
        {
            Charge();
        }
        PixelPerfectMovement.Move(realPos, rb);
    }

    protected override void StartMove()
    {
        endActionPoint = areaMove.RandomPoint();
        int n = 0;
        while((Vector3.Distance(endActionPoint, realPos) > maxDistanceMove || Vector3.Distance(endActionPoint, realPos) < minDistanceMove || Vector3.Distance(endActionPoint, player.transform.position) < minDistancePlayer) && n < 100)
        {
            n++;
            endActionPoint = areaMove.RandomPoint();
        }
        state = MinionState.Move;
    }

    private void Charge()
    {
        state = MinionState.Charge;
        StartCoroutine(ICharge());
    }

    private void Attack()
    {
        Vector2 dir = (player.transform.position + (Vector3)player.lastDir.normalized * player.movementValue.magnitude - realPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Laser instance = Instantiate(projectile, realPos, Quaternion.AngleAxis(angle -90,Vector3.forward));
        instance.damage = damage;
        instance.knockback = knockBackValue;
        StartIdle();
    }

    IEnumerator ICharge()
    {
        yield return new WaitForSeconds(chargeTime);
        if (state == MinionState.Charge)
        {
            Attack();
        }
    }
}

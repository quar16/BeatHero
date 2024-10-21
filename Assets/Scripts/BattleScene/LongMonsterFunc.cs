using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongMonsterFunc : MonoBehaviour
{
    public BattleSet battleSet;
    public GameObject center;
    public LongMonsterFunc Next;
    public Animator animator;
    public GameObject Effect;
    public void Active()
    {
        StartCoroutine(Activing());
    }
    public float l;
    public IEnumerator Activing()
    {
        Vector3 startV = transform.position;
        Effect.SetActive(true);
        animator.SetTrigger("Attack");
        while (l < 1)
        {
            transform.position = Vector3.Lerp(startV, center.transform.position, l);
            l += Time.deltaTime * 2;
            yield return null;
            if (l > 0.55f && battleSet.isLongGuard)
            {
                battleSet.LongPerfact(gameObject.transform.position);
                StartCoroutine(SetHit());
            }
            if (isHit)
                yield break;
        }
        battleSet.LongMiss(gameObject.transform.position);
        Destroy(gameObject);
    }
    bool isHit = false;
    public IEnumerator SetHit()
    {
        isHit = true;
        animator.SetTrigger("Hit");
        Effect.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}

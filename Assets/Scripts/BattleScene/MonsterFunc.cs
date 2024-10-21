using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFunc : MonoBehaviour
{
    public BattleSet battleSet;
    public GameObject center;
    public AudioSource effects;
    public Animator animator;
    public GameObject Effect;
    public int line;
    public void Active()
    {
        StartCoroutine(Activing());
    }
    public void Summon()
    {
        gameObject.SetActive(true);
        effects.Play();
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
            if (line == -1 && l > 0.5f)
            {
                transform.position = Vector3.Lerp(startV, center.transform.position, 0.5f);
                battleSet.animator.SetTrigger("Guard");
                battleSet.Perfact();
                yield break;
            }
            yield return null;
            if (isHit)
                yield break;
        }
        battleSet.Miss();
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

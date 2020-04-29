using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyState
{
    Idle, Walk, Check, Attack, Hit
}

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public GameObject tempEndPanel;

    private Animator enemyAni;
    private AudioSource enemyAudioSource;

    public AudioClip damagedSound;
    public AudioClip roaringSound;
    public AudioClip hitSound;

    [Space]
    public EnemyState enemystate = EnemyState.Idle;

    private bool isDead = false;
    public Image HpSlider;
    public static float maxHp = 100;
    [HideInInspector]
    public float currentHp;


    [Space]
    public GameObject targetPlayer;
    public GameObject hitEffect;

    public float followRotateTime = 0.1f;

    private float previousRotation;

    public Vector3 attackOffset;
    public Vector3 attackSize = Vector3.one;

    [Space]
    public Vector3 checkOffset;
    public Vector3 checkSize = Vector3.one;

    private bool isOverlap = false;

    void Start()
    {
        enemyAni = GetComponent<Animator>();
        enemyAudioSource = GetComponent<AudioSource>();

        StartCoroutine(enemyIntro());

        currentHp = maxHp;
    }

    void FixedUpdate()
    {
        HpSlider.fillAmount = currentHp / maxHp;

        if (isDead) return;

        EnemyMove();
        RaycastCheck();
        EnemyAttack();
    }

    IEnumerator enemyIntro()
    {
        enemyAni.SetTrigger("Intro");

        yield return new WaitForSeconds(1.25f);

        enemyAudioSource.PlayOneShot(roaringSound);
    }

    void EnemyMove()
    {
        if (enemystate.Equals(EnemyState.Check) || enemystate.Equals(EnemyState.Attack))
        {
            enemyAni.SetBool("IsWalk", false);
            return;
        }

        Vector3 lookRotation = targetPlayer.transform.position - transform.position;
        float lookHorizontal = Mathf.Atan2(lookRotation.x, lookRotation.z) * Mathf.Rad2Deg;

        previousRotation = lookHorizontal;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, lookHorizontal, 0), followRotateTime);
        enemyAni.SetBool("IsWalk", true);
        enemystate = EnemyState.Walk;
    }

    void RaycastCheck()
    {
        if (enemystate.Equals(EnemyState.Attack))
        {
            return;
        }

        Vector3 tempCheckPos = transform.position + transform.forward * checkOffset.z + Vector3.up * checkOffset.y;
        Collider[] hit = Physics.OverlapBox(tempCheckPos, checkSize, transform.rotation, LayerMask.GetMask("Player"));

        if (hit.Length > 0)
        {
            enemystate = EnemyState.Check;
        }
        else
        {
            enemystate = EnemyState.Idle;
        }
    }

    void EnemyAttack()
    {
        if (enemyAni.GetCurrentAnimatorStateInfo(0).IsName("Mutant Swipe") || enemyAni.GetCurrentAnimatorStateInfo(0).IsName("Mutant Punch"))
        {
            if (enemyAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                enemystate = EnemyState.Idle;
            }
            else if (enemyAni.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f && !isOverlap)
            {
                isOverlap = true;

                Vector3 tempAttackPos = transform.position + transform.forward * attackOffset.z + Vector3.up * attackOffset.y;
                Collider[] hit = Physics.OverlapBox(tempAttackPos, attackSize, transform.rotation, LayerMask.GetMask("Player"));

                if (hit.Length > 0)
                {
                    enemyAudioSource.PlayOneShot(hitSound);
                    StartCoroutine(FindObjectOfType<Player>().PlayerDamaged());
                }
            }
            return;
        }
        else if (enemyAni.GetCurrentAnimatorStateInfo(0).IsName("Mutant Damaged"))
        {
            if (enemyAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                enemystate = EnemyState.Idle;
            }
            return;
        }
        isOverlap = false;

        if (!enemystate.Equals(EnemyState.Check)) return;

        int tempRand = Random.Range(0, 2);

        switch (tempRand)
        {
            case 0:
                enemyAni.SetTrigger("Swipe");
                enemystate = EnemyState.Attack;
                break;
            case 1:
                enemyAni.SetTrigger("Punch");
                enemystate = EnemyState.Attack;
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 tempAttackPos = transform.position + transform.forward * attackOffset.z + Vector3.up * attackOffset.y;
        Vector3 attackPos = transform.InverseTransformPoint(tempAttackPos);

        Vector3 tempCheckPos = transform.position + transform.forward * checkOffset.z + Vector3.up * checkOffset.y;
        Vector3 checkPos = transform.InverseTransformPoint(tempCheckPos);

        Gizmos.DrawWireCube(attackPos, attackSize);
        Gizmos.DrawWireCube(checkPos, checkSize);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Weapon"))
        {
            StartCoroutine(FindObjectOfType<FollowCamera>().Shake(0.25f, 0.05f));
            GameObject effect = Instantiate(hitEffect, col.transform.position, hitEffect.transform.rotation);

            currentHp -= 10;

            if (currentHp <= 0)
            {
                isDead = true;
                enemyAni.SetTrigger("Die");
            }
            else
            {
                enemyAni.SetTrigger("Damaged");
                enemystate = EnemyState.Hit;
                enemyAudioSource.PlayOneShot(damagedSound);
                enemyAni.SetFloat("Speed", (maxHp - currentHp) / maxHp + 0.5f);
            }
            
            Destroy(effect, 1f);
            Destroy(col.gameObject);
        }
    }

    public void End()
    {
        tempEndPanel.SetActive(true);
    }
}

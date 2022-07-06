using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B};
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public bool isChase;
    public bool isAttack;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        Invoke("ChaseStart", 2);        // 2���Ŀ� ��������
    }

    private void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;       // �Ϻ��ϰ� ���߰� �ϱ�
        }
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
        Targeting();
    }

    private void Targeting()
    {
        float targetRadius = 0;
        float targetRange = 0;

        switch(enemyType)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3.0f;
                break;

            case Type.B:
                targetRadius = 1.5f;
                targetRange = 8.0f;
                break;
        }

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if(rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;        // �߰� ���� �� ����
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch(enemyType)
        {
            case Type.A:

                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1.0f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1.0f);
                break;

            case Type.B:

                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1.5f);

                break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }

    // �������� NavAgent �̵��� �������� �ʵ��� �ϱ�
    private void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            Player player = collision.gameObject.GetComponent<Player>();
            curHealth -= player.damage;
            StartCoroutine(OnDamage());

            Debug.Log("Enemy : " + curHealth);
        }
    }

    IEnumerator OnDamage()
    {
        mat.color = Color.red;      // �ǰ�����Ʈ(���� Ƽ�� ���� ����)
        anim.SetTrigger("Take Damage");

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }

        else
        {
            isChase = false;        // ���� ����
            mat.color = Color.gray;
            gameObject.layer = 8;       // �׾����� ��ü������ �ε����� ���̾� ����
            nav.enabled = false;        // ��� ���׼��� ���� NavAgent ��Ȱ��

            anim.SetTrigger("Die");

            Destroy(gameObject, 3);
        }
    }

    // �߰� ����
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("Walk Forward", true);
    }

}

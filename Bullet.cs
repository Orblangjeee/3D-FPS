using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� : �Ѿ��� ������ ���ư��� �ϰ� �ʹ�.
//  + Rigidbody�� ���� �Ѿ��� �������� �����ϰ� �ʹ�.
//  - ���� : �Ѿ��� ���ư��� ����, �Ѿ��� Z�� ����
//  - �ӷ� : �Ѿ��� ���ư��� �ӷ�
//  - Rigidbody Component 

// ����a : �Ѿ��� ��򰡿� �ε�����, �Ѿ��� �����Ѵ�.
//  - �ε��� üũ�� �ʿ�
// ����b : ��𿡵� �ε����� ���� ��찡 ����� ������,
//         �����ð� ���Ŀ�, �Ѿ��� �����ϰ� �����.
//  - �����ð� (lifeTime)
//  - ����ð� (currentTime)

public class Bullet : MonoBehaviour
{
    // �ӷ� : �Ѿ��� ���ư��� �ӷ�
    public float moveSpeed = 10f;
    // Rigidbody Component 
    private Rigidbody rb;
    // �����ð� (lifeTime)
    public float lifeTime = 10f;
    // ����ð� (currentTime)
    private float currentTime;

    // �Ѿ��� �ε����� �� Effect -- Factory
    public GameObject bulletImpactEffect;

    // Start is called before the first frame update
    private void Awake()
    {
        // Rigidbody ������Ʈ�� �����ͼ� �ʱ�ȭ(init)
        rb = GetComponent<Rigidbody>();
    }
    // �Ѿ��� Ȱ��ȭ �� ������(=����) ���� �缳��
    void OnEnable()
    {
        // Rigidbody�� ���� �Ѿ��� ������ ������.
        // -> AddForce : ������ �ִٰ�, �����̰� ���� ģ���鿡�� ���
        // rb.AddForce(transform.forward * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        // -> velocity : �̹� �����̰� �ִ� ģ����, ������ ģ���鿡�� ���
        // + ���� : deltaTime�� ���� ��� �����ǹǷ�, �Ѿ��� ���ư��� �ӵ��� ����X
        //   �ذ� -> bullet�� Rigidbody�� ������ �����Ƿ� deltaTime�� ����
        rb.velocity = transform.forward * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // �����ð� ���Ŀ�, �Ѿ��� �����Ѵ�.
        // 1. �ð��� �帥��. (�ð��� ���)
        currentTime += Time.deltaTime;
        // 2. ���� ����� �ð��� �����ð��� �Ѿ ���
        if (currentTime > lifeTime)
        {
            // 3. �Ѿ�(=�� �ڽ�)�� źâ�� �ٽ� �ִ´�.
            FindObjectOfType<BulletGun>().DeactiveBullet(gameObject);
        }
    }

    private void OnDisable()
    {
        // LifeTime ����ð� �ʱ�ȭ
        currentTime = 0f;
    }

    // �Ѿ��� ��򰡿� �ε�����,
    private void OnCollisionEnter(Collision collision)
    {
        // + �Ѿ��� ��ġ�� �ε��� Ư��ȿ���� ����Ѵ�.
        FindObjectOfType<BulletGun>().PlayImpactEffect(transform.position);
        // 3. �Ѿ�(=�� �ڽ�)�� źâ�� �ٽ� �ִ´�.
        FindObjectOfType<BulletGun>().DeactiveBullet(gameObject);
    }
}

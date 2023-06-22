using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� : ���� ���ϴ� �������� �Ѿ��� �߻��մϴ�
// �Ѿ� ���� bullet Factory , �ѱ�(�Ѿ� �߻� ��ġ) FirePos, �Ѿ� �߻� ���� FirePos(�ѱ�)�� ���ϴ� ����, ������Է�(���콺 ��)

public class BulletGun : MonoBehaviour
{
    public GameObject bulletFactory;
    public Transform firePos; //������

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButton("Fire1")) // ���콺 ���� ��ư�� ������
        {
            GameObject bullets = Instantiate(bulletFactory);
            bullets.transform.position = firePos.position;
            bullets.transform.forward = firePos.forward;

        }
        // �Ѿ� ����
        // �Ѿ��� �ѱ��� ��ġ ��Ų��.
        // �Ѿ��� ����(Z��)�� �ѱ��� ����(Z��)�� ��ġ��Ų��.
        // �Ѿ��� z�� ���� : bullet�� transform.forward
        // �ѱ��� z�� ���� : firePos�� transform.forward
    }
}

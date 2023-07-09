using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� : ���� ���ϴ� ����, ���� ���ϴ� �������� �Ѿ��� �߻��մϴ�. 
//  - �Ѿ� ���� : bullet Factory
//  - �Ѿ� �߻� ��ġ : Fire Pos
//  - �Ѿ� �߻� ���� : Fire Pos(�ѱ�)�� ���ϴ� ����
//  - ������� �Է� : ���콺 ���� ��ư Ŭ��

// ���� : �Ѿ��� ObjectPooling�� ���� źâ���� ����� �����Ѵ�.
//  - �Ѿ� ���� : bullet Factory
//  - źâ 
//      a. �迭   : źâ�� Original �ʱⰪ(Backup)
//      b. ����Ʈ : ��¥��¥��¥ źâ.
//  - źâ�� ũ�� : �� ���� ���� �Ѿ��� ����

// ���� : ObjectPool�� ����� ���� ���� �Ѿ��� �ı��Ǵ� ���,
//        �ٽ� �� źâ �ȿ� ����ִ´�.

// ���� : �� �߻� ��, Effect���� ����Ѵ�.
//  - Ư��ȿ�� a.�Ѿ��� ��򰡿� �ε����� �� Ư��ȿ�� ���
//  - Ư��ȿ�� b.�Ѿ��� �߻��� �� Ư��ȿ�� ���
//  - �Ѿ� �߻� ��, ȿ���� ���

// ���� : Mode (�ܹ�, ���� ���)
//  - Mode --- �ܹ�, ������
//  - ����(Easy)
//   - �����ð� : 1���� �߻��� �� �ɸ��� �ð�(sec)
//   - ����ð� : 1�� �߻� �� �����ð��� �����ϴ� �ð�
//  - ������� �Է� : ������ ��, ������ �ִ� ����
public class BulletGun : MonoBehaviour
{
    //  - �Ѿ� ���� : bullet Factory
    public GameObject bulletFactory;
    //  - �Ѿ� �߻� ��ġ : Fire Pos
    public Transform firePos;
    //  - źâ a.�迭   : źâ�� Original �ʱⰪ -->> 
    private GameObject[] bulletPool;
    //  - źâ b.����Ʈ : ��¥��¥��¥ źâ. --> 
    private List<GameObject> bulletPoolList = new List<GameObject>();
    //  - źâ�� ũ�� : �� ���� ���� �Ѿ��� ����
    public int bulletSize = 30;

    // ----- �Ѿ��� Effect ------
    // �Ѿ��� �ε����� �� Effect
    public GameObject bulletImpactEffect;
    // �Ѿ��� �߻��� �� Effect 
    public ParticleSystem muzzleFlashEffect;
    // �Ѿ��� �߻��� �� ȿ���� ����
    public AudioSource audios;

    // ----- ���� ��� -----
    public enum Mode
    {
        Pistol, // �ܹ�(����)
        Rifle   // ����(��)
    }
    public Mode mode = Mode.Pistol;
    //   - �����ð� : 1���� �߻��� �� �ɸ��� �ð�(sec)
    public float nextTimeToFire = 1f;
    //   - ����ð� : 1�� �߻� �� �����ð��� �����ϴ� �ð�
    private float currentTime;

    

    private void Start()
    {
        //AudioSource ������Ʈ �����´� (=�ʱ�ȭ)
        AudioSource audio = GetComponent<AudioSource>();
        // Object Pool �� �� ó�� �� ���� ����� �ȴ�.
        BulletObjectPool();
    }

    // Update is called once per frame
    void Update()
    {
        // A. ���� GunMode�� �ܹ�(Pistol) �����?
        if (mode == Mode.Pistol)
        {
            // ����, �Ѿ��� �����ִ� ��쿡�� ������� �Է� : "Fire1" ��ư�� Ŭ������ �� -> �߻�
            if (bulletPoolList.Count > 0 && Input.GetButtonDown("Fire1"))
            {
                Fire();
                // �߻� ��, Effect ���
                PlayMuzzleFlash();
            }
        }
        // B. ���� GunMode�� ����(Rifle) �����?
        if (mode == Mode.Rifle)
        {
            // �Ѿ��� �����ִ� ��쿡��, Fire1��ư�� ������ �����ð����� �Ѿ��� �߻��ϰ� �ʹ�.
            if (bulletPoolList.Count > 0)
            {
                // Fire1 ��ư�� ������ �� -> �Ѿ� �߻�
                if (Input.GetButtonDown("Fire1"))
                {
                    Fire();
                    PlayMuzzleFlash();
                }
                // Fire1 ��ư�� ������ ���� -> nextTimeToFire �ð��� ���߾� ����
                else if (Input.GetButton("Fire1"))
                {
                    currentTime += Time.deltaTime;

                    if (currentTime > nextTimeToFire)
                    {
                        Fire();
                        PlayMuzzleFlash();
                        currentTime = 0f;
                    }
                   

                }
                else if (Input.GetButtonUp("Fire1"))
                {
                    currentTime = 0f;
                }
                // Fire1 ��ư�� ���� �� -> ����ð� Reset


                //  B-1. �ð��� �帥��(����Ѵ�.)
               
            }
        }
    }

    // Bullet ������Ʈ Ǯ�� �����.
    private void BulletObjectPool()
    {
        // 1. ������Ʈ Ǯ�� ũ�⸸ŭ �迭�� ���� ��Ƴ����ϴ�. = źâ ���� �Ѿ� ����
        bulletPool = new GameObject[bulletSize];
        // 2. [�ݺ�]�Ѿ� ���� ��ŭ �Ѿ��� �����ؼ�, ������Ʈ Ǯ�� �ִ´�.
        //  - ������ �Ѿ� ���� : bulletSize
        for (int i = 0; i < bulletSize; i++)
        {
            // 2-1. �Ѿ��� �����Ѵ�.
            GameObject bullet = Instantiate(bulletFactory);
            // 2-2. �Ѿ��� ��(=Original źâ)�� �ִ´�.
            bulletPool[i] = bullet;
            // 2-3. �濡 ���� �Ѿ��� ��Ȱ��ȭ ���·� �����.
            bulletPool[i].gameObject.SetActive(false);
            // 2-4. �Ѿ��� ��¥��¥��¥ źâ�� �ִ´�.
            bulletPoolList.Add(bulletPool[i]);

            // + �Ѿ��� ���ư����� źâ�� �Բ� �˷��ش�.
            bulletPool[i].GetComponent<Bullet>().InitMyGun(this); //BulletGun �߿��������Ʈ
        }
    }


    // �Ѿ� �߻� ---> źâ�� �ִ� �Ѿ��� ����� �߻��Ѵ�.
    private void Fire()
    {
        // 1. ��¥��¥��¥ źâ�� ù ��° �Ѿ��� �����ɴϴ�.
        GameObject bullet = bulletPoolList[0];
        // 2. �Ѿ��� �ѱ��� ��ġ��ŵ�ϴ�.
        bullet.transform.position = firePos.position;
        // 3. �Ѿ��� ������ �ѱ��� ����� ��ġ��ŵ�ϴ�.
        bullet.transform.forward = firePos.forward;
        // 4. �Ѿ��� Ȱ��ȭ ���·� �����.
        bullet.SetActive(true);
        // 5. źâ���� ����� (ù ��°)�Ѿ��� �����Ѵ�.
        bulletPoolList.RemoveAt(0);
        // 6. �Ѿ� �߻� ���� ���
        audios.Stop();
        audios.Play();

    }

    // �Ѿ� �߻� ---> �Ѿ��� ���� �����ؼ� �߻��Ѵ�.
    private void LegacyFire()
    {
        // 2. �Ѿ��� �����Ѵ�.
        GameObject bullet = Instantiate(bulletFactory);
        // 3. �Ѿ��� �ѱ��� ��ġ��Ų��.
        bullet.transform.position = firePos.position;
        // 4. �Ѿ��� ����(Z��)�� �ѱ��� ����(Z��)�� ��ġ��Ų��.
        //  - �Ѿ��� z�� ���� : Bullet�� transform.forward
        //  - �ѱ��� z�� ���� : firePos�� .forward
        bullet.transform.forward = firePos.forward;
    }

    // źâ�� �? �Ѿ��� �ٽ� �߰��Ѵ�.
    // - ���� ���� �Ű�ü : Bullet (�ε����ų�, lifetime�� �� �� �Ѿ�)
    public void DeactiveBullet(GameObject bullet)
    {
        // 1. �޾ƿ� �Ѿ��� ��Ȱ��ȭ ���·� �����Ѵ�.
        bullet.SetActive(false);
        // 2. ��Ȱ��ȭ ������ �Ѿ��� źâ�� �ٽ� �ִ´�.
        bulletPoolList.Add(bullet);
    }

    // �Ѿ��� �ε����� ���� Effect ����Ѵ�.
    // - �Ѿ��� �ε��� ��ġ
    // - Effect (�ε����� �� ����� Ư��ȿ��)
    public void PlayImpactEffect(Vector3 impactPos)
    {
        // 1. ��ƼŬ�� �Ѿ��� �ε��� ���� ��ġ��Ų��.
        bulletImpactEffect.transform.position = impactPos;
        // 2. ��ƼŬ�� ��� (Stop & Play)
        ParticleSystem ps = bulletImpactEffect.GetComponent<ParticleSystem>();
        ps.Stop();
        ps.Play();
    }

    // �Ѿ��� �߻��� �� Effect�� ����Ѵ�.
    // - �Ѿ� �߻� Effect
    public void PlayMuzzleFlash()
    {
        // �Ѿ� �߻� Effect�� ��� (Stop & Play)
        muzzleFlashEffect.Stop();
        muzzleFlashEffect.Play();
    }
}

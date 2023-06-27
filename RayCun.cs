using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� : ������ �ʴ� �Ѿ��� �߻��ϰ� �ʹ�
// ������ �ʴ� �Ѿ� : Ray(����� �Ÿ��� ���� ��) , RayCast(���� �߻�), RayCasHit(�߻��� ���� �ε��� ���� ����)
// �Ѿ��� �ε����� �� ȿ��, �ѱ�,�Ѿ� �߻� ��ġ(�ѱ�)

// [����] ȭ���� ��� �������� �ѱ����� �߻�Ǵ� �Ѿ� ��ġ�� ���� ����
// [�ذ�] �ѱ��� ��ġ���� ī�޶� �ٶ󺸴� ��������  Ray�� �߻�
// + ������ �ʴ� �Ѿ��� Ư�� ��ü�� �浹���� �ʵ��� ����� �ʹ�. (layer)

// ���� : Mode�� �����ؼ� �ܹ�/���� ���
// Mode (pistol, rifle) , ���� �߻� �ð�, �߻� ��Ÿ��
public class RayCun : MonoBehaviour
{
    public ParticleSystem impactEffect; //ȿ��
    public Transform firePos; //�ѱ�
    public AudioSource impactaudio;

    private Camera mainCam; //����ȭ

    public enum Mode { Pistol, Rifle }
    public Mode mode = Mode.Pistol;
    private float nextTimeToFire; // ���� �߻� �ð�
    public float fireRate = 10f; //������
    private Vector3 randomMousePos; //�ݵ� ����

    // Start is called before the first frame update
    void Start()
    {
        impactaudio = GetComponent<AudioSource>();
        mainCam = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // A. �ܹ� (Pistol ���)
        if (mode == Mode.Pistol)
        {
            //fire1 ��ư ������ �߻�
            if (Input.GetButtonDown("Fire1")) { Shoot(); }
        }

        // B. ���� (Rifle ���)
        if (mode == Mode.Rifle)
        {
            //Fire1 ��ư�� ������ �ִ� ���� �� �߻�
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                //���� �߻� �ð� ��� = ���� �߻� �ð� ���� + �߻���Ÿ��(������)
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        
    }

    //Ray ������ �ʴ� �Ѿ� �߻�
    void Shoot()
    {
        // A. �ܹ� ��� ��, Ray�� ��ġ�� ���콺 Ŀ�� ��ġ
        if (mode == Mode.Pistol)
        {
            randomMousePos = Input.mousePosition;
        }
        // B. ���� ��� ��, Ray�� ��ġ�� Random ���콺 Ŀ�� ��ġ
        if (mode == Mode.Rifle)
        {
            //�������� R�� ���� ���� ȭ�� ��� ���� ���� ��� ����Ʈ ��ġ ����
            randomMousePos = Input.mousePosition + Random.insideUnitSphere * 30f; //radius
            //z ���� 2���� ��� Screen���� �ʿ� ���� ������ 0 ���� Reset
            randomMousePos.z = 0f;
        }
        

        // 1. Ray ���� : ��ġ(firePos), ����(firePos�� Z��, Camera�� �ٶ󺸴� ���߾��� ����) , �Ÿ�
        //Vector3 direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        Ray ray = mainCam.ScreenPointToRay(randomMousePos);
        //Ray ray = new Ray(firePos.position, direction);
        // 2. RaycastHit ���� : �ε��� �� ����
        RaycastHit hitInfo;
        // 3. Raycast ... (1)������ ������ Ray�� �߻�
        int layer = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Weapon");
        // 4. �ε������� �� �ε������� �浹üũ
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~layer)) //(��, �ε�����������, �Ÿ�);
        {
             // 5. ImpactEffect�� �ε��� ���� ��ġ
            impactEffect.transform.position = hitInfo.point;
            // 5-1. �ε��� ������ �������� Effect �� ������ ȸ�������ش�. 
            impactEffect.transform.forward = hitInfo.normal;
            // 6. ImpactEffect ȿ�� ���
            impactEffect.Stop();
            impactEffect.Play();

           
        }
        impactaudio.Stop();
        impactaudio.Play();
    }
}

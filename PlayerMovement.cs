using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//���� : player�� �������� ���������� �����Ѵ�. -> ĳ������ �������� CharacterController ������Ʈ�� ���� ����
// A. player�� ��/��/��/��� ������ �ӷ����� �����δ�.
//������� �Է� (a. �յ�/ b. �¿�), ������ �ӷ�

// B. player�� ������� �Է¿� ���� ��/��� ȸ����Ų��.
// ������� �Է� (���콺 ��-��), ȸ�� �ӷ� (�ΰ���), ������� �Է¿� ���� ȸ�� ���� ����

// C. player �� ������� �Է¿� ���� ��/�Ϸ� ȸ����Ų��.
// ������� �Է� (���콺 ��-��), ȸ�� �ӷ� (B���� ����� ����), ������� �Է¿� ���� ȸ�� ������ ����, ī�޶� ȸ��, ���� ȸ�� ���� ����

// D.�ڿ������� �߷�
// ChracterController ������Ʈ, �߷� ���ӵ�, �߷� ������ (�߷°��ӵ��� ���� ������ ������ �߷°�)

// E. ������� �Է¿� ���� Player ����
// ������� �Է�(Jump), �Ŀ�, �ִ���������Ƚ��, ��������Ƚ��

//player�� �̵���Ű�� �Լ�
// [����] : palyer�� �ٶ󺸴� ������ �ƴ� unity ���� ������ �̵��Ѵ�. (�۷ι�)
// [�ذ�] : palyer�� �ٶ󺸴� �������� �̵��Ѵ�. (����)

public class PlayerMovement : MonoBehaviour
{
    #region ��������
    public float speed = 5f; //�̵��ӷ�
    public float mouseSensitivity = 500f; //ȸ�� �ӷ�(�ΰ���)
    private float horizontalAngle; //������� �Է¿� ���� ��-�� ȸ�� ������ ����
    private float verticalAngle; //������� �Է¿� ���� ��-�� ȸ�� ���� ����
    public float limitAngle = 30; //ȸ�� ���� ����
    
    private CharacterController cc; //ĳ���� ��Ʈ�ѷ� 
    public float gravity = -9.81f; //�߷� ���ӵ� (9.81m/s)
    private float yVelocity; 

    public GameObject face; // ī�޶�

    public float jumpPower = 4.0f; 
    public bool isJump = false;
    public int jumpCount =1;
    private int jumping;
    #endregion
    
    void Start()
    {
        // Ŀ���� ������ �ʰ� �ϰ�, ȭ�� ����� ������Ų��. //esc������ ���콺 ����, Ŭ���ϸ� �����
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>(); //�ʱ�ȭ

    }

    void Update()
    {

        MovePlayer();
        RotateHorizontal();
        RotateVertical();
    }

    
    void MovePlayer() //player�� �̵���Ű�� �Լ�
    {
        //1. player ������� �Է¿� ����
        float vertical = Input.GetAxis("Vertical");// a. ��-��
        float horizontal = Input.GetAxis("Horizontal");// b ��-��
        //Vector3 direction = new Vector3(horizontal, 0, vertical);//2. ����� �Է¿� ���� ������ ���Ѵ� //3. Ư�� �������� ������ �ӷ����� �����δ�.                                            
        //2-1. ����� �Է¿� ���� local�� ������ ���Ѵ�.(���� ���� �������� ������)
        Vector3 direction = transform.forward * vertical + transform.right * horizontal;

        //[����] �߷��� ��� ���� //[�ذ�] ���� ������ �پ����� �� ������ ����Ǵ� �߷��� reset
        /*if (cc.collisionFlags == CollisionFlags.Below) // ���� �浹 ���� == ���� �浹�� ����
        {
            //�߷°��� 0���� Reset
            yVelocity = 0;
        }*/
        if (cc.isGrounded == true) //�� ������ �ڵ�
        {
            yVelocity = 0f;
            isJump = false;
            jumping = jumpCount;
        }

        #region ���� ����
        // 1. isJump�� ���¸� üũ�մϴ�.
        // 1-1. (��) isJump�� false�� ������ ������ ���°� �˴ϴ�. -> false�� ���¿��� jump ��ư�� ������ �����մϴ�.
        // 2. (����) jump ��ư�� ���� ���� isjump�� true ���°� �˴ϴ�. -> jump ��ư�� ������ ���̻� ������ ���� �ʽ��ϴ�.
        // 3. (���� -> ��) jump �ߴ� �ٽ� ���� ������ isJump�� false ���°� �˴ϴ�.
        #endregion

        // player�� jump ��ư ������ ����
       /* if (Input.GetButtonDown("Jump"))
        {
            if (isJump == false)
            {
                if (jumping > 0)
                {
                    //JumpPower��ŭ �ڱ�ģ��.
                    yVelocity = jumpPower;
                    jumping--;
                    isJump = false;
                } else if (jumping <= 0)
                {
                    isJump = true;
                }
            } 
            
        }*/

       if (jumping > 0 && Input.GetButtonDown("Jump"))
        {
            if (!isJump)
            {
                yVelocity = jumpPower;
                jumping--;
            }
            
        }


        //�߷�(F) ����
        // 1. F(�߷�) = m(����=1)*a(�߷°��ӵ�=gravity)
        // 2. V(�ӵ�) = V0(�߷´�����) + a(�߷°��ӵ�)t
        yVelocity += gravity * Time.deltaTime;
        // �߷��� �޵��� Player�� Y�� ������ �����Ѵ�.
        direction.y = yVelocity;

        //transform.position += direction * speed * Time.deltaTime; //(���� * �ӷ�)//p=p0+vt //�浹üũ X
        cc.Move(direction * speed * Time.deltaTime); //CharacterController�� ���� ������ //�浹üũO
    }

    void RotateHorizontal()
    {
        //1. ������� �Է°� (���콺)
        // a. ��-�� �̵���
        float horizontal = Input.GetAxis("Mouse X");
        //2. ���콺 �ΰ����� ���� �Է°��� ����
        float turnPlayer = horizontal * mouseSensitivity *Time.deltaTime;
        //3. ���콺 �̵��� ���� ��-�� ȸ�� ����.
        horizontalAngle += turnPlayer;
        //4. ������ ȸ�� ���� player�� ȸ���� �ݿ� (Y�� ȸ��)
        transform.localEulerAngles = new Vector3(0, horizontalAngle, 0);
    }

    
    void RotateVertical()
    {
        // 1. ������� �Է°� (���콺) // a. ��-�� �̵��� (-1 ~ +1)
        float vertical = Input.GetAxis("Mouse Y");
        // 2. ���콺 �ΰ����� ���� �Է°��� ����
        float turnFace = vertical * mouseSensitivity *Time.deltaTime;
        // 3. ���콺 �̵��� ���� ��-�� ȸ���� ����
        verticalAngle += turnFace;

        //[����] : 360�� ȸ�� //[�ذ�]: ���� ȸ�� ���� ����
        // VerticalAngle�� ���� + limitAngle���� ũ�� -> verticalAngle == limitAngle 
        // VerticalAngle�� ���� - limitAngle���� �۴ٸ� -> verticalAngle == -limitAngle
        verticalAngle = Mathf.Clamp(verticalAngle, -limitAngle, limitAngle);  
        /* mathf.clamp�� ���� �� 
         if (verticalAngle > limitAngle)
        {
            verticalAngle = limitAngle;
        } else if (verticalAngle < -limitAngle)
        {
            verticalAngle =  -limitAngle;
        } */

        // 4. ������ ȸ�� ���� Face(=Camera)�� ȸ���� �ݿ� (X�� ȸ��)
        face.transform.localEulerAngles = new Vector3(-verticalAngle, 0, 0);

        

    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{

    //�ִϸ��̼� ���� �� �÷��̾� ���� 
    public float moveSpeed;
    float hAxis;
    float vAxis;
    Vector3 movevec;
    Vector3 Dodgevec;
    public int JumpPower;
    private Rigidbody rb;
    Animator anim;
    public float rayLength;
    public GameObject playerEquipPoint;
    public float throwPwer;

    bool jDown;
    bool isJump;
    bool isDodge;
    bool isAimig;


    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        GetInput();
        PMove();
        Jump();
        Aim();


    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jDown = Input.GetButton("Jump");
    }

    void PMove()
    {
        movevec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
        {
            movevec = Dodgevec;
        }
        else
            transform.position += movevec * Time.deltaTime * moveSpeed;


        transform.LookAt(transform.position + movevec);
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
        }
        
           
    }


    void Aim()
    {
        if (!isAimig)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        int floorMast = LayerMask.GetMask("floor");

        if (Physics.Raycast(ray, out rayHit, rayLength, floorMast))
{
            Debug.DrawRay(transform.position, Vector3.forward * 1000f, Color.red);
            Vector3 playerToMouse = rayHit.point - playerEquipPoint.transform.position;
            playerToMouse.y = 0f;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            rb.MoveRotation(newRotation);

        }
    }

    void SetEquip(GameObject item, bool isEquip)
    {
        //������ ���� �Լ� �Ϻ�.... ���� ��ƴ�

    }
    void Drop()
    {
        GameObject item = playerEquipPoint.GetComponentInChildren<Rigidbody>().gameObject;
        Rigidbody rb = item.GetComponent<Rigidbody>();

        SetEquip(item, false);
        playerEquipPoint.transform.DetachChildren();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        float rayLength = 500f;
        int floorMask = LayerMask.GetMask("Floor");
        Vector3 throwAngle;

        if (Physics.Raycast(ray, out rayHit, rayLength, floorMask))
        { 
            throwAngle = rayHit.point - playerEquipPoint.transform.position;
        }
        else
        {
            throwAngle = transform.forward * 50f;

        }
        
        throwAngle.y = 25f;
        rb.AddForce(throwAngle * throwPwer, ForceMode.Impulse);

        isAimig = false;


    }

    }
   

  
    


 





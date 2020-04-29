using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    Idle, Walk, Run, Roll, Cast, Throw, Hit
}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public GameObject tempEndPanel;

    private CharacterController playerCc;
    private Animator playerAni;
    private AudioSource playerAudioSource;

    public AudioClip throwSound;
    public AudioClip castSound;
    public AudioClip rollSound;

    [Space]
    public PlayerState playerstate = PlayerState.Idle;
    public GameObject hitEffect;

    public Image HpSlider;
    public float maxHp = 100;
    [HideInInspector]
    public float currentHp;

    [Space]
    public float gravity = -9.81f;
    public float moveSpeed = 1;
    public float additionMoveSpeed = 2;
    private float reduceMoveSpeed = 1;
    private float AddMoveSpeed = 1;
    public float rollSpeed = 1;
    public float rotateSpeed = 0.1f;

    [Space]
    public float pushedAmount = 1;
    public float pushedSpeed = 1;
    private float currentPushedAmount;

    [Space]
    private float currentRollSpeed = 0;
    private float currentGravity = 0;

    private float rotateAmount = 0;

    private GameObject followCamera;

    [Space]
    public Transform holdPosition;
    public GameObject holdItem;
    private GameObject currentHoldItem;
    private bool isHold = false;
    private bool isAim = false;

    private bool isOverlap = false;

    void Start()
    {
        playerCc = GetComponent<CharacterController>();
        playerAni = GetComponent<Animator>();
        playerAudioSource = GetComponent<AudioSource>();

        followCamera = Camera.main.gameObject;

        currentHp = maxHp;
    }

    void FixedUpdate()
    {
        HpSlider.fillAmount = currentHp / maxHp;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotateAmount, 0), rotateSpeed);

        PlayerMove();
        PlayerAct();
    }

    void PlayerMove()
    {
        if (playerstate.Equals(PlayerState.Roll) || playerstate.Equals(PlayerState.Cast) || playerstate.Equals(PlayerState.Throw) || playerstate.Equals(PlayerState.Hit))
        {
            playerAni.SetBool("IsWalk", false);
            return;
        }

        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        currentGravity += gravity * 0.1f;

        if (playerCc.isGrounded)
            currentGravity = 0;

        rotateAmount = followCamera.transform.eulerAngles.y;

        if (Input.GetKey(KeyCode.S))
            reduceMoveSpeed = additionMoveSpeed;
        else
            reduceMoveSpeed = 1f;


        if (Input.GetKey(KeyCode.LeftShift))
            AddMoveSpeed = additionMoveSpeed;
        else
            AddMoveSpeed = 1f;

        Vector3 movePosition = transform.rotation * new Vector3(xAxis, currentGravity, zAxis);
        playerCc.Move(movePosition * (moveSpeed * AddMoveSpeed / reduceMoveSpeed) * Time.deltaTime);


        if (xAxis * xAxis + zAxis * zAxis > 0)
        {
            playerAni.SetBool("IsWalk", true);
            playerstate = PlayerState.Walk;
        }
        else
        {
            playerAni.SetBool("IsWalk", false);
            playerstate = PlayerState.Idle;
        }


        if ((AddMoveSpeed / reduceMoveSpeed) > 1)
        {
            playerAni.SetBool("IsRun", true);
            playerstate = PlayerState.Run;
        }
        else
        {
            playerAni.SetBool("IsRun", false);
            playerstate = PlayerState.Idle;
        }

    }

    void PlayerAct()
    {
        if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
            {
                playerstate = PlayerState.Idle;
            }
            else if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
            {
                playerCc.Move(transform.forward * currentRollSpeed * Time.deltaTime);
                currentRollSpeed *= 0.95f;
            }
            return;
        }

        else if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("Cast"))
        {
            if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f && !isOverlap)
            {
                isOverlap = true;

                playerstate = PlayerState.Idle;
                playerAni.SetBool("IsHold", true);
                isHold = true;

                currentHoldItem = Instantiate(holdItem);

                currentHoldItem.transform.SetParent(holdPosition);
                currentHoldItem.transform.localPosition = new Vector3(0.15f, -0.015f, 0.115f);
                currentHoldItem.transform.localRotation = Quaternion.Euler(35f, 110f, 80f);
            }
            return;
        }

        else if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("Throw"))
        {
            if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
            {
                FindObjectOfType<FollowCamera>().isCameraAim = false;
                playerstate = PlayerState.Idle;
                playerAni.SetBool("IsHold", false);
                isHold = false;
            }
            else if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35f && !isOverlap)
            {
                isOverlap = true;
                currentHoldItem.transform.SetParent(null);

                currentHoldItem.transform.rotation = transform.rotation * Quaternion.Euler(90f, 0f, 0f);

                Rigidbody rb = currentHoldItem.GetComponent<Rigidbody>();
                rb.isKinematic = false;

                playerAudioSource.PlayOneShot(throwSound);

                rb.AddForce(transform.forward * 1000);
                rb.AddForce(transform.up * 250);

                Destroy(currentHoldItem, 7.0f);
            }
            return;
        }
        else if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
            {
                if (currentHoldItem == null)
                {
                    FindObjectOfType<FollowCamera>().isCameraAim = false;
                    playerAni.SetBool("IsHold", false);
                    isHold = false;
                }
                playerstate = PlayerState.Idle;
            }
            return;
        }

        isOverlap = false;



        if (playerstate.Equals(PlayerState.Roll) || playerstate.Equals(PlayerState.Cast) || playerstate.Equals(PlayerState.Throw) || playerstate.Equals(PlayerState.Hit))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isAim)
        {
            if (Input.GetKey(KeyCode.A))
            {
                PlayerRoll(-1);
            }
            if (Input.GetKey(KeyCode.D))
            {
                PlayerRoll(1);
            }
        }

        if (!(playerstate.Equals(PlayerState.Run)) && Input.GetMouseButtonDown(0) && !isHold)
        {
            PlayerCast();
        }
        if (!(playerstate.Equals(PlayerState.Run)) && Input.GetMouseButton(1) && isHold)
        {
            playerAni.SetBool("IsAim", true);
            isAim = true;

            if (Input.GetMouseButtonDown(0))
            {
                playerAni.SetBool("IsAim", false);
                isAim = false;

                playerAni.SetTrigger("Throw");
                playerstate = PlayerState.Throw;
                return;
            }
            FindObjectOfType<FollowCamera>().isCameraAim = true;
        }
        else if (isHold)
        {
            playerAni.SetBool("IsAim", false);
            isAim = false;

            FindObjectOfType<FollowCamera>().isCameraAim = false;
        }
    }

    void PlayerRoll(int direction)
    {
        playerstate = PlayerState.Roll;
        currentRollSpeed = rollSpeed;

        rotateAmount = transform.eulerAngles.y + 90 * direction;

        playerAni.SetTrigger("Roll");
        playerAudioSource.PlayOneShot(rollSound);
    }
    void PlayerCast()
    {
        playerstate = PlayerState.Cast;
        playerAni.SetTrigger("Cast");
        playerAudioSource.PlayOneShot(castSound);
    }

    public IEnumerator PlayerDamaged()
    {
        StartCoroutine(FindObjectOfType<FollowCamera>().Shake(0.25f, 0.5f));
        GameObject effect = Instantiate(hitEffect, transform.position + Vector3.up, hitEffect.transform.rotation);
        Destroy(effect, 1f);

        playerAni.SetTrigger("Hit");
        playerstate = PlayerState.Hit;
        currentHp -= 10;
        currentPushedAmount = pushedAmount;

        if (currentHp <= 0)
        {
            tempEndPanel.SetActive(true);
        }

        while (currentPushedAmount >= 0)
        {
            currentPushedAmount -= pushedSpeed * Time.deltaTime;
            playerCc.Move(-transform.forward * currentPushedAmount * Time.deltaTime);
            yield return null;
        }
    }
}

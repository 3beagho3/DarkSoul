using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject targetPlayer;

    public Vector3 offset;
    public float followTime = 0.1f;


    private float horizontal = 0;
    private float vertical = 0;
    public float xRotateLimt = 90;
    public float yRotateLimt = 90;

    private bool IsShake = false;

    private Vector3 firstPos, lastPos;

    private Vector3 originPos;

    [HideInInspector]
    public bool isCameraAim = false;

    void OnValidate()
    {
        CameraMove();
    }

    void Start()
    {
        if (targetPlayer == null)
            targetPlayer = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate()
    {
        if(isCameraAim)
        {
            CameraAim();
            return;
        }

        CameraMove();
        CameraRotate();
    }
    
    void CameraAim()
    {
        transform.position = Vector3.Lerp(transform.position, targetPlayer.transform.position + targetPlayer.transform.rotation * new Vector3(0.5f , 1.5f , -1), followTime);
    }
    void CameraMove()
    {
        horizontal += Input.GetAxis("Mouse X") * 0.1f;

        float x = Mathf.Sin(horizontal);
        float z = Mathf.Cos(horizontal);

        Vector3 moveRect = new Vector3(x, 0, z) * offset.z;

        transform.position = Vector3.Lerp(transform.position, targetPlayer.transform.position + (Vector3.up * offset.y), followTime) + moveRect;
    }

    void CameraRotate()
    {
        vertical -= Input.GetAxis("Mouse Y");

        vertical = Mathf.Clamp(vertical, -yRotateLimt, yRotateLimt);

        Vector3 lookRotation = targetPlayer.transform.position - transform.position;

        float lookHorizontal = Mathf.Atan2(lookRotation.x, lookRotation.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(vertical, lookHorizontal, 0);
    }

    public IEnumerator Shake(float ShakeDuration , float ShakeAmount)
    {
        float timer = 0;
        if (!IsShake)
        {
            IsShake = true;
            originPos = transform.localPosition;
            while (timer <= ShakeDuration)
            {
                transform.localPosition = (Vector3)Random.insideUnitCircle * ShakeAmount + originPos;
                timer += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = originPos;
            IsShake = false;
        }
    }
}

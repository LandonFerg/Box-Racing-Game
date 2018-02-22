using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    public Rigidbody[] rb;
    public CapsuleCollider playerColl;
    public GameObject cam;
    public WheelDrive drive;
    public Driver CC;
    public GameObject carRoot;
    public Transform carPrefab;
    public bool dead;
    public bool collided;
    public UnityStandardAssets.Cameras.AutoCam camScript;
    //public AutoCam camScript;
    void Start()
    {
       // carRoot.SetActive(true);
        dead = false;   // Alive at start

        rb = GetComponentsInChildren<Rigidbody>();
        camScript = cam.GetComponent<UnityStandardAssets.Cameras.AutoCam>();
        for (int i = 0; i < rb.Length; i++)
        {
            var rigidbodies = rb[i];

            rigidbodies.isKinematic = true;
            rigidbodies.detectCollisions = false;
            playerColl.enabled = true;
            camScript.enabled = true;
            CC.enabled = true;
            drive.enabled = true;
        }

    }


    void OnTriggerEnter(Collider dataFromCollision)
    {
        if (dataFromCollision.gameObject.tag == "ground")
        {
            StartCoroutine(Reset());
            rb = GetComponentsInChildren<Rigidbody>();
            Debug.Log("COLLIDING");

            for (int i = 0; i < rb.Length; i++)
            {
                var rigidbodies = rb[i];

                rigidbodies.isKinematic = false;
                rigidbodies.detectCollisions = true;
                playerColl.enabled = false;
                camScript.enabled = false;
                CC.enabled = false;
                drive.enabled = false;
                dead = true;
                collided = true;    // debug
            }
        }
        else { }

    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("RESPAWNING");

        Instantiate(carPrefab, new Vector3(10.4f, 244, -1060), Quaternion.identity);
        rb = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rb.Length; i++)
        {
            var rigidbodies = rb[i];

            rigidbodies.isKinematic = true;
            rigidbodies.detectCollisions = false;
        }
        carRoot.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMove : MonoBehaviour
{
    [SerializeField] private float MoveSpeed;
    [SerializeField] bool isRandomMove;

    void Start()
    {
        RandomPos();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * MoveSpeed * Time.deltaTime);

        if(transform.position.x < -25)
        {
            Vector3 newPos = transform.position;
            newPos.x = 25;
            transform.position = newPos;
            RandomPos();
        }

    }

    void RandomPos()
    {
        if(isRandomMove)
        {
            transform.position = new Vector3(transform.position.x, 2, transform.position.z);

            //¾à°£ Y·£´ý
            float yPos = Random.Range(-1.0f, 0.75f);

            transform.position += new Vector3(0, 0 + yPos, 0);
        }


    }
}

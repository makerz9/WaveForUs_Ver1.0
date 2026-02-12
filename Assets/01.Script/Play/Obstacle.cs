using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private int MoveSpeed;

    void Start()
    {
        //¾à°£ Y·£´ý
        float yPos = Random.Range(-2, 4);

        transform.position += new Vector3(0, 0 + yPos, 0);

        Invoke(nameof(DeactivateObject), 20f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * MoveSpeed * Time.deltaTime);
    }

    void DeactivateObject()
    {
        gameObject.SetActive(false);
    }
}

using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Vector2 spawnPosition;
    [SerializeField] private GameObject[] WaveUpCreate = new GameObject[2];
    private BoxCollider2D boxCollider;

    [SerializeField] private Sprite[] StoneImage;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //int sprite = Random.Range(0, StoneImage.Length);
        //gameObject.GetComponent<SpriteRenderer>().sprite = StoneImage[sprite];

        GetComponent<SpriteRenderer>().sprite = StoneImage[Random.Range(0, StoneImage.Length)];
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 350f));

        Invoke(nameof(GameObjectDisable), 10f);
    }

    public void SetSpawnPosition(Vector2 pos)
    {
        spawnPosition = pos;
        Debug.Log($"돌이 생성된 위치 저장: {spawnPosition.y}");
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {

            foreach (GameObject WavePrefab in WaveUpCreate)
            {
                GameObject wave = Instantiate(WavePrefab, transform.position, Quaternion.identity);
                WaveMakerMove waveScript = wave.GetComponent<WaveMakerMove>();

                if (waveScript != null)
                {
                    waveScript.SetPosPower(spawnPosition.y);
                }

            }

            Debug.Log($"물에 닿음! 생성 위치는 {spawnPosition.y}");

            int soundIndex = Random.Range(0, 2);

            if (soundIndex == 0)
            {
                GameManager.Instance.SoundCall("waterDrop1");
            }
            else if (soundIndex == 1)
            {
                GameManager.Instance.SoundCall("waterDrop2");
            }

            int waveSoundIndex = Random.Range(0, 3);
            
            if (waveSoundIndex == 0)
            {
                GameManager.Instance.SoundCall("wave1");
            }
            else if (waveSoundIndex == 1)
            {
                GameManager.Instance.SoundCall("wave2");
            }
            else if (waveSoundIndex == 2)
            {
                GameManager.Instance.SoundCall("wave3");
            }

            //rb.AddForce(Vector2.up * 500, ForceMode2D.Force);
            boxCollider.enabled = false;



            //rb.drag = 8;
            //GetComponent<Rigidbody2D>().gravityScale = 0.1f;
        }
    }

    void GameObjectDisable()
    {
        gameObject.SetActive(false);
    }

}

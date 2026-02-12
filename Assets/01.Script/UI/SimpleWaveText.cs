using UnityEngine;
using TMPro;

public class SimpleWaveText : MonoBehaviour
{
    private TMP_Text tmp;

    [SerializeField] string txtType;


    void Start()
    {
        tmp = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if(txtType == "Title")
        {
            tmp.ForceMeshUpdate();
            var textInfo = tmp.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertIndex = charInfo.vertexIndex;
                int matIndex = charInfo.materialReferenceIndex;
                var verts = textInfo.meshInfo[matIndex].vertices;

                float wave = Mathf.Sin(Time.time * 4f + i * 0.3f) * 7f;

                verts[vertIndex + 0].y += wave;
                verts[vertIndex + 1].y += wave;
                verts[vertIndex + 2].y += wave;
                verts[vertIndex + 3].y += wave;


            }

            tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
        else if(txtType == "GameOver")
        {
            tmp.ForceMeshUpdate();
            var textInfo = tmp.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertIndex = charInfo.vertexIndex;
                int matIndex = charInfo.materialReferenceIndex;
                var verts = textInfo.meshInfo[matIndex].vertices;


                float wave = Mathf.Sin(Time.time * 3f + i * 0.3f) * 5f;
                float wobble = Mathf.Cos(Time.time * 5f + i * 0.2f) * 2f;

                verts[vertIndex + 0] += new Vector3(wobble, wave, 0);
                verts[vertIndex + 1] += new Vector3(wobble, wave, 0);
                verts[vertIndex + 2] += new Vector3(wobble, wave, 0);
                verts[vertIndex + 3] += new Vector3(wobble, wave, 0);
            }

            tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }







    }
}
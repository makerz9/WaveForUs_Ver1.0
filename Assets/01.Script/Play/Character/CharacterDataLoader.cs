using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDataLoader : MonoBehaviour
{
    public static CharacterData LoadByName(string name) //??
    {
        // Resources/Data 폴더에 CSV 파일 있어야 함
        TextAsset csv = Resources.Load<TextAsset>("Data/LOOPWARE_Stats"); //??
        string[] lines = csv.text.Split('\n'); //??

        // 0번째 줄은 헤더라서 1번부터 읽기
        for (int i = 1; i < lines.Length; i++) //첫줄에 헤더 써서 그런듯
        {
            string[] col = lines[i].Split(','); //?
            if (col[0].Trim() == name) //?
            {
                CharacterData data = new CharacterData(); //생성자선언?
                data.characterName = col[0].Trim(); //?? data에 있는거 가져온건데 col[0].Trim(), int.Parse(col[1]);이건 뭐지??
                data.level = int.Parse(col[1]);
                data.maxHp = float.Parse(col[2]);
                data.hp = float.Parse(col[3]);
                data.hpRegen = float.Parse(col[4]);
                data.attackPower = float.Parse(col[5]);
                data.moveSpeed = float.Parse(col[6]);
                data.defense = float.Parse(col[7]);
                data.criticalChance = float.Parse(col[8]);
                data.criticalDamage = float.Parse(col[9]);
                return data;
            }

        }

        Debug.LogError($"{name} 캐릭터 데이터를 찾을 수 없습니다.");
        return null;

    }





}

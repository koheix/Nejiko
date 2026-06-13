using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    const int StageChipSize = 30;

    int currentChipIndex;

    public Transform character;
    public GameObject[] stageChips;
    public int startChipIndex;
    public int preInstantiate;
    public List<GameObject> generatedStageList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        currentChipIndex = startChipIndex - 1;
        UpdateStage(preInstantiate);
        
    }

    // Update is called once per frame
    void Update()
    {
        int charaPositionIndex = (int)(character.position.z / StageChipSize);

        if(charaPositionIndex + preInstantiate > currentChipIndex){
            UpdateStage(charaPositionIndex + preInstantiate);
        }
        
    }

    void UpdateStage(int toChipIndex){
        //もし生成する個数を満たしていれば終了
        if(toChipIndex <= currentChipIndex) return;

        for(int i = currentChipIndex + 1; i <= toChipIndex; i++){
            GameObject stageObject = GenerateStage(i);

            generatedStageList.Add(stageObject);
        }

        while(generatedStageList.Count > preInstantiate + 2) DestroyOlddestStage();

        currentChipIndex = toChipIndex;
    }

    GameObject GenerateStage(int chipIndex){
        int nextStageChip = Random.Range(0, stageChips.Length);

        GameObject stageObject = (GameObject)Instantiate(
            stageChips[nextStageChip],
            new Vector3(0, 0, chipIndex * StageChipSize),
            Quaternion.identity
        );

        return stageObject;
    }

    void DestroyOlddestStage(){
        GameObject oldStage = generatedStageList[0];
        generatedStageList.RemoveAt(0);
        Destroy(oldStage);
    }
}

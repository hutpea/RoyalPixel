using BizzyBeeGames.ColorByNumbers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevel : MonoBehaviour
{
    public LoadDataCate dataAreas;
    public LoadDataPic dataPic;

    public List<PictureInformation> pics;

    public GameObject gamePlayControllerPrefab;

    public void Inits()
    {
        pics = new List<PictureInformation>();
        for (int i = 0; i < dataAreas.cateItems.Count; i++)
        {

            foreach (var arena in dataAreas.cateItems[i].dataPic.CategoryInfos)
            {
                for (int j = 0; j < arena.Value.pictureFiles.Count; j++)
                {
                    pics.Add(new PictureInformation(arena.Value.pictureFiles[j].ToString()));
                }
            }
        }

        foreach (var arena in dataPic.CategoryInfos)
        {
            for (int j = 0; j < arena.Value.pictureFiles.Count; j++)
            {
                pics.Add(new PictureInformation(arena.Value.pictureFiles[j].ToString()));
            }
        }

        StartCoroutine(RunTest());
    }


    public IEnumerator RunTest()
    {
        for (int i = 0; i < pics.Count; i++)
        {
            int index = i;
            GameData.picChoice = pics[index];
            Texture2D texture = TextureController.Instance.GenerateColorPicture(GameData.picChoice);
            Texture2D texPic = TextureController.Instance.GenerateGrayscaleTexture(GameData.picChoice, 0.8f, true);
            GameData.CurColorTexture = texture;
            GameData.curGrayTexture = texPic;
            var gamePlayController = Instantiate(gamePlayControllerPrefab, null);
            yield return new WaitForSeconds(0.2f);
            var generateMap = GamePlayControl.Instance.numberColoring;
            yield return new WaitUntil(() => generateMap.loadSuccess);
            GamePlayControl.Instance.FinishPicReward();
            //for (int j = 0; j < generateMap.m_colorDist.Count; j++)
            //{
            //    if (generateMap.m_colorDist[j] == Color.white)
            //    {
            //        Debug.Log(GameData.picChoice.Id + "|" + GameData.picChoice.IdArea + "|" + GameData.picChoice.IdCateAre);
            //    }
            //}

            Destroy(gamePlayController.gameObject);
            Destroy(generateMap.gameObject);
            Debug.Log("================== Finish Map ===============");
        }

        Debug.Log("================== DONE ===============");
    }
}

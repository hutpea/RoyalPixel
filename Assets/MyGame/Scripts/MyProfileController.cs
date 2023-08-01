using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyProfileController : MonoBehaviour
{
    [SerializeField] Transform content;
    [SerializeField] ElementAchievement elementAchievement;
    public Text txtPen;
    public Text txtBomb;
    public Text txtFind;
    public Text txtStar;
    public Text txtName;
    public static MyProfileController instance;
    [SerializeField] GameObject armorialIcon;
    [SerializeField] Transform contentArmorial;
    private void Start()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    private void OnEnable()
    {
        InitAchievement();
        InitArmorial();
        ShowItem();
        txtName.text = GameData.UserName;
    }
    public void ShowItem()
    {
        txtBomb.text = "x" + GameData.ItemBomb.ToString();
        txtFind.text = "x" + GameData.ItemFind.ToString();
        txtPen.text = "x" + GameData.ItemPen.ToString();
        txtStar.text = "x" + GameData.ItemStar.ToString();
    }
    public void InitAchievement()
    {
        if (content.childCount != 0)
        {
            foreach (Transform child in content)
                Destroy(child.gameObject);
        }

        foreach (Achivement achivement in AchievementController.instance.quests.questAchievement)
        {
            ElementAchievement element = Instantiate(elementAchievement, content);
            element.InitElement(achivement);
        }
    }
    public void InitArmorial()
    {
        if (contentArmorial.childCount != 0)
        {
            foreach (Transform child in contentArmorial)
                Destroy(child.gameObject);
        }
        foreach (Achivement achivement in AchievementController.instance.quests.questAchievement)
        {
            if (achivement.Claimed > 0)
            {
                GameObject obj = Instantiate(armorialIcon, contentArmorial);
                obj.transform.GetChild(1).gameObject.SetActive(true);
                obj.transform.GetChild(0).gameObject.SetActive(true);
                obj.transform.GetChild(1).GetComponent<Image>().sprite = achivement.ribbon;
                obj.GetComponent<Image>().sprite = achivement.spBorderArmorial;
                obj.transform.GetChild(2).gameObject.SetActive(true);
                obj.transform.GetChild(2).GetComponent<Text>().text = "x" + achivement.Claimed;
                obj.transform.GetChild(0).GetComponent<Image>().sprite = achivement.spArmorial;
            }
        }
        foreach (Achivement achivement in AchievementController.instance.quests.questAchievement)
        {
            if (achivement.Claimed <= 0)
            {
                GameObject obj = Instantiate(armorialIcon, contentArmorial);
            }
        }
    }
    public void Rename()
    {
        PopupRename.Setup().Show();
    }
}

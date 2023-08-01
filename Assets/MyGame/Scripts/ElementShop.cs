using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementShop : MonoBehaviour
{
    public List<PackShop> packs;
    public int gemBuy;
    GameObject lose;
    public void Buy()
    {
        if (GameData.Gem < gemBuy)
        {
            GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
 (
    transform.position,
     "Not Enough Gem!",
     Color.red,
     isSpawnItemPlayer: true
 );
            return;
        }
        int index = 0;
        foreach (PackShop pack in packs)
        {
            switch (pack.item)
            {
                case Item.Bomb:
                    GameData.ItemBomb += pack.number;
                    this.PostEvent(EventID.CHANGE_VALUE_ITEM_BOMB);
                    break;
                case Item.Star:
                    GameData.ItemStar += pack.number;
                    this.PostEvent(EventID.CHANGE_VALUE_ITEM_WAND);

                    break;
                case Item.Find:
                    GameData.ItemFind += pack.number;
                    this.PostEvent(EventID.CHANGE_VALUE_ITEM_FIND);
                    break;
                case Item.Pen:
                    GameData.ItemPen += pack.number;
                    this.PostEvent(EventID.CHANGE_VALUE_ITEM_PEN);
                    break;
            }
            if (index == 0)
                GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(transform.position, pack.item, "+" + pack.number, Color.black);
            else
            {
                Vector3 pos = new Vector3(transform.position.x + 0.7f, transform.position.y, transform.position.z);
                GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(pos, pack.item, "+" + pack.number, Color.black);
            }
            index++;
        }
        lose = PanelShopController.Setup().GetPoolLoseItem();
        GameData.Gem -= gemBuy;
        this.PostEvent(EventID.CHANGE_VALUE_GEM);
        PanelShopController.Setup().InitShop();
        GameController.Instance.musicManager.PlaySoundGem();
        lose.GetComponentInChildren<Text>().text = "-" + gemBuy.ToString();
    }
    private void OnDisable()
    {
        if (lose != null)
            lose.SetActive(false);
    }
}
[System.Serializable]
public class PackShop
{
    public Item item;
    public int number;
}
using UnityEngine;
using System.Collections;

public class ItemObject : MonoBehaviour
{
    public Item item;

    private IEnumerator Start()
    {
        // TODO : Data를 잘 받는지 확인용 임시 코드.
        yield return new WaitForSeconds(1.0f);

        // 'Silver Sword' 아이템 정보를 받아옵니다.
        var itemData = Main.Data.Items["SilverSword"];
        if (itemData != null)
        {
            item = new Item(itemData);
            LogItemInfo();
        }
        else
        {
            Debug.Log("'SilverSword' 아이템을 찾을 수 없습니다.");
        }
    }

    private void LogItemInfo()
    {
        // 아이템 정보를 Debug.Log로 확인합니다.
        if (item != null)
        {
            Debug.Log($"Item Name: {item.Key}, Description: {item.Description}, Type: {item.Type}, Cost: {item.Cost}");
        }
    }
}

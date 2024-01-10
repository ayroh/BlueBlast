using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private RectTransform rectTransform;

    public RectTransform rect => rectTransform;
    private CellType cellType;

    private int _count;
    public int count
    {
        get { return _count; }
        private set { if (value >= 0) _count = value; }
    }

    public void Release() => PoolManager.instance.ReleaseGoal(this);

    public int DecrementCount()
    {
        count--;
        countText.text = count.ToString();
        return count;
    }


    public void SetCount(int newCount)
    {
        count = newCount;
        countText.text = count.ToString();
        SetCountTextActive(true);
    }

    public void SetImage(Sprite newSprite)
    {
        image.sprite = newSprite;
        image.SetNativeSize();
    }

    public void SetCountTextActive(bool choice) => countText.gameObject.SetActive(choice);

    public void SetCellType(CellType newCellType) => cellType = newCellType;

    public CellType GetCellType() => cellType;
}

using UnityEngine;
using UnityEngine.UI;

// A script to enable scrolling only if the content requires it
public class OrdersUIDynamicScroll : MonoBehaviour
{
    public GameObject content;
    public ScrollRect scrollRect;
    // The maximum number of children in content for which scrolling is disabled
    // Past this number, more children will enable scrolling
    public int childrenLimit;

    private Transform contentParent;
    private int numberOfChildren;
    // Small hack to prevent the scroll bar for flickering
    // when dynamic scroll is used with reorderable lists
    private bool isEnabled = true;
    // Wait a while before reenabling checking for scrolling
    private bool isCountingDown = true;
    private int counter = 2;
    
    void Start()
    {
        contentParent = content.GetComponent<Transform>();
        numberOfChildren = contentParent.childCount;
        ChangeScrollingBehaviour();
    }

    void Update()
    {
        if (isCountingDown)
        {
            counter--;
            if (counter == 0)
            {
                counter = 2;
                isCountingDown = false;
                isEnabled = true;
            }
        }

        int currentChildren = contentParent.childCount;
        if (currentChildren != numberOfChildren && isEnabled)
        {
            numberOfChildren = currentChildren;
            ChangeScrollingBehaviour();
        }
    }

    private void ChangeScrollingBehaviour()
    {
        bool isOverLimit = numberOfChildren > childrenLimit;
        scrollRect.vertical = isOverLimit;
        scrollRect.verticalScrollbar.gameObject.SetActive(isOverLimit);
        scrollRect.verticalNormalizedPosition = 1;
    }

    public void DisableChangingOfScrolling()
    {
        isEnabled = false;
    }

    public void EnableChangingOfScrolling()
    {
        // Start the countdown
        isCountingDown = true;
    }
}

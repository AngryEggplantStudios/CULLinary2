using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField] public Camera minimapCamera;
    [SerializeField] public Transform navArrow;
    [SerializeField] public GameObject orderSubmissionStationIconPrefab;
    [SerializeField] public Transform iconsParent;

    private Transform playerBody;
    // List of pairs of the actual station and the icon of that station
    private Dictionary<int, (Transform, Transform)> orderSubmissionStationLocationsAndIcons;
    private float width;
    private float height;
    private Vector3 playerOldPosition;

    private bool hasInstantiatedIcons = false;

    void Awake()
    {
        playerBody = GameObject.FindGameObjectWithTag("PlayerBody").transform;
    }

    private void InstantiateMinimapIcons()
    {
        if (hasInstantiatedIcons)
        {
            return;
        }

        foreach (Transform child in iconsParent)
        {
            Destroy(child.gameObject);
        }

        RectTransform rt = this.GetComponent<RectTransform>();
        width = rt.sizeDelta.x;
        height = rt.sizeDelta.y;

        orderSubmissionStationLocationsAndIcons = new Dictionary<int, (Transform, Transform)>();
        playerOldPosition = playerBody.position;

        Dictionary<int, (Transform, Sprite)> relevantOrders = OrdersManager.instance.GetRelevantOrderStations();
        foreach (KeyValuePair<int, (Transform, Sprite)> order in relevantOrders)
        {
            int stationId = order.Key;
            Transform stationTransform = order.Value.Item1;
            GameObject minimapIcon = Instantiate(orderSubmissionStationIconPrefab,
                                                 new Vector3(0, 0, 0),
                                                 Quaternion.identity,
                                                 iconsParent.transform) as GameObject;
            SetIconPos(stationTransform, minimapIcon.transform);
            // Set icon image
            minimapIcon.GetComponent<Image>().sprite = order.Value.Item2;
            orderSubmissionStationLocationsAndIcons.Add(stationId, (stationTransform, minimapIcon.transform));
        }

        // Register the callbacks
        OrdersManager.instance.AddOrderCompletionCallback((stationId, _) =>
        {
            if (orderSubmissionStationLocationsAndIcons.ContainsKey(stationId))
            {
                Destroy(orderSubmissionStationLocationsAndIcons[stationId].Item2.gameObject);
                orderSubmissionStationLocationsAndIcons.Remove(stationId);
            }
        });
        OrdersManager.instance.AddOrderGenerationCallback(() => ResetInstiantedIconsFlag());

        hasInstantiatedIcons = true;
    }

    // Calling this will trigger the minimap to redraw the icons
    private void ResetInstiantedIconsFlag()
    {
        hasInstantiatedIcons = false;
    }

    void Update()
    {
        if (OrdersManager.instance.IsOrderGenerationComplete() && !hasInstantiatedIcons)
        {
            InstantiateMinimapIcons();
            return;
        }
        else if (!hasInstantiatedIcons)
        {
            return;
        }

        // Check if player has moved
        if (playerOldPosition != playerBody.position)
        {
            playerOldPosition = playerBody.position;
            // Update positions of old icons
            foreach ((Transform station, Transform icon) in orderSubmissionStationLocationsAndIcons.Values)
            {
                SetIconPos(station, icon);
            }
            navArrow.eulerAngles = new Vector3(0, 0, -playerBody.eulerAngles.y);
        }
    }

    private void SetIconPos(Transform target, Transform icon)
    {
        if (target == null)
        {
            icon.gameObject.SetActive(false);
            return;
        }

        icon.gameObject.SetActive(true);
        Vector3 screenPos = minimapCamera.WorldToScreenPoint(target.position) -
                            minimapCamera.WorldToScreenPoint(playerOldPosition);
        Vector3 localPos = new Vector3(screenPos.x,
                                       screenPos.y,
                                       0);

        if (localPos.magnitude > width / 2)
        {
            localPos = localPos.normalized * width / 2;
        }

        icon.GetComponent<RectTransform>().anchoredPosition = localPos;
    }
}

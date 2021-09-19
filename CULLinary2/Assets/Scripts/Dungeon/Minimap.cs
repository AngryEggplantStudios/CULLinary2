using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        playerBody = GameObject.FindGameObjectWithTag("PlayerBody").transform;
    }

    void Start()
    {
        RectTransform rt = this.GetComponent<RectTransform>();
        width = rt.sizeDelta.x;
        height = rt.sizeDelta.y;

        orderSubmissionStationLocationsAndIcons = new Dictionary<int, (Transform, Transform)>();
        playerOldPosition = playerBody.position;

        Dictionary<int, Transform> relevantOrders = OrdersManager.instance.GetRelevantOrderStations();
        foreach (KeyValuePair<int, Transform> order in relevantOrders)
        {
            int stationId = order.Key;
            Transform stationTransform = order.Value;
            GameObject minimapIcon = Instantiate(orderSubmissionStationIconPrefab,
                                                 new Vector3(0, 0, 0),
                                                 Quaternion.identity,
                                                 iconsParent.transform) as GameObject;
            SetIconPos(stationTransform, minimapIcon.transform);
            orderSubmissionStationLocationsAndIcons.Add(stationId, (stationTransform, minimapIcon.transform));
        }

        // Register the callback
        OrdersManager.instance.AddOrderCompletionCallback(stationId =>
        {
            if (orderSubmissionStationLocationsAndIcons.ContainsKey(stationId))
            {
                Destroy(orderSubmissionStationLocationsAndIcons[stationId].Item2.gameObject);
                orderSubmissionStationLocationsAndIcons.Remove(stationId);
            }
        });
    }

    void Update()
    {
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

    void SetIconPos(Transform target, Transform icon)
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

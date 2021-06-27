using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageCounter : MonoBehaviour
{
  [SerializeField] private GameObject damageCounterPrefab;
  [SerializeField] private RectTransform displayCanvasTransform;
  [SerializeField] private Camera displayCamera;

  public void SpawnDamageCounter(int damage)
  {
    GameObject damageCounterObject = Instantiate(damageCounterPrefab);
    damageCounterObject.transform.GetComponentInChildren<Text>().text = damage.ToString();
    damageCounterObject.transform.SetParent(displayCanvasTransform);
    damageCounterObject.transform.position = displayCamera.WorldToScreenPoint(transform.position);
  }

}

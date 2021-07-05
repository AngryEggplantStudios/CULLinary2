using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : PlayerAction
{
  public delegate void PlayerMeleeDelegate();
  public delegate void PlayerStopDelegate();
  public event PlayerMeleeDelegate OnPlayerMelee;

  private KeyCode meleeKeyCode;
  private float damage;

  private void Awake()
  {
    meleeKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Melee);
    damage = PlayerManager.instance ? PlayerManager.instance.meleeDamage : 20;
  }

  private void Update()
  {
    if (Input.GetKey(meleeKeyCode))
    {
      this.SetIsInvoking(true);
      OnPlayerMelee?.Invoke();
    }
  }

  public void StopInvoking()
  {
    this.SetIsInvoking(false);
  }

  private void OnTriggerEnter(Collider collider)
  {
    Enemy target = collider.GetComponent<Enemy>();
    if (target != null)
    {
      target.HandleHit(damage);
    }
  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ZombieSurvivorWeapon : MonoBehaviour {
    public Sprite Icon;
    public Animator anim;
    public int MagazineSize = 1;
    public int CurrentMagazine = 0;
    public int AmmoConsumption = 1;
    public ZombieSurvivorAmmo Ammo;
    public long RefireMilliseconds = 1;
    public long ReloadMilliseconds = 500;
    protected long CycleTime = 0;

    public bool ReloadOnEmpty = false;

    public float MuzzleDistance = 1.0f;

    private void Start() {
        if (Ammo == null)
            Ammo = GetComponentInChildren<ZombieSurvivorAmmo>();
        anim = GetComponent<Animator>();
    }

    protected virtual void OnFire(GameObject Projectile) {
    }

    public void Fire() {
        if (DateTime.Now.Ticks > CycleTime) {
            if (CurrentMagazine > AmmoConsumption) {
                //Set Next Refire time
                CycleTime = DateTime.Now.Ticks + (RefireMilliseconds * 1000000);
                CurrentMagazine -= AmmoConsumption;
                OnFire(Ammo.GetNext(transform.position + transform.forward * MuzzleDistance, transform.rotation));
                anim.SetTrigger("Fire");
            } else {
                //Click, reload!
                if (ReloadOnEmpty) {
                    Reload();
                } else {
                    //Set Next Refire time
                    CycleTime = DateTime.Now.Ticks + (RefireMilliseconds * 1000000);
                }
            }
        }
    }

    protected virtual void OnReload() {
    }

    public void Reload() {
        if (DateTime.Now.Ticks > CycleTime) {
            CycleTime = DateTime.Now.Ticks + (ReloadMilliseconds * 1000000);
            Ammo.Available -= (MagazineSize - CurrentMagazine);
            CurrentMagazine = MagazineSize;
            anim.SetTrigger("Reload");
            OnReload();
        }
    }
}

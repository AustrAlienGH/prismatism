﻿using System;
using System.Collections;
using UnityEngine;

public class BoomerangEffect : MonoBehaviour
{
	public BoomerangEffect()
	{
		m_speed = 0.01f;
		m_range = .1f;
		m_damage = 0.2f;
		StartingDamage = 0;
	}

	public void Start()
	{
		try
        {
		this.m_projectile = base.GetComponent<Projectile>();
		if (this.IsSynergyContingent)
		{
			if (!this.m_projectile.PossibleSourceGun || !(this.m_projectile.PossibleSourceGun.CurrentOwner is PlayerController))
			{
				return;
			}
			if (!(this.m_projectile.PossibleSourceGun.CurrentOwner as PlayerController).HasActiveBonusSynergy(this.SynergyToTest, false))
			{
				return;
			}
		}
		this.m_projectile.specRigidbody.UpdateCollidersOnScale = true;
		this.m_projectile.OnPostUpdate += this.HandlePostUpdate;
		} catch (Exception errex)
        {
			ETGModConsole.Log($"{errex}");
        }
	}

	public virtual void OnDespawned()
	{
		/*if (this.m_projectile)
		{
			this.m_projectile.RuntimeUpdateScale(1f / this.m_projectile.AdditionalScaleMultiplier);
			this.m_projectile.baseData.damage = this.m_projectile.baseData.damage / this.m_elapsedDamageGain;
		}*/
		UnityEngine.Object.Destroy(this);
	}

	private void HandlePostUpdate(Projectile proj)
	{
		try
		{
			if (!proj)
			{
			return;
			}
			float elapsedDistance = proj.GetElapsedDistance();
			if (elapsedDistance - m_lastElapsedDistance > m_range)
			{
				this.m_lastElapsedDistance = elapsedDistance;
				m_projectile.Speed -= m_speed;
				if (m_projectile.baseData.damage < StartingDamage * 1.7)
				{
					this.m_projectile.baseData.damage += m_damage;
				}
			}
			/*if (cooldown)
            {
				cooldown = false;
				StartCoroutine(StartCooldown());
            }*/
			if ((m_projectile.Speed <= 0.1) && (m_projectile.Speed >= -0.1f))
            {
				StartCoroutine(waitaminute());
            }
		}
		catch (Exception errex)
		{
			ETGModConsole.Log($"{errex}");
		}
	}

	public IEnumerator waitaminute()
    {
		yield return new WaitForSeconds(m_range);
		if ((m_projectile.Speed <= 0.1) && (m_projectile.Speed >= -0.1f))
		{
			this.m_projectile.Speed -= m_speed;
		}
		yield break;
    }
	private IEnumerator StartCooldown()
	{
		yield return new WaitForSeconds(m_range);
		this.m_projectile.Speed -= m_speed;
		cooldown = true;
		yield break;
	}

	public bool IsSynergyContingent;

	[LongNumericEnum]
	public CustomSynergyType SynergyToTest;

	public float PercentGainPerUnit;

	[NonSerialized]
	public float ScaleMultiplier;

	[NonSerialized]
	public float DamageMultiplier;

	public float MaximumDamageMultiplier;

	[NonSerialized]
	public float ScaleToDamageRatio;

	private Projectile m_projectile;

	private float m_lastElapsedDistance = 0;

	public float m_speed;

	public float m_range;

	public float m_damage;

	private bool cooldown;

	public float StartingDamage;
}

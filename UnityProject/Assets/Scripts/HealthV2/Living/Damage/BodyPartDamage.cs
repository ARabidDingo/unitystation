﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HealthV2
{
	public partial class BodyPart
	{
		//How much damage gets transferred to This part
		public Armor BodyPartArmour = new Armor();

		//How much damage gets transferred to sub organs
		public Armor SubOrganBodyPartArmour = new Armor();


		//The point at which  Specified health of Category is below that Armour becomes less effective for organs
		[Range(0, 100)] public int SubOrganDamageIncreasePoint = 50;


		public bool DamageContributesToOverallHealth = true;

		//Used for Calculating Overall damage
		public float TotalDamageWithoutOxy
		{
			get
			{
				float TDamage = 0;
				for (int i = 0; i < Damages.Length; i++)
				{
					if ((int)DamageType.Oxy == i) continue;
					TDamage += Damages[i];
				}

				return TDamage;
			}
		}

		//Used for Calculating body part damage, Such as how effective an Organ is
		public float TotalDamage
		{
			get
			{
				float TDamage = 0;
				foreach (var Damage in Damages)
				{ TDamage += Damage; }

				return TDamage;
			}
		}


		public float Toxin => Damages[(int)DamageType.Tox];
		public float Brute => Damages[(int)DamageType.Brute];
		public float Burn => Damages[(int)DamageType.Burn];
		public float Cellular => Damages[(int)DamageType.Clone];
		public float Oxy => Damages[(int)DamageType.Oxy];
		public float Stamina => Damages[(int)DamageType.Stamina];

		public readonly float[] Damages = {
			0,
			0,
			0,
			0,
			0,
			0,
		};

		public void AffectDamage(float HealthDamage, DamageType healthDamageType)
		{
			float Damage = Damages[(int)healthDamageType] + HealthDamage;

			if (Damage < 0) Damage = 0;

			Damages[(int)healthDamageType] = Damage;
		}


		public void TakeDamage(GameObject damagedBy, float damage,
			AttackType attackType, DamageType damageType)
		{

			var damageToLimb = BodyPartArmour.GetDamage(damage, attackType);
			AffectDamage(damageToLimb, damageType);

			//TotalDamage// Could do without oxygen maybe
			//May be changed to individual damage

			var organDamageRatingValue = SubOrganBodyPartArmour.GetRatingValue(attackType);
			if (health-Damages[(int)damageType] < SubOrganDamageIncreasePoint)
			{
				organDamageRatingValue += (1 - (health - Damages[(int)damageType] / SubOrganDamageIncreasePoint));
				organDamageRatingValue = Math.Min(1, organDamageRatingValue);
			}

			var OrganDamage = damage * organDamageRatingValue;
			var OrganToDamage = containBodyParts.PickRandom(); //It's not like you can aim for Someone's  liver can you
			OrganToDamage.TakeDamage(damagedBy,OrganDamage,attackType,damageType);
		}

		public void HealDamage(GameObject healingItem, int healAmt,
			DamageType damageTypeToHeal)
		{
			AffectDamage(-healAmt, damageTypeToHeal);
		}
	}
}
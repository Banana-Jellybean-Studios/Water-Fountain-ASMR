using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using PathCreation;
using TMPro;

public class Player : MonoBehaviour
{
	[System.Serializable]
	public struct Level
	{
		public ParticleSystem particleSystem;
		public GameObject moneyTextEffectPos;
		public List<GameObject> objects;
	}


	[Header("Stats")]
	public float money = 0;
	public int addLevel = 1;
	public int mergeLevel = 1;
	public int incomeLevel = 1;

	[Header("Level Increase Counts")]
	public float incomeIncreaseByLevel = 0.5f;

	[Header("Level Money Counts")]
	public float addMoneyByLevel = 50;
	public float mergeMoneyByLevel = 50;
	public float incomeMoneyByLevel = 50;

	//Current Level Counts
	private float currentMoneyIncrease = 1;

	//Current Money Counts
	private float currentAddLevelMoney = 50;
	private float currentMergeLevelMoney = 50;
	private float currentIncomeLevelMoney = 50;

	public List<Level> levelObjects;
	public List<GameObject> boruSetleri;

	public GameObject moneyTextEffect;
	public TextMeshProUGUI moneyText;

	public float timeIntervalPlus = 2;
	public float timeIntervalForEach = 1.0f;

	private float flowTime = 0;

	private float TextedMoney(float money)
	{
		return (float)Math.Round((decimal)money, 2);
	}

	private void Start()
	{
		addLevel = 1;
		CheckLevels();
	}

	private void Update()
	{
		if (Time.time > flowTime)
		{
			StopAllCoroutines();
			StartCoroutine(FlowWater());
			flowTime = Time.time + timeIntervalPlus + timeIntervalForEach * addLevel;
		}

		moneyText.text = TextedMoney(money).ToString();
	}

	private IEnumerator FlowWater()
	{
		float moneyAmount = 5;

		for (int i = 0; i < levelObjects.Count; i++)
		{
			levelObjects[i].particleSystem.Stop();
		}

		for (int i = addLevel - 1; i > -1; i--)
		{
			levelObjects[i].particleSystem.Play();
			Instantiate(moneyTextEffect, levelObjects[i].moneyTextEffectPos.transform.position, Quaternion.Euler(0, -90, 0)).GetComponent<TextMeshPro>().text = "$" + TextedMoney(moneyAmount).ToString();
			money += moneyAmount;
			yield return new WaitForSeconds(timeIntervalForEach);
			levelObjects[i].particleSystem.Stop();
		}

		yield return new WaitForSeconds(timeIntervalPlus);
	}

	public void AddFountain()
	{
		addLevel++;
		CheckLevels();
	}

	private void CheckLevels()
	{
		//Level Money Counts
		currentIncomeLevelMoney = 0;
		for (int i = 0; i < incomeLevel + 1; i++)
		{
			currentIncomeLevelMoney += (incomeLevel + 1) * incomeMoneyByLevel;
		}
		
		//Add Level
		for (int i = 0; i < levelObjects.Count; i++)
		{
			for (int j = 0; j < levelObjects[i].objects.Count; j++)
			{
				levelObjects[i].objects[j].SetActive(false);
			}
		}

		for (int i = 0; i < addLevel; i++)
		{
			for (int j = 0; j < levelObjects[i].objects.Count; j++)
			{
				levelObjects[i].objects[j].SetActive(true);
			}
		}

		//Boru
		for (int i = 0; i < boruSetleri.Count; i++)
		{
			boruSetleri[i].SetActive(false);
		}

		boruSetleri[addLevel - 1].SetActive(true);

		//Merge


		//Income
		currentMoneyIncrease = incomeLevel * incomeIncreaseByLevel + incomeIncreaseByLevel;

	}

	public void BuyIncomeUpgrade()
	{
		if (money >= currentIncomeLevelMoney)
		{
			incomeLevel++;
			money -= currentIncomeLevelMoney;
			//Vibrate();
			CheckLevels();
		}
		//Save();
	}

	/*
	 * [SerializeField] private PathCreator creator;
	[SerializeField] private GameObject waterBall;
	[SerializeField] private float spawnDistance = 0.2f;
	[SerializeField] private float spawnScaleMin = 0.3f;
	[SerializeField] private float spawnScaleMax = 1f;
	[SerializeField] private float speed = 1;
	private float distance = 0;

	private List<GameObject> balls;
	 * 
	 * balls = new List<GameObject>();

		for (float i = 0; i < creator.path.length; i+=spawnDistance)
		{
			GameObject spawned = Instantiate(waterBall, creator.path.GetPointAtDistance(i), Quaternion.identity);
			spawned.transform.localScale = Vector3.one * Random.Range(spawnScaleMin, spawnScaleMax);
			balls.Add(spawned);
		}
	 * 
	 * 
	 distance += speed * Time.deltaTime;

		for (int i = 0; i < balls.Count; i++)
		{
			balls[i].transform.position = creator.path.GetPointAtDistance(i * spawnDistance + distance);
		}
	 * */
}

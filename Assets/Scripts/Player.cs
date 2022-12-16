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

	public float money = 0;

	public List<Level> levelObjects;
	public List<GameObject> boruSetleri;

	public GameObject moneyTextEffect;
	public TextMeshProUGUI moneyText;

	public int currentLevel = 1;
	public float timeIntervalPlus = 2;
	public float timeIntervalForEach = 1.0f;

	private float flowTime = 0;

	private float TextedMoney(float money)
	{
		return (float)Math.Round((decimal)money, 2);
	}

	private void Start()
	{
		currentLevel = 1;
		CheckLevel();
	}

	private void Update()
	{
		if (Time.time > flowTime)
		{
			StopAllCoroutines();
			StartCoroutine(FlowWater());
			flowTime = Time.time + timeIntervalPlus + timeIntervalForEach * currentLevel;
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

		for (int i = currentLevel - 1; i > -1; i--)
		{
			levelObjects[i].particleSystem.Play();
			Instantiate(moneyTextEffect, levelObjects[i].moneyTextEffectPos.transform.position, Quaternion.Euler(0, -90, 0)).GetComponent<TextMeshPro>().text = "$" + TextedMoney(moneyAmount).ToString();
			money += moneyAmount;
			yield return new WaitForSeconds(timeIntervalForEach);
			levelObjects[i].particleSystem.Stop();
		}

		yield return new WaitForSeconds(timeIntervalPlus);
	}

	public void LevelUp()
	{
		currentLevel++;
		CheckLevel();
	}

	[ContextMenu("CheckLevel")]
	private void CheckLevel()
	{
		for (int i = 0; i < levelObjects.Count; i++)
		{
			for (int j = 0; j < levelObjects[i].objects.Count; j++)
			{
				levelObjects[i].objects[j].SetActive(false);
			}
		}

		for (int i = 0; i < currentLevel; i++)
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

		boruSetleri[currentLevel - 1].SetActive(true);
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

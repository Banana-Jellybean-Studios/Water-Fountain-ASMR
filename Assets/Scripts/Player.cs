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
	public struct FountainSet { public List<Fountain> fountains; public List<GameObject> otherObjs; }

	[System.Serializable]
	public struct Fountain
	{
		public GameObject flowObj;
		public GameObject flowGroundObj;
		public GameObject moneyTextEffectPos;
		public GameObject pipe;
		public GameObject pipeInnerObj;
		public float pipeOnShaderStartValue;
		public float pipeOnShaderEndValue;
		public float pipeOnShaderTransitionSeconds;
		public float pipeOffShaderStartValue;
		public float pipeOffShaderEndValue;
		public float pipeOffShaderTransitionSeconds;
		public List<GameObject> objects;
	}

	public List<FountainSet> fountainSets;

	private int openFountainCount;
	private Fountain currentFountain;

	[Header("Stats")]
	public float money = 0;
	public int fountainLevel = 1;
	public int speedLevel = 1;
	public int incomeLevel = 1;
	public float moneyIncomeForEachFlow = 10;

	[Header("Speed")]
	public float fastSpeedFactorOnTouch = 1.5f;
	public float flowWaitTime = 1.0f;

	[Header("Level Start Counts")]
	public float speedStartCount = 1f;
	public float incomeStartCount = 1f;

	[Header("Level Increase Counts")]
	public float speedIncreaseByLevel = 0.05f;
	public float incomeIncreaseByLevel = 0.5f;

	[Header("Level Money Counts")]
	public float fountainMoneyByLevel = 50;
	public float speedMoneyByLevel = 50;
	public float incomeMoneyByLevel = 50;

	//Current Level Counts
	private float currentMoneyIncrease = 1;
	private float currentSpeedNormal = 1;
	private float currentSpeed = 1;

	//Current Money Counts
	private float currentFountainLevelMoney = 50;
	private float currentSpeedLevelMoney = 50;
	private float currentIncomeLevelMoney = 50;

	[Header("Pipe Materials")]
	public Material pipeOnMat;
	public Material pipeOffMat;

	[Header("Shader On Settings")]
	public Material waterfallOnMat;
	public float shaderOnStartValue = 0;
	public float shaderOnEndValue = 0;
	public float shaderOnTransitionSeconds = 1f;

	[Header("Shader Off Settings")]
	public Material waterfallOffMat;
	public float shaderOffStartValue = 0;
	public float shaderOffEndValue = 0;
	public float shaderOffTransitionSeconds = 1f;

	public GameObject moneyTextEffect;
	public TextMeshProUGUI moneyText;

	private float flowTime = 0;
	private bool flowed = true;

	private float TextedMoney(float money)
	{
		return (float)Math.Round((decimal)money, 2);
	}

	private void Start()
	{
		fountainLevel = 1;
	    speedLevel = 1;
	    incomeLevel = 1;

		CheckLevels();

		//Load();
	}

	private void Update()
	{
		if (flowed)
		{
			StopAllCoroutines();
			StartCoroutine(FlowWater());
		}

		if (Input.touchCount != 0) currentSpeed = currentSpeedNormal * fastSpeedFactorOnTouch;
		else currentSpeed = currentSpeedNormal;

		moneyText.text = TextedMoney(money).ToString();
	}

	private IEnumerator FlowWater()
	{
		float moneyAmount = moneyIncomeForEachFlow * currentMoneyIncrease;
		flowed = false;

		for (int i = openFountainCount - 1; i > -1; i--)
		{
			//Shader On
			MeshRenderer meshFlow = currentFountain.flowObj.GetComponent<MeshRenderer>();
			meshFlow.material = waterfallOnMat;
			meshFlow.material.SetFloat("_ProgressBorder", shaderOnStartValue);
			meshFlow.material.DOFloat(shaderOnEndValue, "_ProgressBorder", shaderOnTransitionSeconds / currentSpeed);

			currentFountain.flowGroundObj.SetActive(true);

			//Money
			Instantiate(moneyTextEffect, currentFountain.moneyTextEffectPos.transform.position, Quaternion.Euler(0, -90, 0)).GetComponent<TextMeshPro>().text = "$" + TextedMoney(moneyAmount).ToString();
			money += moneyAmount;

			//Wait
			yield return new WaitForSeconds(flowWaitTime / currentSpeed);

			//Shader Off
			meshFlow.material = waterfallOffMat;
			meshFlow.material.SetFloat("_ProgressBorder", shaderOffStartValue);
			meshFlow.material.DOFloat(shaderOffEndValue, "_ProgressBorder", shaderOffTransitionSeconds / currentSpeed);

			currentFountain.flowGroundObj.SetActive(false);
		}

		//Pipe On Shader
		MeshRenderer meshPipe = currentFountain.pipeInnerObj.GetComponent<MeshRenderer>();
		meshPipe.material = pipeOnMat;
		meshPipe.material.SetFloat("_Fill", currentFountain.pipeOnShaderStartValue);
		meshPipe.material.DOFloat(currentFountain.pipeOnShaderEndValue, "_Fill", currentFountain.pipeOnShaderTransitionSeconds / currentSpeed);

		//Wait
		yield return new WaitForSeconds(currentFountain.pipeOnShaderTransitionSeconds / currentSpeed);

		//Pipe Off Shader
		meshPipe.material = pipeOffMat;
		meshPipe.material.SetFloat("_Fill", currentFountain.pipeOffShaderStartValue);
		meshPipe.material.DOFloat(currentFountain.pipeOffShaderEndValue, "_Fill", currentFountain.pipeOffShaderTransitionSeconds / currentSpeed);

		//Flow Finish
		flowed = true;
	}

	private void CheckLevels()
	{
		//Level Money Counts
		currentFountainLevelMoney = fountainLevel * fountainMoneyByLevel;
		currentSpeedLevelMoney = speedLevel * speedMoneyByLevel;
		currentIncomeLevelMoney = incomeLevel * incomeMoneyByLevel;

		//Current Fountain Level
		int checkLevel = 0;
		bool isChecked = false;

		for (int i = 0; i < fountainSets.Count; i++)
		{
			for (int j = 0; j < fountainSets[i].fountains.Count; j++)
			{
				if (fountainLevel <= checkLevel)
				{
					currentFountain = fountainSets[i].fountains[j];
					openFountainCount = j + 1;
					isChecked = true;
					break;
				}
				else
				{
					checkLevel++;
					continue;
				}
			}
			if (isChecked) break;
		}

		//Pipe
		for (int i = 0; i < fountainSets.Count; i++)
		{
			for (int j = 0; j < fountainSets[i].fountains.Count; j++)
			{
				fountainSets[i].fountains[j].pipe.SetActive(false);
			}
		}

		currentFountain.pipe.SetActive(true);

		//Speed
		currentSpeedNormal = speedStartCount + speedIncreaseByLevel * (speedLevel - 1);

		//Income
		currentMoneyIncrease = incomeStartCount + incomeIncreaseByLevel * (incomeLevel - 1);

	}

	public void BuyFountainUpgrade()
	{
		if (money >= currentFountainLevelMoney)
		{
			fountainLevel++;
			money -= currentFountainLevelMoney;
			//Vibrate();
			CheckLevels();
		}
		//Save();
	}

	public void BuySpeedUpgrade()
	{
		if (money >= currentSpeedLevelMoney)
		{
			speedLevel++;
			money -= currentSpeedLevelMoney;
			//Vibrate();
			CheckLevels();
		}
		//Save();
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
}

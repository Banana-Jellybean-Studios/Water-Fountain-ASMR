using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using PathCreation;
using TMPro;
using Unity.VisualScripting;

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
	public Transform camera;

	[Header("Rotate")]
	public GameObject rotatingObj;
	public Rigidbody rb;
	[SerializeField] private float rotateSpeed = 10;

	private int openFountainCount;
	private FountainSet currentFountainSet;

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

	[Header("Effect")]
	public GameObject moneyTextEffect;

	[Header("UI")]
	public TextMeshProUGUI haveMoneyText;
	public TextMeshProUGUI addMoneyText;
	public TextMeshProUGUI speedMoneyText;
	public TextMeshProUGUI incomeMoneyText;
	public TextMeshProUGUI addLevelText;
	public TextMeshProUGUI speedLevelText;
	public TextMeshProUGUI incomeLevelText;
	public GameObject addUIObj;
	public GameObject mergeUIObj;

	private float flowTime = 0;
	private bool flowed = true;
	private int maxFountainLevel = 0;

	private float TextedMoney(float money)
	{
		return (float)Math.Round((decimal)money, 2);
	}

	private void Start()
	{
		fountainLevel = 1;
	    speedLevel = 1;
	    incomeLevel = 1;

		//Max fountain level
		maxFountainLevel = 0;

		for (int i = 0; i < fountainSets.Count; i++)
		{
			for (int j = 0; j < fountainSets[i].fountains.Count; j++)
			{
				maxFountainLevel++;
			}
		}

		Load();

		CheckLevels();
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

		haveMoneyText.text = TextedMoney(money).ToString();
	}

	private void FixedUpdate()
	{
		if (Input.touchCount != 0)
		{
			var touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Moved)
			{
				rb.AddTorque(0, touch.deltaPosition.x * -rotateSpeed, 0);
				rotatingObj.transform.rotation = rb.transform.rotation;
			}
		}
	}

	private IEnumerator FlowWater()
	{
		float moneyAmount = moneyIncomeForEachFlow * currentMoneyIncrease;
		flowed = false;

		for (int i = openFountainCount - 1; i > -1; i--)
		{
			//Shader On
			MeshRenderer meshFlow = currentFountainSet.fountains[i].flowObj.GetComponent<MeshRenderer>();
			meshFlow.material = waterfallOnMat;
			meshFlow.material.SetFloat("_ProgressBorder", shaderOnStartValue);
			meshFlow.material.DOFloat(shaderOnEndValue, "_ProgressBorder", shaderOnTransitionSeconds / currentSpeed);

			currentFountainSet.fountains[i].flowGroundObj.SetActive(true);

			//Money
			GameObject moneyObj = Instantiate(moneyTextEffect, currentFountainSet.fountains[i].moneyTextEffectPos.transform.position, Quaternion.Euler(0, -90, 0));
			moneyObj.GetComponent<TextMeshPro>().text = "$" + TextedMoney(moneyAmount).ToString();
			moneyObj.transform.rotation = camera.transform.rotation;
			money += moneyAmount;

			//Wait
			yield return new WaitForSeconds(flowWaitTime / currentSpeed);

			//Shader Off
			meshFlow.material = waterfallOffMat;
			meshFlow.material.SetFloat("_ProgressBorder", shaderOffStartValue);
			meshFlow.material.DOFloat(shaderOffEndValue, "_ProgressBorder", shaderOffTransitionSeconds / currentSpeed);

			currentFountainSet.fountains[i].flowGroundObj.SetActive(false);
		}

		//Pipe On Shader
		MeshRenderer meshPipe = currentFountainSet.fountains[openFountainCount - 1].pipeInnerObj.GetComponent<MeshRenderer>();
		meshPipe.material = pipeOnMat;
		meshPipe.material.SetFloat("_Fill", currentFountainSet.fountains[openFountainCount - 1].pipeOnShaderStartValue);
		meshPipe.material.DOFloat(currentFountainSet.fountains[openFountainCount - 1].pipeOnShaderEndValue, "_Fill", currentFountainSet.fountains[openFountainCount - 1].pipeOnShaderTransitionSeconds / currentSpeed);

		//Wait
		yield return new WaitForSeconds(currentFountainSet.fountains[openFountainCount - 1].pipeOnShaderTransitionSeconds / currentSpeed);

		//Pipe Off Shader
		meshPipe.material = pipeOffMat;
		meshPipe.material.SetFloat("_Fill", currentFountainSet.fountains[openFountainCount - 1].pipeOffShaderStartValue);
		meshPipe.material.DOFloat(currentFountainSet.fountains[openFountainCount - 1].pipeOffShaderEndValue, "_Fill", currentFountainSet.fountains[openFountainCount - 1].pipeOffShaderTransitionSeconds / currentSpeed);

		//Flow Finish
		flowed = true;
	}

	private void CheckLevels()
	{
		//Level money counts and texts
		currentFountainLevelMoney = fountainLevel * fountainMoneyByLevel;
		currentSpeedLevelMoney = speedLevel * speedMoneyByLevel;
		currentIncomeLevelMoney = incomeLevel * incomeMoneyByLevel;

		if (fountainLevel >= maxFountainLevel)
		{
			addMoneyText.text = "Max";
			addLevelText.text = $"Level Max";
		}
		else
		{
			addMoneyText.text = TextedMoney(currentFountainLevelMoney).ToString();
			addLevelText.text = $"Level {fountainLevel}";
		}

		speedMoneyText.text = TextedMoney(currentSpeedLevelMoney).ToString();
		speedLevelText.text = $"Level {speedLevel}";

		incomeMoneyText.text = TextedMoney(currentIncomeLevelMoney).ToString();
		incomeLevelText.text = $"Level {incomeLevel}";

		//Current Fountain Level
		int checkLevel = 1;
		bool isChecked = false;

		for (int i = 0; i < fountainSets.Count; i++)
		{
			for (int j = 0; j < fountainSets[i].fountains.Count; j++)
			{
				fountainSets[i].fountains[j].objects[0].SetActive(false);
			}
		}

		for (int i = 0; i < fountainSets.Count; i++)
		{
			for (int j = 0; j < fountainSets[i].fountains.Count; j++)
			{
				if (fountainLevel <= checkLevel)
				{
					currentFountainSet = fountainSets[i];
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

		for (int i = 0; i < fountainSets.Count; i++)
		{
			for (int j = 0; j < fountainSets[i].fountains.Count; j++)
			{
				fountainSets[i].fountains[j].objects[0].SetActive(false);
			}
		}

		for (int i = 0; i < openFountainCount; i++)
		{
			currentFountainSet.fountains[i].objects[0].SetActive(true);
		}

		//Add - Merge text and icon change
		if (openFountainCount == currentFountainSet.fountains.Count)
		{
			mergeUIObj.SetActive(true);
			addUIObj.SetActive(false);
		}
		else
		{
			mergeUIObj.SetActive(false);
			addUIObj.SetActive(true);
		}

		//Pipe
		for (int i = 0; i < fountainSets.Count; i++)
		{
			for (int j = 0; j < fountainSets[i].fountains.Count; j++)
			{
				fountainSets[i].fountains[j].pipe.SetActive(false);
			}
		}

		currentFountainSet.fountains[openFountainCount - 1].pipe.SetActive(true);

		//Speed
		currentSpeedNormal = speedStartCount + speedIncreaseByLevel * (speedLevel - 1);

		//Income
		currentMoneyIncrease = incomeStartCount + incomeIncreaseByLevel * (incomeLevel - 1);

		Save();
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

	private void Save()
	{
		PlayerPrefs.SetFloat("Money", money);
		PlayerPrefs.SetInt("FountainLevel", fountainLevel);
		PlayerPrefs.SetInt("SpeedLevel", speedLevel);
		PlayerPrefs.SetInt("IncomeLevel", incomeLevel);
	}

	private void Load()
	{
		if(PlayerPrefs.HasKey("Money")) money = PlayerPrefs.GetFloat("Money");
		if(PlayerPrefs.HasKey("FountainLevel")) fountainLevel = PlayerPrefs.GetInt("FountainLevel");
		if(PlayerPrefs.HasKey("SpeedLevel")) speedLevel = PlayerPrefs.GetInt("SpeedLevel");
		if (PlayerPrefs.HasKey("IncomeLevel")) incomeLevel = PlayerPrefs.GetInt("IncomeLevel");
	}

	public void ResetSave()
	{
		money = 0;
		fountainLevel = 1;
		speedLevel = 1;
		incomeLevel = 1;

		CheckLevels();
	}

	private void OnApplicationQuit()
	{
		Save();
	}
}

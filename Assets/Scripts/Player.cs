using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{
	[SerializeField] private float lerpTime = 0.3f;

	[Header("WaterfallsGround")]
	[SerializeField] private List<GameObject> waterfallsGround;
	[SerializeField] private float normalVoroniSpeedOnGround = 0.15f;
	[SerializeField] private float highVoroniSpeedOnGround = 0.3f;

	private float currentSpeedOnGround;

	[Header("Waterfalls")]
	[SerializeField] private List<GameObject> waterfalls;
	[SerializeField] private float normalVoroniSpeed = 0.3f;
	[SerializeField] private float highVoroniSpeed = 0.6f;
	[SerializeField] private float woobbleAmountNormal = 0;
	[SerializeField] private float woobbleAmountHigh = 0.1f;
	[SerializeField] private float woobbleFrequencyNormal = 0;
	[SerializeField] private float woobbleFrequencyHigh = 0.1f;
	[SerializeField] private float woobbleHeightNormal = 0;
	[SerializeField] private float woobbleHeightHigh = 0.1f;
	[SerializeField] private float woobbleSpeedNormal = 0;
	[SerializeField] private float woobbleSpeedHigh = 0.1f;

	private float currentSpeed;
	private float currentWoobbleAmount;
	private float currentWoobbleFrequency;
	private float currentWoobbleHeight;
	private float currentWoobbleSpeed;

	private void Update()
    {
        if (Input.touchCount > 0)
        {
			//Waterfalls
			currentSpeed = highVoroniSpeed;
			currentWoobbleAmount = woobbleAmountHigh;
			currentWoobbleFrequency = woobbleFrequencyHigh;
			currentWoobbleHeight = woobbleHeightHigh;
			currentWoobbleSpeed = woobbleSpeedHigh;

			//Waterfalls On Ground
			currentSpeedOnGround = highVoroniSpeedOnGround;
		}
        else
		{
			//Waterfalls
			currentSpeed = normalVoroniSpeed;
			currentWoobbleAmount = woobbleAmountNormal;
			currentWoobbleFrequency = woobbleFrequencyNormal;
			currentWoobbleHeight = woobbleHeightNormal;
			currentWoobbleSpeed = woobbleSpeedNormal;

			//Waterfalls On Ground
			currentSpeedOnGround = normalVoroniSpeedOnGround;
		}

		//Waterfalls
		foreach (var item in waterfalls)
		{
			item.GetComponent<MeshRenderer>().material.DOVector(new Vector2(0, currentSpeed), "_VoroniSpeed", lerpTime);
			item.GetComponent<MeshRenderer>().material.DOFloat(currentWoobbleAmount, "_WoobbleAmount", lerpTime);
			item.GetComponent<MeshRenderer>().material.DOFloat(currentWoobbleFrequency, "_WoobbleFrequency", lerpTime);
			item.GetComponent<MeshRenderer>().material.DOFloat(currentWoobbleHeight, "_WoobbleHeight", lerpTime);
			item.GetComponent<MeshRenderer>().material.DOFloat(currentWoobbleSpeed, "_WoobbleSpeed", lerpTime);
		}

		//Waterfalls On Ground
		foreach (var item in waterfallsGround)
		{
			item.GetComponent<MeshRenderer>().material.DOVector(new Vector2(0, currentSpeedOnGround), "_VoroniSpeed", lerpTime);
		}
	}
}

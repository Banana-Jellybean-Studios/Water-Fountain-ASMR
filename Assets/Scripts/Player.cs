using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{
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
	[SerializeField] private float woobbleFrequancyNormal = 0;
	[SerializeField] private float woobbleFrequancyHigh = 0.1f;
	[SerializeField] private float woobbleHeightNormal = 0;
	[SerializeField] private float woobbleHeightHigh = 0.1f;
	[SerializeField] private float woobbleSpeedNormal = 0;
	[SerializeField] private float woobbleSpeedHigh = 0.1f;

	private float currentSpeed;
	private float currentWoobbleAmount;
	private float currentWoobbleFrequancy;
	private float currentWoobbleHeight;
	private float currentWoobbleSpeed;

	private void Update()
    {
        if (Input.touchCount > 0)
        {
			//Waterfalls
			currentSpeed = highVoroniSpeed;
			currentWoobbleAmount = woobbleAmountHigh;
			currentWoobbleFrequancy = woobbleFrequancyHigh;
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
			currentWoobbleFrequancy = woobbleFrequancyNormal;
			currentWoobbleHeight = woobbleHeightNormal;
			currentWoobbleSpeed = woobbleSpeedNormal;

			//Waterfalls On Ground
			currentSpeedOnGround = normalVoroniSpeedOnGround;
		}

		//Waterfalls
		foreach (var item in waterfalls)
		{
			item.GetComponent<MeshRenderer>().material.SetVector("_VoroniSpeed", new Vector2(0, currentSpeed));
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleAmount", currentWoobbleAmount);
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleFrequancy", currentWoobbleFrequancy);
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleHeight", currentWoobbleHeight);
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleSpeed", currentWoobbleSpeed);
		}

		//Waterfalls On Ground
		foreach (var item in waterfallsGround)
		{
			item.GetComponent<MeshRenderer>().material.SetVector("_VoroniSpeed", new Vector2(0, currentSpeedOnGround));
		}
	}
}

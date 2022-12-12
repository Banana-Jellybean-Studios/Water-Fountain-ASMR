using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{
	[SerializeField] private float lerpSpeed = 5;

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
			item.GetComponent<MeshRenderer>().material.SetVector("_VoroniSpeed", new Vector2(0, Mathf.Lerp(item.GetComponent<MeshRenderer>().material.GetVector("_VoroniSpeed").y, currentSpeed, lerpSpeed * Time.deltaTime)));
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleAmount", Mathf.Lerp(item.GetComponent<MeshRenderer>().material.GetVector("_WoobbleAmount").y, currentWoobbleAmount, lerpSpeed * Time.deltaTime));
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleFrequancy", Mathf.Lerp(item.GetComponent<MeshRenderer>().material.GetVector("_WoobbleFrequancy").y, currentWoobbleFrequancy, lerpSpeed * Time.deltaTime));
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleHeight", Mathf.Lerp(item.GetComponent<MeshRenderer>().material.GetVector("_WoobbleHeight").y, currentWoobbleHeight, lerpSpeed * Time.deltaTime));
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleSpeed", Mathf.Lerp(item.GetComponent<MeshRenderer>().material.GetVector("_WoobbleSpeed").y, currentWoobbleSpeed, lerpSpeed * Time.deltaTime));
		}

		//Waterfalls On Ground
		foreach (var item in waterfallsGround)
		{
			item.GetComponent<MeshRenderer>().material.SetVector("_VoroniSpeed", new Vector2(0, Mathf.Lerp(item.GetComponent<MeshRenderer>().material.GetVector("_VoroniSpeed").y, currentSpeedOnGround, lerpSpeed * Time.deltaTime)));
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SplineMesh;
using DG.Tweening;

[ExecuteInEditMode]
[RequireComponent(typeof(Spline))]
public class ContortAlong : MonoBehaviour
{
	private Spline spline;
	private MeshBender meshBender;

	[HideInInspector]
	public GameObject generated;
	public bool isSplashed = false;

	public Mesh mesh;
	public Material material;
	public Vector3 rotation;
	public Vector3 scale;

	public Vector3 startScale;
	public Vector3 targetScale;
	[Range(0, 1)] private float lerp;
	[SerializeField] private float scaleDuration = 1f;
	[SerializeField] private Ease scaleEase = Ease.OutBack;
	[SerializeField] private float moveStartDuration = 1f;
	[SerializeField] private float moveSpeed;

	[SerializeField] private ParticleSystem puddle;
	[SerializeField] private ParticleSystem splash;
	[SerializeField] private float effectStartDuration = 1f;

	private void Start()
	{
		isSplashed = false;
		Init(); 
		puddle.gameObject.SetActive(true);
		puddle.Stop();
		splash.gameObject.SetActive(true);
		splash.Stop();
		Invoke("ClosePuddle", 0);
	}

	private void Update()
	{
		if (generated != null && lerp < 1)
		{
			meshBender.SetInterval(spline, spline.Length * lerp);
			meshBender.ComputeIfNeeded();
		}
	}

	[ContextMenu("SplashWater")]
	public void SplashWater()
	{
		StartCoroutine(Splash());
	}

	private IEnumerator Splash()
	{
		GameObject curObj = new GameObject();
		isSplashed = false;
		meshBender.Source = meshBender.Source.Scale(startScale);
		lerp = 0;
		curObj.transform.localScale = startScale;

		meshBender.gameObject.SetActive(true);

		curObj.transform.DOScale(targetScale, scaleDuration).SetEase(scaleEase);
		meshBender.Source = meshBender.Source.Scale(curObj.transform.localScale);

		float time = 0;

		//Scale
		while (true)
		{
			yield return new WaitForFixedUpdate();

			time += Time.deltaTime;

			if (time > effectStartDuration && puddle.isStopped)
			{
				puddle.Play();
				splash.Play();
			}

			/*if (lerp > 0.5f)
			{
				isSplashed = true;
			}*/

			if (lerp < 1)
			{
				if (time > moveStartDuration) lerp += moveSpeed * Time.deltaTime;

				if (time < scaleDuration) meshBender.Source = meshBender.Source.Scale(curObj.transform.localScale);
			}

			else break;
		}

		Destroy(curObj);
		puddle.Stop();
		splash.Stop();
		isSplashed = true;
		meshBender.Source.Scale(startScale);
		Invoke("ClosePuddle", 0.1f);
	}

	public void CloseSplash()
	{
		isSplashed = true;
		meshBender.Source = meshBender.Source.Scale(startScale);
		lerp = 0;
		meshBender.gameObject.SetActive(false);
		puddle.Stop();
		splash.Stop();
		ClosePuddle();
	}

	private void ClosePuddle()
	{
		puddle.gameObject.SetActive(false);
		//meshBender.gameObject.SetActive(false);
	}

	private void Init()
	{
		string generatedName = "generated by " + GetType().Name;
		var generatedTranform = transform.Find(generatedName);
		generated = generatedTranform != null ? generatedTranform.gameObject : UOUtility.Create(generatedName, gameObject,
			typeof(MeshFilter),
			typeof(MeshRenderer),
			typeof(MeshBender));

		generated.GetComponent<MeshRenderer>().material = material;

		meshBender = generated.GetComponent<MeshBender>();
		spline = GetComponent<Spline>();

		meshBender.Source = SourceMesh.Build(mesh)
			.Rotate(Quaternion.Euler(rotation))
			.Scale(scale);
		meshBender.Mode = MeshBender.FillingMode.Once;
		meshBender.SetInterval(spline, 0);
	}
}

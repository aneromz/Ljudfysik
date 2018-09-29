using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisual : MonoBehaviour {

	private const int SAMPLE_SIZE = 1024; // array size (samples). -> average sound level of the last 21.3mS(1024/48000)

	public float rmsValue; // sound level - RMS
	public float dbValue; // sound level - dB
	public float pitchValue; // sound pitch - Hz

	public int amnVisual = 64; // How many boxes we want

	public float backgroundIntensity;
	public Material backgroundMaterial;
	public Color minColor;
	public Color maxColor;

	public float visualModifier = 50.0f;
	public float smoothSpeed = 10.0f;
	public float KeepPercentage = 0.5f; 	
	public float maxvalue = 25f;

	private AudioSource source;
	private float[] samples; //Audio samples
	private float[] spectrum; // Audio spectrum
	private float sampleRate;

	private Transform[] visualList;
	private float[] visualScale;




	private void Start()
	{
		source = GetComponent<AudioSource>();
		samples = new float[SAMPLE_SIZE];
		spectrum = new float[SAMPLE_SIZE];
		sampleRate = AudioSettings.outputSampleRate; //The actual sampling frequency (48000Hz)

		SpawnLine();

	}

	private void SpawnLine()
	{
		visualScale = new float[amnVisual];
		visualList = new Transform[amnVisual];

		for (int i = 0; i < amnVisual; i++) {
			GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube) as GameObject;
			visualList[i] = go.transform;
			visualList[i].position = Vector3.right * i;
		}

	}

	/*
	private void spawnCircle()
	{
		visualScale = new float[amnVisual];
		visualList = new Transform[amnVisual];

		Vector3 center = Vector3.zero;
		float radius = 10.0f;

		for (int i = 0; i < amnVisual; i++)
		{
			float ang = i* 1.0f / amnVisual;
			ang = ang * Mathf.PI * 2;

			float x = center.x + Mathf.Cos(ang) * radius;
			float y = center.y + Mathf.Sin (ang) * radius;

			Vector3 pos = center + new Vector3 (x, y, 0);
			GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube) as GameObject;
			go.transform.position = pos;
			go.transform.rotation = Quaternion.LookRotation (Vector3.forward, pos);
			visualList [i] = go.transform;
		}
	} */

	private void Update()
	{
		AnalyzeSound();
		UpdateVisual();
		UpdateBackground();
	}

	private void UpdateBackground()
	{
		backgroundIntensity -= Time.deltaTime * smoothSpeed;
		if (backgroundIntensity < dbValue / 40) {
			backgroundIntensity = dbValue / 40;

			backgroundMaterial.color = Color.Lerp (maxColor, minColor, -backgroundIntensity);

		}
	}

	private void UpdateVisual()
	{
		int visualIndex = 0;
		int spectrumIndex = 0;
		int averageSize = (int)((SAMPLE_SIZE * KeepPercentage) / amnVisual)			;

		while (visualIndex < amnVisual)
		{
			int j = 0;
			float sum = 0;
			while (j < averageSize) {
				sum += spectrum [spectrumIndex];
				spectrumIndex++;
				j++;
			}

			float scaleY = sum / averageSize * visualModifier;
			visualScale [visualIndex] -= Time.deltaTime * smoothSpeed;
			if (visualScale [visualIndex] < scaleY)
			{
				visualScale [visualIndex] = scaleY;	
			}

			if (visualScale [visualIndex] > maxvalue)
				{
					visualScale[visualIndex] = maxvalue;
				}

			visualList[visualIndex].localScale = Vector3.one + Vector3.up * visualScale[visualIndex];
			visualIndex++;
		}
	}

	private void AnalyzeSound()
	{
		source.GetOutputData (samples, 0);

		//Get the RMS
		float sum = 0;
		for (int i = 0; i < SAMPLE_SIZE; i++) {

			sum += samples [i] * samples [i];

		}

		rmsValue = Mathf.Sqrt (sum / SAMPLE_SIZE);

		//Get the DB value
		dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

		// Get sound spectrum
		source.GetSpectrumData (spectrum, 0, FFTWindow.BlackmanHarris);

		//Find pitch

		//ADD CODE
	 }


}

using UnityEngine;
using System.Collections.Generic;
using Leap.Unity;
using PaintUtilities;

public class Paint : MonoBehaviour {

	public PolyHand right;

	// Leap motion api to detect pinch gestures
	public PinchDetector[] pinchDetectors;

	// Paint material / shader
	public Material[] materials;

	// Paint line thickness
	public float thickness;

	// Paint line resolution
	public int resolution;

	// Paint lines
	private PaintLine[] paintLines;

    // Fluid obj
    //public GameObject fluidTemplate;
    public GameObject fluid;

	void Awake() {
		thickness = 0.0015f;
		resolution = 3;
	}

	void Start() {
		paintLines = new PaintLine[pinchDetectors.Length];
		for (int i = 0; i < pinchDetectors.Length; i++) {
			paintLines[i] = new PaintLine(this);
		}
	}

	void Update() {
		// Update each pinch detector (one per hand)
		int index = 0;
		foreach (PinchDetector pd in pinchDetectors) {
			if (pd.DidStartPinch) {
                // Spawns fluid at point
                //if (!fluid)
                //{
                //    fluid = Instantiate(fluidTemplate);
                //}
                //else
                //{
                UnityEngine.ParticleSystem.EmissionModule em = fluid.GetComponent<ParticleSystem>().emission;
                em.enabled = true;
                //}
                Vector3 pos = pd.transform.position;
                pos.y = -pos.y;
                fluid.transform.position = pos;
                
                // Line stuff
				paintLines [index].InitPaintLine ();
			}
			if (pd.DidRelease) {
                // Fluid stuff
                UnityEngine.ParticleSystem.EmissionModule em = fluid.GetComponent<ParticleSystem>().emission;
                em.enabled = false;
                
                // Line stuff
                paintLines [index].EndPaintLine();
			}
			if (pd.IsHolding) {
                // Fluid stuff
                Vector3 pos = pd.transform.position;
                pos.y = -pos.y;
                fluid.transform.position = pos;

                // Line stuff
				paintLines [index].UpdatePaintLine (pd.Position);
			}
		}
	}
}
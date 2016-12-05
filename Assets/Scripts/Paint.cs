using UnityEngine;
using System.Collections.Generic;
using Leap.Unity;
using PaintUtilities;
using UnityEngine.UI;

public class Paint : MonoBehaviour {

	public PolyHand right;

	// Leap motion api to detect pinch gestures
	public PinchDetector[] pinchDetectors;

	// Paint material / shader
	public Material[] materials;
	public int materialIndex;

	// Paint line thickness
	public float thickness;

	// Paint line resolution
	public int resolution;

	// Paint lines
	public PaintLine[] paintLines;

    // Stencil stuff
    public GameObject stencil;
    public float snapMax;
	public bool snapping;

    private float MAKSYM_CONSTANT = 0.1f;

    // Fluid obj
    public GameObject fluidTemplate;
    public GameObject fluid;

    public GameObject fluid1;
    public GameObject fluid2;
	public bool fluidEnabled;

    // VR optimization constant
    public bool inVr;

    // Color
    public Color fluidColor;

	void Awake() {
		
	}

	void Start() {
        fluidColor = Color.green;
        stencil = null;
		snapping = false;
		fluidEnabled = false;
		materialIndex = 0;
		paintLines = new PaintLine[pinchDetectors.Length];
		for (int i = 0; i < pinchDetectors.Length; i++) {
			paintLines[i] = new PaintLine(this);
		}
	}

	void Update() {
		// Update each pinch detector (one per hand)
		int index = 0;
		foreach (PinchDetector pd in pinchDetectors) {
            
            // Sets the fluid to the currently looped on fluid (mainly to save time refactoring code for the multi-hand support added)
            if (index == 0)
            {
                fluid = fluid1;
            } else
            {
                fluid = fluid2;
            }

            // Updates the fluids color based on user selection
            updateFluidColor();

            // Logic for painting
			float strength = pd.hand.GetLeapHand () != null ? pd.hand.GetLeapHand ().PinchStrength : 0f;
			float speed = pd.hand.GetLeapHand () != null ? pd.hand.GetLeapHand ().PalmVelocity.Magnitude : 0f;

			if (pd.DidStartHold) {
				if (fluidEnabled) {
					// Spawns fluid at point
					UnityEngine.ParticleSystem.EmissionModule em = fluid.GetComponent<ParticleSystem> ().emission;
					em.enabled = true;

					// Disable fluid collision
					UnityEngine.ParticleSystem.CollisionModule mod = fluid.GetComponent<ParticleSystem> ().collision;
					mod.enabled = false;

					// Disable fluid gravity
					fluid.GetComponent<ParticleSystem> ().gravityModifier = 0.0f;
	                
					Vector3 pos = pd.transform.position;
					if (!inVr)
                    {
                        pos.y = -pos.y;
                    }
					fluid.transform.position = pos;
				} else {
					// Line stuff
					paintLines [index].InitPaintLine ();
				}
			}
			if (pd.DidRelease) {

                // Fluid stuff
				if (fluidEnabled) {
					UnityEngine.ParticleSystem.EmissionModule em = fluid.GetComponent<ParticleSystem> ().emission;
					em.enabled = false;
				} else {
					// Line stuff
					paintLines [index].EndPaintLine();
				}
			}
			if (pd.IsHolding) {
				if (fluidEnabled) {
                    // Fluid stuff
                    if (stencil == null || stencil.name == "Robot Kyle" || !snapping)
                    {
                        Vector3 pos = pd.Position;
                        if (!inVr)
                        {
                            pos.y = -pos.y;
                        }
                        fluid.transform.position = pos;
                    } else
                    {
                        Vector3 pos = pd.Position;
                        if (!inVr)
                        {
                            pos.y = -pos.y;
                        }
                        Vector3 nearest = NearestPointTo(pos, stencil);
                        fluid.transform.position = nearest;
                    }
				} else {
					// Line stuff
					if (stencil == null || stencil.name == "Robot Kyle" || !snapping)
					{
						paintLines[index].UpdatePaintLine(pd.Position, strength, speed);
					} else
					{
						paintLines[index].UpdatePaintLine(NearestPointTo(pd.Position, stencil), strength, speed);
					}
				}
			}
			index++;
		}
	}

    public Vector3 NearestVertexTo(Vector3 point, GameObject obj)
    {
        float minDist = -1;
        //print("T1" + point.x + " " + point.y + " " + point.z);
        point = obj.transform.InverseTransformPoint(point);
        //print("T2" + point.x + " " + point.y + " " + point.z);
        Mesh m = obj.GetComponent<MeshFilter>().mesh;
        Vector3 nearest = Vector3.zero;

        //print("Count" + m.vertices.Length);

        foreach(Vector3 v in m.vertices)
        {
            Vector3 diff = point - v;
            float dist = diff.sqrMagnitude;

            if (dist < minDist || minDist == -1)
            {
                nearest = v;
                minDist = dist;
            }
        }

        if ((nearest - point).magnitude > snapMax)
        {
            return obj.transform.TransformPoint(point);
        }
        // Maksym's algorithm. Calculate a point near the mesh close enough not to intersect and fuck up
        //Vector3 tempDist = point - nearest;
        //tempDist = Vector3.ClampMagnitude(tempDist, tempDist.magnitude - MAKSYM_CONSTANT);
        //nearest = point + tempDist;
        //Vector3 trans = obj.transform.TransformPoint(nearest);

		Vector3 origTrans = obj.transform.TransformPoint (point);
		Vector3 newTrans = obj.transform.TransformPoint (nearest);
		//print ("Orig:" + origTrans.x + " " + origTrans.y + " " + origTrans.z);
		//print ("New:" + newTrans.x + " " + newTrans.y + " " + newTrans.z);

		return newTrans;
    }

    // Superior method to previous as it checks triangles in mesh instead of vertices for closest point (also more computationally expensive)
    public Vector3 NearestPointTo(Vector3 point, GameObject obj)
    {

        MeshFilter m = obj.GetComponent<MeshFilter>();

        var CPC = new BaryCentricDistance(m);
        var res = CPC.GetClosestTriangleAndPoint(point);
        var closest = res.closestPoint;

        return closest;
        
    }

	public void resetFluid () {
		if (fluid1.GetComponent<ParticleSystem> ().particleCount != 0) {
			fluid1.GetComponent<ParticleSystem> ().Clear();
		}
		if (fluid2.GetComponent<ParticleSystem> ().particleCount != 0) {
			fluid2.GetComponent<ParticleSystem> ().Clear();
		}
	}

	public void changeMaterial () {
        //changeFluid();
	}

    public void changeFluid()
    {
        // Changing fluid status
		fluidEnabled = !fluidEnabled;

        //if (fluidEnabled)
        //{
        //    // If fluid is disabled, disable checkbox as well
        //    snapping = false;
        //    GameObject.Find("Snap").GetComponent<Toggle>().isOn = false;
        //}

        //GameObject.Find("Snap").GetComponent<Toggle>().interactable = !GameObject.Find("Snap").GetComponent<Toggle>().interactable;

    }

    public void updateFluidColor()
    {
        fluid.GetComponent<ParticleSystem>().startColor = fluidColor;
    }

}
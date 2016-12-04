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

    // Stencil stuff
    public GameObject stencil;
    public float snapMax;

    private float MAKSYM_CONSTANT = 0.1f;

	void Awake() {
		//thickness = 0.0015f;
		//resolution = 3;
	}

	void Start() {
        stencil = null;
		paintLines = new PaintLine[pinchDetectors.Length];
		for (int i = 0; i < pinchDetectors.Length; i++) {
			paintLines[i] = new PaintLine(this);
		}
	}

	void Update() {
		// Update each pinch detector (one per hand)
		int index = 0;
		foreach (PinchDetector pd in pinchDetectors) {
			float strength = pd.hand.GetLeapHand () != null ? pd.hand.GetLeapHand ().PinchStrength : 0f;
			float speed = pd.hand.GetLeapHand () != null ? pd.hand.GetLeapHand ().PalmVelocity.Magnitude : 0f;

			if (pd.DidStartHold) {
				paintLines [index].InitPaintLine ();
			}
			if (pd.DidRelease) {
				paintLines [index].EndPaintLine ();
			}
			if (pd.IsHolding) {
				if (stencil == null)
				{
					paintLines[index].UpdatePaintLine(pd.Position, strength, speed);
				} else
				{
					//Vector3 nearest = NearestVertexTo(pd.Position, stencil);
					//print(pd.Position.x + " " + pd.Position.y + " " + pd.Position.z);
					//print(nearest.x + " " + nearest.y + " " + nearest.z);

					paintLines[index].UpdatePaintLine(NearestVertexTo(pd.Position, stencil), strength, speed);
				}
			}

			index++;
		}
	}

    // @TODO Refactor or cite http://answers.unity3d.com/questions/7788/closest-point-on-mesh-collider.html
    public Vector3 NearestVertexTo(Vector3 point, GameObject obj)
    {
        point = obj.transform.InverseTransformPoint(point);

        Mesh m = obj.GetComponentInChildren<MeshFilter>().mesh;
        float minDist = Mathf.Infinity;
        Vector3 nearest = Vector3.zero;

        foreach(Vector3 v in m.vertices)
        {
            Vector3 diff = point - v;
            float dist = diff.sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                nearest = v;
            }
        }

        if ((nearest - point).magnitude > snapMax)
        {
			//print((nearest - point).magnitude
			//print ("Case A");
            print((nearest - point).magnitude);
            return obj.transform.TransformPoint(point);
        }
        // Maksym's algorithm. Calculate a point near the mesh close enough not to intersect and fuck up
        //Vector3 tempDist = point - nearest;
        //tempDist = Vector3.ClampMagnitude(tempDist, tempDist.magnitude - MAKSYM_CONSTANT);
        //nearest = point + tempDist;
        //Vector3 trans = obj.transform.TransformPoint(nearest);

		print ("Case B");
       // print("Trans" + trans);

		Vector3 origTrans = obj.transform.TransformPoint (point);
		Vector3 newTrans = obj.transform.TransformPoint (nearest);
		print ("Orig:" + origTrans.x + " " + origTrans.y + " " + origTrans.z);
		print ("New:" + newTrans.x + " " + newTrans.y + " " + newTrans.z);


		return newTrans;
    }
}
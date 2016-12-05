using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;

public class MenuScript : MonoBehaviour {

	//Variables used for menu UI
	public Button Menu;
	public GameObject Stencils;
	public Dropdown stencilsDD;
	public GameObject BrushThickness;
	public Slider BrushThicknessSlider;
	public GameObject BrushType;
	public GameObject ColorSelector;
	public Dropdown colorDD;
	public GameObject erase;
	public GameObject gravity;
	public GameObject snap;

	//Stencil variables
	public GameObject cube;
	public GameObject star;
	public GameObject teapot;
	public GameObject robot;

	//Other variables
	public Paint paint;
	private bool menuIsActive;

	void Start () {
        Button menuBtn = Menu.GetComponent<Button>();
        menuBtn.onClick.AddListener(MenuToggle);

        Button eraseBtn = erase.GetComponent<Button>();
        eraseBtn.onClick.AddListener(Erase);

		Button gravityBtn = gravity.GetComponent<Button>();
		gravityBtn.onClick.AddListener(Gravity);

		BrushThicknessSlider.minValue = 0.005f;
		BrushThicknessSlider.maxValue = 0.03f;

		Slider bts = BrushThicknessSlider.GetComponent<Slider> ();
		bts.onValueChanged.AddListener (delegate {
			ChooseThickness (bts);
		});
			
		Dropdown cdd = colorDD.GetComponent<Dropdown> ();
		cdd.onValueChanged.AddListener (delegate {
			ChooseColor(cdd);
		});

        Dropdown sdd = stencilsDD.GetComponent<Dropdown>();
        sdd.onValueChanged.AddListener(delegate
        {
            ChooseStencil(sdd);
        });

        menuIsActive = false;
    }

	void ChooseThickness(Slider target)
	{
		paint.thickness = target.value;
	}

	void MenuToggle(){
		if (!menuIsActive) {
			Stencils.SetActive (true);
			BrushThickness.SetActive (true);
			BrushType.SetActive (true);
			ColorSelector.SetActive (true);
			erase.SetActive (true);
			gravity.SetActive (true);
			snap.SetActive (true);
			menuIsActive = true;
		} else {
			Stencils.SetActive (false);
			BrushThickness.SetActive (false);
			BrushType.SetActive (false);
			ColorSelector.SetActive (false);
			erase.SetActive (false);
			gravity.SetActive (false);
			snap.SetActive (false);
			menuIsActive = false;
		}
	}

	private void ChooseStencil(Dropdown target) {
		switch (target.value) {
		case 0:
            GameObject.Find("PaintManager").GetComponent<Paint>().stencil = null;
			cube.SetActive (false);
			star.SetActive (false);
			teapot.SetActive (false);
			robot.SetActive (false);
			break;
		case 1:
            GameObject.Find("PaintManager").GetComponent<Paint>().stencil = cube;
			cube.SetActive (true);
			star.SetActive (false);
			teapot.SetActive (false);
			robot.SetActive (false);
			break;
		case 2:
            GameObject.Find("PaintManager").GetComponent<Paint>().stencil = star;
			cube.SetActive (false);
			star.SetActive (true);
			teapot.SetActive (false);
			robot.SetActive (false);
			break;
		case 3:
            GameObject.Find("PaintManager").GetComponent<Paint>().stencil = teapot;
			cube.SetActive (false);
			star.SetActive (false);
			teapot.SetActive (true);
			robot.SetActive (false);
			break;
		case 4:
            GameObject.Find("PaintManager").GetComponent<Paint>().stencil = robot;
			cube.SetActive (false);
			star.SetActive (false);
			teapot.SetActive (false);
			robot.SetActive (true);
			break;
		default:
			break;			
		}
	}

	private void ChooseColor(Dropdown target) {
		switch (target.value) {
		case 0:
			paint.materials [0].color = Color.green;
			break;
		case 1:
			paint.materials [0].color = Color.red;
			break;
		case 2:
			paint.materials [0].color = Color.blue;
			break;
		case 3:
			paint.materials [0].color = Color.yellow;
			break;
		default:
			break;
		}
	}

	void Erase()
	{
		Paint script = GameObject.Find("PaintManager").GetComponent<Paint>();
		if (script.fluidEnabled) script.resetFluid ();

		foreach(GameObject GO in GameObject.FindObjectsOfType(typeof(GameObject)))
		{
			if (GO.name == "PaintLineSegment") {
				//GO.GetComponent<Rigidbody> ().useGravity = true;
				//GO.GetComponent<MeshCollider> ().convex = true;
				Destroy (GO);
			}
		}
	}

	void Gravity()
	{
		Paint script = GameObject.Find("PaintManager").GetComponent<Paint>();

		script.fluid.GetComponent<ParticleSystem>().gravityModifier = -0.1f;
		UnityEngine.ParticleSystem.CollisionModule mod = script.fluid.GetComponent<ParticleSystem>().collision;
		mod.enabled = true;
	}

	public void Snap (){
		Paint script = GameObject.Find("PaintManager").GetComponent<Paint>();
		script.snapping = !script.snapping;
	}
}

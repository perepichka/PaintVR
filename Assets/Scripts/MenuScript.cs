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
	public Dropdown brushTypeDD;
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

		BrushThicknessSlider.minValue = 0.001f;
		BrushThicknessSlider.maxValue = 0.03f;

		Slider bts = BrushThicknessSlider.GetComponent<Slider> ();
		bts.onValueChanged.AddListener (delegate {
			ChooseThickness (bts);
		});
			
		Dropdown cdd = colorDD.GetComponent<Dropdown> ();
		cdd.onValueChanged.AddListener (delegate {
			ChooseColor(cdd);
		});

		Dropdown btdd = brushTypeDD.GetComponent<Dropdown> ();
		btdd.onValueChanged.AddListener (delegate {
			ChooseBrushType(btdd);
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
            snap.GetComponent<Toggle>().interactable = true;
			break;
		case 1:
            GameObject.Find("PaintManager").GetComponent<Paint>().stencil = cube;
			cube.SetActive (true);
			star.SetActive (false);
			teapot.SetActive (false);
			robot.SetActive (false);
            snap.GetComponent<Toggle>().interactable = true;
			break;
		case 2:
            GameObject.Find("PaintManager").GetComponent<Paint>().stencil = star;
			cube.SetActive (false);
			star.SetActive (true);
			teapot.SetActive (false);
			robot.SetActive (false);
            snap.GetComponent<Toggle>().interactable = true;
			break;
		case 3:
            GameObject.Find("PaintManager").GetComponent<Paint>().stencil = teapot;
			cube.SetActive (false);
			star.SetActive (false);
			teapot.SetActive (true);
			robot.SetActive (false);
            snap.GetComponent<Toggle>().interactable = true;
			break;
		case 4:
            GameObject.Find("PaintManager").GetComponent<Paint>().stencil = robot;
			cube.SetActive (false);
			star.SetActive (false);
			teapot.SetActive (false);
			robot.SetActive (true);
            paint.snapping = false;
            snap.GetComponent<Toggle>().interactable = false;
            break;
		default:
			break;			
		}
	}

	private void ChooseBrushType(Dropdown target) {
		paint.materialIndex = target.value % 2;
		paint.fluidEnabled = target.value == 2;
		if (paint.fluidEnabled)
        {
            snap.GetComponent<Toggle>().interactable = false;
            snap.GetComponent<Toggle>().isOn = false;
        } else
        {
            snap.GetComponent<Toggle>().interactable = true;
        }
	}
		
	private void ChooseColor(Dropdown target) {
		Color currentColor = Color.white;
		switch (target.value) {
		case 0:
			currentColor = Color.green;
			break;
		case 1:
			currentColor = Color.red;
			break;
		case 2:
			currentColor = Color.blue;
			break;
		case 3:
			currentColor = Color.yellow;
			break;
		default:
			break;
		}
		for (int i = 0; i < paint.paintLines.Length; i++) {
			for (int j = 0; j < paint.paintLines[i].copyMaterials.Length; j++) {
				paint.paintLines [i].copyMaterials [j].color = currentColor;
			}
		}
        paint.fluidColor = currentColor;
	}

	void Erase()
	{
		Paint script = GameObject.Find("PaintManager").GetComponent<Paint>();
		script.resetFluid ();

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

        if (paint.inVr)
        {
            script.fluid1.GetComponent<ParticleSystem>().gravityModifier = 0.1f;
            script.fluid2.GetComponent<ParticleSystem>().gravityModifier = 0.1f;
        } else
        {
            script.fluid1.GetComponent<ParticleSystem>().gravityModifier = -0.1f;
            script.fluid2.GetComponent<ParticleSystem>().gravityModifier = -0.1f;
        }
		UnityEngine.ParticleSystem.CollisionModule mod = script.fluid1.GetComponent<ParticleSystem>().collision;
		mod.enabled = true;
		UnityEngine.ParticleSystem.CollisionModule mod2 = script.fluid2.GetComponent<ParticleSystem>().collision;
		mod2.enabled = true;
	}

	public void Snap (){
		Paint script = GameObject.Find("PaintManager").GetComponent<Paint>();
		script.snapping = !script.snapping;
	}
}

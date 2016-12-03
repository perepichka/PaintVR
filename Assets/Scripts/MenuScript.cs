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
	public Button EraseButton;

	//Stencil variables
	public GameObject cube;
	public GameObject star;
	public GameObject teapot;
	public GameObject robot;

	//Other variables
	public PaintVR pinchDraw;
	private bool menuIsActive;

	void Start () {
		//Button menuBtn = Menu.GetComponent<Button>();
		//menuBtn.onClick.AddListener (MenuToggle);

		//Button eraseBtn = erase.GetComponent<Button>();
		//eraseBtn.onClick.AddListener (Erase);

		//BrushThicknessSlider.minValue = 0.0015f;
		//BrushThicknessSlider.maxValue = 0.004f;

		//Slider bts = BrushThicknessSlider.GetComponent<Slider> ();
		//bts.onValueChanged.AddListener (delegate {
		//	ChooseThickness (bts);
		//});

		//pinchDraw.DrawColor = Color.blue;

		//Dropdown cdd = colorDD.GetComponent<Dropdown> ();
		//cdd.onValueChanged.AddListener (delegate {
		//	ChooseColor(cdd);
		//});

		//Dropdown sdd = stencilsDD.GetComponent<Dropdown> ();
		//sdd.onValueChanged.AddListener (delegate {
		//	ChooseStencil(sdd);
		//});

		//menuIsActive = false;
	}

	void ChooseThickness(Slider target)
	{
		pinchDraw.DrawRadius = target.value;
	}

	void MenuToggle(){
		if (!menuIsActive) {
			Stencils.SetActive (true);
			BrushThickness.SetActive (true);
			BrushType.SetActive (true);
			ColorSelector.SetActive (true);
			erase.SetActive (true);
			menuIsActive = true;
		} else {
			Stencils.SetActive (false);
			BrushThickness.SetActive (false);
			BrushType.SetActive (false);
			ColorSelector.SetActive (false);
			erase.SetActive (false);
			menuIsActive = false;
		}
	}

	private void ChooseStencil(Dropdown target) {
		switch (target.value) {
		case 0:
			cube.SetActive (false);
			star.SetActive (false);
			teapot.SetActive (false);
			robot.SetActive (false);
			break;
		case 1:
			cube.SetActive (true);
			star.SetActive (false);
			teapot.SetActive (false);
			robot.SetActive (false);
			break;
		case 2:
			cube.SetActive (false);
			star.SetActive (true);
			teapot.SetActive (false);
			robot.SetActive (false);
			break;
		case 3:
			cube.SetActive (false);
			star.SetActive (false);
			teapot.SetActive (true);
			robot.SetActive (false);
			break;
		case 4:
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
			pinchDraw.DrawColor = Color.blue;
			break;
		case 1:
			pinchDraw.DrawColor = Color.red;
			break;
		case 2:
			pinchDraw.DrawColor = Color.green;
			break;
		case 3:
			pinchDraw.DrawColor = Color.yellow;
			break;
		default:
			pinchDraw.DrawColor = Color.blue;
			break;
		}
	}

	void Erase()
	{
		foreach(GameObject GO in GameObject.FindObjectsOfType(typeof(GameObject)))
		{
			if (GO.name == "Line Object")
				Destroy (GO);
		}
	}

}

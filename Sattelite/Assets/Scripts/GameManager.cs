using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public enum factions {
    None,
    USSR,
    USA,
    Europe,
    Asia,
    Commercial
}

public enum filters {
    None,
    Work
}

public class GameManager : MonoBehaviour {


    RaycastHit hit;
    public bool selectedSomething = false;
    public Satellite satellite;
    //SatFactory satFactory;
    //GameProgression gameProgression;
    public static int[] score = new int[10];        // current score for each sat
    public static int[] subScore = new int[10];     // a subscore for each sat, per level (minimum required score to advance level)
    public static int[] potentialScore = new int[10];   // the amount of points left to acquire
    public static int[] activeSatellites = new int[10]; 
    public static filters activeFilter;
    //int layerMassk = ~(1 << 8);
	public LayerMask layerMask;

    void Start() {
        //satFactory = GetComponent<SatFactory>();
        //gameProgression = GetComponent<GameProgression>();
    }

    void Update() {
        SelectionDetection();   // Check if im selecting
        SatControls();          // Check if im controlling selected stuff
        //Debug.Log(activeSatellites[1]);
    }

    // Check if Im pressing LMB to select stuff.
    void SelectionDetection() {
        if (Input.GetMouseButtonDown(0)) {
            if (!EventSystem.current.IsPointerOverGameObject()) {
                // Deselect if anything's selected.
                if (selectedSomething && hit.transform != null) {
					DeselectObject();
                }
                // Select next thing if it's selectable
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 132323.0f, layerMask)) {        // Clicked on something that isn't the void
                    if (hit.transform.tag == "Satellite") {  // Selected satellite so highlight it
                        hit.transform.GetComponentInChildren<Renderer>().material.color = Color.red;
                        satellite = hit.transform.GetComponent<Satellite>();
						selectedSomething = true;
                    }
					else if (hit.transform.tag == "PlanetaryButton") {
						Debug.Log("User clicked the planet");
					}
                    else {                                          // Clicked on something that isn't selectable
                        selectedSomething = false;
                    }
                }

                else {                                      // Clicked on the endless void
                    selectedSomething = false;
                }
            }
        }
    }

	public void DeselectObject() {
		selectedSomething = false;
		satellite.ColorOverlay();
		satellite.downwardSpeed = 0.0f;
		satellite = null;
	}


	void SatControls() {
        GameObject[] satellites;
        if (Input.GetKeyDown(KeyCode.Space)) {                  // Pressing Space toggles between overlays, as a test.
            if (activeFilter == filters.None) {
                activeFilter = filters.Work;
                satellites = GameObject.FindGameObjectsWithTag("Satellite");
                foreach (GameObject sat in satellites) {
                    sat.GetComponent<Satellite>().ColorOverlay();
                }
            }
            else {
                activeFilter = filters.None;
                satellites = GameObject.FindGameObjectsWithTag("Satellite");
                foreach (GameObject sat in satellites) {
                    sat.GetComponent<Satellite>().ColorOverlay();
                }
            }
        }

        //go up or down in orbit
        if (selectedSomething) {
            if (Input.GetKey(KeyCode.S)) {
				satellite.AltitudeChange(0.3f);
            }
            else if (Input.GetKey(KeyCode.W)) {
				satellite.AltitudeChange(-0.3f);
			}
            else {
				satellite.AltitudeChange(0.0f);
			}
        }
    }

    // keep track of a score, function is called from all the satellites. 
    public static void UpdateScore(int s) {
        score[s]++;
        subScore[s]--;
    }
}
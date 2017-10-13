using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeadsUpDisplay : MonoBehaviour {

    // I hate making huds. I'll make this pretty once this game is something I intend on finishing. 
    GameManager gameManager;
    public Text launchStatusInf;
    public Text typeDataInf;
    public Text typeDataProgInf;
	public Text fuelInf;

	public Text totalScoreInf;
	public Text subScoreInf;
	public Text potentialScoreInf;

    public Button mainButton;
    public Text mainButtonText;



    // Use this for initialization
    void Start() {
        gameManager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        if (gameManager.selectedSomething) {
            launchStatusInf.text = "Launch Status: " + gameManager.satellite.launchStatus.ToString();
            typeDataInf.text = "Type: " + gameManager.satellite.satType.ToString() + " " + gameManager.satellite.workActive;
            typeDataProgInf.text = "Progress: " + Mathf.Floor(gameManager.satellite.workProgressL).ToString() + "% | " + Mathf.Floor(gameManager.satellite.workProgressS).ToString() + "%";
			fuelInf.text = "Fuel: " + gameManager.satellite.fuel.ToString();
			if (gameManager.satellite.launchStatus == Satellite.launchStatuses.Waiting) {
                mainButtonText.text = "Launch";
            }
            else if (gameManager.satellite.launchStatus == Satellite.launchStatuses.Launching) {
                mainButtonText.text = null;
            }
			else if (gameManager.satellite.derelict == true) {
				mainButtonText.text = "<Derelict>";
			}
			else {
                mainButtonText.text = "Toggle Research";
            }

        }
        else {
            launchStatusInf.text = "No satellite selected";
            typeDataInf.text = null;
            typeDataProgInf.text = null;
			fuelInf.text = null;
            mainButtonText.text = null;
        }
		totalScoreInf.text = "Score: " + GameManager.score[(int)satTypes.Research].ToString();
		subScoreInf.text = "Points to score: " + GameManager.subScore[(int)satTypes.Research].ToString();
		potentialScoreInf.text = "Potential score: " + GameManager.potentialScore[(int)satTypes.Research].ToString();
    }

    public void MainButtonPress() {
        if (gameManager.selectedSomething) {
            if (gameManager.satellite.launchStatus == Satellite.launchStatuses.Waiting || gameManager.satellite.launchStatus == Satellite.launchStatuses.Launching) {
                gameManager.satellite.LiftOff();
            }
            else {
                gameManager.satellite.workActive = !gameManager.satellite.workActive;
            }
        }
    }
}
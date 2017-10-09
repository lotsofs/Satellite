using UnityEngine;
using System.Collections;

public class SatResearch : Satellite {

    public float researchProgress = 0f;
    public int researchDone = 0;
    public int researchQuantity = 60;
    public int researchLevels = 1;
    RaycastHit hit;



    public void Initialize(int resQ, int resL, factions fac, float orbS, float fue, float mas, float hea, bool rad, bool sol, bool shi) {
        researchQuantity = resQ;
        researchLevels = resL;
        GameManager.potentialScore[(int)satTypes.Research] += resL;
        base.Initialize(fac, orbS, fue, mas, hea, rad, sol, shi);
    }


    public override void Update() {
        base.Update();
        if (workActive && !workFinished && launchStatus == launchStatuses.Orbiting) {      // Are we currently doing research and is it not done?
            DoResearch();
        }
        workProgressL = 100 * researchDone / researchLevels;
        workProgressS = researchProgress / researchQuantity * 100;
    }

	public void DoResearch() {
		if (radioTransmitter == false) {
			if (researchDone < researchLevels) {
				researchProgress += Time.deltaTime;
			}
			if (researchProgress >= researchQuantity) {
				researchDone += 1;              // mark one research level done within the sat
				researchProgress = 0;           // reset research progress to 0 for the next sat level
			}
			if (fuel == 0) {
				derelict = true;        // satellite has become non-functional and can't rake in its points.
				Destruction();
			}

		}
		else {
			if (Physics.Linecast(transform.position, planetToOrbit.transform.position, out hit)) {
				if (hit.transform.tag == "Planet") {        // Distort signal if another sat is blocking it. 
					researchProgress += Time.deltaTime;
					if (researchProgress >= researchQuantity) {
						researchDone += 1;              // mark one research level done within the sat
						researchProgress = 0;           // reset research progress to 0 for the next sat level
						if (researchDone >= researchLevels) {               // check if research is finished
							GameManager.activeSatellites[(int)satTypes.Research]--;     // set this satellite is inactive.
							workFinished = true;
							ColorOverlay();
						}
						AwardPoint();
					}
				}
			}
		}
		
	}

    
    public void AwardPoint() {
        GameManager.UpdateScore((int)satTypes.Research);    // give a point for research which will factor into the final score or advances the game.
        GameManager.potentialScore[(int)satTypes.Research]--;      // subtract one from the game total amount of points still available to get
    }

    public override void Destruction() {
        base.Destruction();
        //researchProgress = 0;           // reset research progress to 0 just to be sure.
        GameManager.potentialScore[(int)satTypes.Research] -= researchLevels - researchDone;      // subtract whatever this satellite could still do from the potential points
    }


}

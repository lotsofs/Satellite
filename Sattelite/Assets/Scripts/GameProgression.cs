using UnityEngine;
using System.Collections;

public class GameProgression : MonoBehaviour {

    SatFactory satFactory;

    public int level1Progress = 0;
    public int allSatsHaveSpawned = 0; // making this an int instead of a bool to avoid issues like GTA Onmission
	public SatResearch specialSatResearch;

	// Use this for initialization
	void Start() {
        satFactory = GetComponent<SatFactory>();
    }

    // Update is called once per frame
    void Update() {
		Debug.Log(level1Progress);
        Level1();
    }



    void Level1() {
		if (GameManager.score[(int)satTypes.Research] == 0 && GameManager.activeSatellites[(int)satTypes.Research] == 0) {
			// no progress has been made, and no satellite exists, so launch the first one.
			// teach the player how to launch a sat and how to enable it.
			satFactory.LaunchResearchSpecific(20, 1, factions.USSR, 50, 0, 1, 10, true, false, false);
			GameManager.subScore[(int)satTypes.Research] = 1;
		}
		else if (GameManager.subScore[(int)satTypes.Research] == 0 && level1Progress == 0) {
			level1Progress++;    //1   
								 // launch another sat and show the player that a larger orbit means a slower satellite.
								 // explain that distorted signals is a thing. --> not actually a thing yet
			satFactory.LaunchResearchSpecific(20, 3, factions.USSR, 100, 0, 1, 10, true, false, false);
			GameManager.subScore[(int)satTypes.Research] = 1;
		}
		else if (GameManager.subScore[(int)satTypes.Research] == 0 && level1Progress == 1) {
			level1Progress++;      //2 
			GameManager.subScore[(int)satTypes.Research] = 4;
			// launch three more sats to really throw the player in the deep now.
			// teach him about fuel and changing a satellites orbit. Warn him that the satellite cannot come too close to earth.
			StartCoroutine(satFactory.DelayedLaunchResearchSpecific(5, 40, 2, factions.USSR, 60, 5, 1, 10, true, false, false));
			StartCoroutine(satFactory.DelayedLaunchResearchSpecific(8, 40, 2, factions.USSR, 50, 5, 1, 10, true, false, false));
			StartCoroutine(satFactory.DelayedLaunchResearchSpecific(15, 40, 2, factions.USSR, 40, 5, 1, 10, true, false, false));
			StartCoroutine(MarkAllSatsAsSpawned(15));
		}
		else if (GameManager.subScore[(int)satTypes.Research] == 0 && level1Progress == 2) {
			level1Progress++;   //3
			GameManager.subScore[(int)satTypes.Research] = 6;
			// Tutorial is over. Launch a USSR satellite that rakes up lots of point over the long term. Then bring the US into the game.
			satFactory.LaunchResearchSpecific(120, 10, factions.USSR, 70, 10, 1, 10, true, false, false);
			StartCoroutine(satFactory.DelayedLaunchResearchSpecific(15, 40, 2, factions.USA, Random.Range(50, 100), 0, 2, 10, true, false, false));
			// After the first USA sat launch, have the president talk, then launch a bunch more.
			StartCoroutine(satFactory.DelayedLaunchResearchSpecific(65, 30, 4, factions.USSR, Random.Range(50, 100), 5, 1, 10, true, false, false));
			StartCoroutine(satFactory.DelayedLaunchResearchSpecific(65, 30, 2, factions.USA, Random.Range(50, 100), 5, 2, 10, true, false, false));
			StartCoroutine(satFactory.DelayedLaunchResearchSpecific(75, 40, 2, factions.USA, Random.Range(50, 100), 5, 2, 10, true, false, false));
			StartCoroutine(satFactory.DelayedLaunchResearchSpecific(76, 40, 3, factions.USA, Random.Range(50, 100), 5, 2, 10, true, false, false));
			StartCoroutine(satFactory.DelayedLaunchResearchSpecific(77, 60, 10, factions.USA, Random.Range(50, 100), 5, 2, 10, true, false, false));
			StartCoroutine(MarkAllSatsAsSpawned(77));
		}
		else if (GameManager.subScore[(int)satTypes.Research] == 0 && level1Progress == 3) {
			level1Progress++;   //4
			GameManager.subScore[(int)satTypes.None] = 10;
			// Introduce research satellites that need to be taken back to earth in order to collect points.
			specialSatResearch = satFactory.LaunchResearchSpecific(20, 3, factions.USA, 50, 25, 2, 10, false, false, true);
		}
		else if (level1Progress == 4 && specialSatResearch.launchStatus == Satellite.launchStatuses.Landing && specialSatResearch.researchDone > 0) {
			level1Progress++;   //5
			specialSatResearch = null;
		}
		else if (level1Progress == 5) {
			Debug.Log("level passed");
		}

        if (level1Progress == 1 && GameManager.activeSatellites[(int)satTypes.Research] == 0) {
            // The only remaining satellite was destroyed, so launch a new one (one that doesn't give as many points.)
            satFactory.LaunchResearchSpecific(20, 1, factions.USSR, 100, 0, 1, 10, true, false, false);
        }
        else if (level1Progress == 2 && allSatsHaveSpawned == 0 && GameManager.subScore[(int)satTypes.Research] > GameManager.potentialScore[(int)satTypes.Research]) {
            // All the satellites were destroyed and now you can't get more points. Launch a new one. 
            satFactory.LaunchResearchSpecific(30, 1, factions.USSR, Random.Range(4, 7) * 10, 10, 1, 10, true, false, false);
        }
		else if (level1Progress == 3 && allSatsHaveSpawned == 0 && GameManager.subScore[(int)satTypes.Research] > GameManager.potentialScore[(int)satTypes.Research]) {
            // All the satellites were destroyed and now you can't get more points. Launch a new one. 
            satFactory.LaunchResearchSpecific(40, 1, factions.USA, Random.Range(50, 100), 30, 2, 10, true, false, false);
        }
		else if (level1Progress == 4 && (specialSatResearch.derelict == true || (int)specialSatResearch.launchStatus >= 5 || specialSatResearch == null)) {
			// The one satellite integral to this sublevel has gone derelict. Launch a new one.
			specialSatResearch = satFactory.LaunchResearchSpecific(20, 1, factions.USA, 50, 25, 2, 10, false, false, true);
		}


    }


    IEnumerator MarkAllSatsAsSpawned(int t) {
        allSatsHaveSpawned++;
        yield return new WaitForSeconds(t);
        allSatsHaveSpawned--;
    }

}
using UnityEngine;
using System.Collections;

public class RoundManager : MonoBehaviour {
	
	
	/*
	 * Enemy tiene los prefabs, desde aca solo se instansean los enemigos, los stats y todo, allá (en Enemy)
	 * Quiero que los pibes se copen y comenten todo bien piola ok ?
	 * 
	 * */
		
	public static float interval; 
	public bool isSpawning;
	
	public Transform [] spawnPoints;
	public GUIText roundAlert;
	public GameObject spawnParticlePrefab;
	
	
	public PlayerBehave player;
	
	float waveInterval;
	float waveDeltaTime = 0f;	
	public bool roundStarted = false;	
	
	
	
	void Start () {	
		startNewRound();
		//lo tuve que poner aca porque no andaba, alta negrada :/
		Stats.read();
		GridSystem.setGridLength(spawnPoints.Length);
		for (int i=0; i<spawnPoints.Length; i++) {
			GridSystem.grids[i].xPos = spawnPoints[i].transform.position.x;	
		}
	}	
	
	public void startNewRound(){
		Enemy.numerator = 0;
		Enemy.current = 0;
		player.killsInRound = 0;
		waveDeltaTime = 0f;
		Round.next();	
		updateEnemiesLeft();
		
		roundAlert.GetComponent<FadeGUI>().startAnim();
		roundAlert.transform.position = new Vector3(0.5f, 0.5f, 0.5f);

		roundStarted = false;
	}	
	
	
	public void updateEnemiesLeft() {
		if (!Round.isTargetRound) {
			player.enemyCounter.text = "Restantes: " + (Round.destinyQuant - player.killsInRound);
		}
	}	

	void FixedUpdate () {
		
		waveDeltaTime+=Time.deltaTime;	
		if (!Round.isTargetRound) {
			if(roundStarted) {
				if (Enemy.current + player.killsInRound < Round.destinyQuant) {
					spawnEnemy();
				}
				else if(player.killsInRound==Round.destinyQuant) {
						startNewRound();
				}
			}
			
			else{
				newRoundWarning();
			}
		} 

		
		
	}
	
	private void newRoundWarning() {
		
		if(waveDeltaTime>roundAlert.GetComponent<FadeGUI>().timeToFade) {
			roundStarted = true;
		}
		this.roundAlert.text = "round " + (Round.number);
		
	}	
	
	
	public void spawnEnemy() {
		if(waveDeltaTime>=RoundManager.interval) {
			int randomSpawn = Random.Range(0,spawnPoints.Length);
			if (Round.isTargetRound) {
				gameObject.GetComponent<TargetManager>().enabled = true;
			}
			else {
				gameObject.GetComponent<TargetManager>().enabled = false;
				Enemy.createEnemy(Round.type, Round.number, spawnPoints[randomSpawn], randomSpawn);
				GridSystem.grids[randomSpawn].currentEnemies ++;
				waveDeltaTime = 0;
				Enemy.current++;				
				
			}
			
			/*Instantiate (
							spawnParticlePrefab, 
							new Vector3(spawnPoints[randomSpawn].transform.position.x, 0, spawnPoints[randomSpawn].transform.position.z),
							spawnParticlePrefab.transform.rotation
						);*/
			

		}
	}
	
	public void killOne() {
		Enemy.current--;
		updateEnemiesLeft(); 
		if((Round.destinyQuant-player.killsInRound)==0) {
			this.startNewRound();
		}
		
	}	
	
	
	
	
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    [Header("Configuracion de Spawn")]
    [Tooltip("Prefab del enemigo a spawnear.")]
    public GameObject enemyPrefab;
    [Tooltip("Lista de Transforms que actuan como puntos de aparicion.")]
    public Transform[] spawnPoints;
    [Tooltip("Cantidad de enemigos a spawnear en esta oleada.")]
    public int enemiesToSpawn = 5;

    [Header("Eventos")]
    [Tooltip("Evento que se dispara cuando todos los enemigos de la oleada han sido eliminados.")]
    public UnityEngine.Events.UnityEvent OnWaveCleared;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private int currentEnemiesAlive = 0;

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab no asignado en el SpawnManager.");
            enabled = false; // Deshabilita el script si no hay prefab
            return;
        }
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de aparicion asignados en el SpawnManager.");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (spawnPoints.Length > 0)
            {
                int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);
                Transform spawnPoint = spawnPoints[randomSpawnPointIndex];
                GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                spawnedEnemies.Add(newEnemy);
                currentEnemiesAlive++;

                // Asegurarse de que el enemigo tenga un componente que notifique su destrucci�n
                EnemyHealth enemyHealth = newEnemy.GetComponent<EnemyHealth>(); // Asume que los enemigos tienen un script EnemyHealth
                if (enemyHealth != null)
                {
                    enemyHealth.OnDeath.AddListener(EnemyDestroyed);
                }
                else
                {
                    Debug.LogWarning("El enemigo spawneado no tiene un componente EnemyHealth con un evento OnDeath. No se podra rastrear su destruccion.");
                }
            }
            yield return new WaitForSeconds(0.5f); // Peque�a pausa entre spawns
        }
    }

    public void EnemyDestroyed()
    {
        currentEnemiesAlive--;
        Debug.Log($"Enemigo destruido. Enemigos restantes: {currentEnemiesAlive}");

        if (currentEnemiesAlive <= 0)
        {
            Debug.Log("Oleada despejada!");
            OnWaveCleared.Invoke();
            // Aqu� podr�as iniciar la siguiente oleada o finalizar el nivel
        }
    }

    // Opcional: Limpiar la lista de enemigos spawneados si es necesario
    void OnDestroy()
    {
        foreach (GameObject enemy in spawnedEnemies.Where(e => e != null))
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.OnDeath.RemoveListener(EnemyDestroyed);
            }
        }
    }
}
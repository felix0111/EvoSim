using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackupManager : MonoBehaviour {

    public string BackupFile => Application.persistentDataPath + "/backup.entity";

    void Start() {
        StartCoroutine(StartBackupRoutine());
    }

    void OnDestroy(){
        StopAllCoroutines();
        Backup();
    }

    IEnumerator StartBackupRoutine() {
        var wait = new WaitForSeconds(60f * 5f);
        while (true) {
            yield return wait;
            Backup();
        }
    }

    public void Backup() {
        Serializer.WriteToBinaryFile(BackupFile, SimulationScript.Instance.BestEntity.Item2);
    }
    
}

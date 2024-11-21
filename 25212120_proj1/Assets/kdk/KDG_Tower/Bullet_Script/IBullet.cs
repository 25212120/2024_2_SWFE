using UnityEngine; 

public interface IBullet
{
    void SetTower(BaseStructure tower); // 타워 설정
    void Seek(Transform target); // 목표 설정
    public void SetTargetPosition(Vector3 position);

}

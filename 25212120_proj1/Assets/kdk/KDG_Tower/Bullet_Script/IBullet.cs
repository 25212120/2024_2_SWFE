using UnityEngine; 

public interface IBullet
{
    void SetTower(BaseStructure tower); // Ÿ�� ����
    void Seek(Transform target); // ��ǥ ����
    public void SetTargetPosition(Vector3 position);

}

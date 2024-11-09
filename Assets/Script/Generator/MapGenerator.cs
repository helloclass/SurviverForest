using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject[] treesPrefabs;
    public GameObject[] bushsPrefabs;
    public GameObject[] bigStonesPrefabs;
    public GameObject[] grassPrefabs;
    public GameObject[] branchsPrefabs;
    public GameObject[] logsPrefabs;

    public float RayHeight;

    void Start()
    {
        GenerateForest(200);

        RayHeight = 150.0f;
    }

    // 以前い生まれたobjectの位置にしてposToCheckまでの距離を測れてそれがminDistanceを超えるかないかをcheckして返還します。
    bool IsPosAvailableByDistance(Vector3 posToCheck, List<Vector3> otherPoses, float minDistance)
    {
        float minDistanceWeHas = -1f;
        // 各々のobjectのpositionとposToCheckまでの距離を測れでminDistanceWehasを更新します。
        for (int i = 0; i < otherPoses.Count; i++)
        {
            float distance = Vector3.Distance(posToCheck, otherPoses[i]);
            if (distance < minDistanceWeHas || minDistanceWeHas < 0f) minDistanceWeHas = distance;
        }
        // 最後にminDistanceWeHasとminDistanceを測れて結果を返還します。
        if (minDistanceWeHas > minDistance || minDistanceWeHas < 0f) return true;
        else return false;
    }

    // 多様な環境objectsを配置します。
    public void GenerateForest(int sizeOfMap)
    {
        // 群集の大きさを決定する。
        int additionalFilling = Random.Range(1, 2);

        // 群集の数
        int countsCycle = (int)((float)sizeOfMap / 5f) * (int)additionalFilling;
        // 群集の半径
        float circlesRange = ((float)sizeOfMap / 6f) + (((float)sizeOfMap / 30f) * (int)additionalFilling);
        // 群集の中の最大objectの数
        float objectsCounts = sizeOfMap / 5f + ((sizeOfMap / 6) * (int)additionalFilling);

        // 距離比較用 変数
        List<Vector3> treesPoints = new List<Vector3>();
        List<Vector3> bushsPoints = new List<Vector3>();
        List<Vector3> bigStonesPoints = new List<Vector3>();
        List<Vector3> grassPoint = new List<Vector3>();
        List<Vector3> branchsPoints = new List<Vector3>();
        List<Vector3> logsPoints = new List<Vector3>();

        for (int a = 0; a < countsCycle; a++)
        {
            // 群集の中点
            Vector3 circleTreesPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));

            for (int i = 0; i < objectsCounts; i++) // 木のobject
            {
                // 無作為に位置を決めて
                RaycastHit hit;
                Vector3 rayPos = circleTreesPos;
                rayPos.x += Random.Range(-circlesRange, circlesRange);
                rayPos.y = RayHeight;
                rayPos.z += Random.Range(-circlesRange, circlesRange);

                // rayを照らして
                if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    // rayがうまく到達すれば
                    if (IsPosAvailableByDistance(hit.point, treesPoints, 15.0f))
                    {
                        // objectを生成する。
                        GameObject tree = Instantiate(treesPrefabs[Random.Range(0, treesPrefabs.Length)], hit.point, Quaternion.identity);
                        tree.transform.eulerAngles = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 360f), Random.Range(-7.5f, 7.5f));

                        // 次生まれる木は今の木と半径を維持する必要がありますので、今の木の位置を貯蔵します。
                        treesPoints.Add(hit.point);
                    }
                }
            }

            for (int i = 0; i < objectsCounts; i++) // bushsのobject
            {
                // 無作為に位置を決めて
                RaycastHit hit;
                Vector3 rayPos = circleTreesPos;
                rayPos.x += Random.Range(-circlesRange, circlesRange);
                rayPos.y = RayHeight;
                rayPos.z += Random.Range(-circlesRange, circlesRange);

                // rayを照らして
                if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    // rayがうまく到達すれば
                    if (IsPosAvailableByDistance(hit.point, treesPoints, 15.0f))
                    {
                        // objectを生成する。
                        GameObject tree = Instantiate(bushsPrefabs[Random.Range(0, bushsPrefabs.Length)], hit.point, Quaternion.identity);
                        tree.transform.eulerAngles = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 360f), Random.Range(-7.5f, 7.5f));

                        // 次生まれる木は今の木と半径を維持する必要がありますので、今の木の位置を貯蔵します。
                        bushsPoints.Add(hit.point);
                    }
                }
            }

            for (int i = 0; i < objectsCounts; i++) // bigStonesのobject
            {
                // 無作為に位置を決めて
                RaycastHit hit;
                Vector3 rayPos = circleTreesPos;
                rayPos.x += Random.Range(-circlesRange, circlesRange);
                rayPos.y = RayHeight;
                rayPos.z += Random.Range(-circlesRange, circlesRange);

                // rayを照らして
                if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    // rayがうまく到達すれば
                    if (IsPosAvailableByDistance(hit.point, treesPoints, 20.0f))
                    {
                        // objectを生成する。
                        GameObject tree = Instantiate(bigStonesPrefabs[Random.Range(0, bigStonesPrefabs.Length)], hit.point, Quaternion.identity);
                        tree.transform.eulerAngles = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 360f), Random.Range(-7.5f, 7.5f));

                        // 次生まれる木は今の木と半径を維持する必要がありますので、今の木の位置を貯蔵します。
                        bigStonesPoints.Add(hit.point);
                    }
                }
            }

            for (int i = 0; i < objectsCounts; i++) // grassのobject
            {
                // 無作為に位置を決めて
                RaycastHit hit;
                Vector3 rayPos = circleTreesPos;
                rayPos.x += Random.Range(-circlesRange, circlesRange);
                rayPos.y = RayHeight;
                rayPos.z += Random.Range(-circlesRange, circlesRange);

                // rayを照らして
                if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    // rayがうまく到達すれば
                    if (IsPosAvailableByDistance(hit.point, treesPoints, 15.0f))
                    {
                        // objectを生成する。
                        GameObject tree = Instantiate(grassPrefabs[Random.Range(0, grassPrefabs.Length)], hit.point, Quaternion.identity);
                        tree.transform.eulerAngles = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 360f), Random.Range(-7.5f, 7.5f));

                        // 次生まれる木は今の木と半径を維持する必要がありますので、今の木の位置を貯蔵します。
                        grassPoint.Add(hit.point);
                    }
                }
            }

            for (int i = 0; i < objectsCounts; i++) // branchsのobject
            {
                // 無作為に位置を決めて
                RaycastHit hit;
                Vector3 rayPos = circleTreesPos;
                rayPos.x += Random.Range(-circlesRange, circlesRange);
                rayPos.y = RayHeight;
                rayPos.z += Random.Range(-circlesRange, circlesRange);

                // rayを照らして
                if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    // rayがうまく到達すれば
                    if (IsPosAvailableByDistance(hit.point, treesPoints, 25.0f))
                    {
                        // objectを生成する。
                        GameObject tree = Instantiate(branchsPrefabs[Random.Range(0, branchsPrefabs.Length)], hit.point, Quaternion.identity);
                        tree.transform.eulerAngles = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 360f), Random.Range(-7.5f, 7.5f));

                        // 次生まれる木は今の木と半径を維持する必要がありますので、今の木の位置を貯蔵します。
                        branchsPoints.Add(hit.point);
                    }
                }
            }

            for (int i = 0; i < objectsCounts; i++) // logsのobject
            {
                // 無作為に位置を決めて
                RaycastHit hit;
                Vector3 rayPos = circleTreesPos;
                rayPos.x += Random.Range(-circlesRange, circlesRange);
                rayPos.y = RayHeight;
                rayPos.z += Random.Range(-circlesRange, circlesRange);

                // rayを照らして
                if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    // rayがうまく到達すれば
                    if (IsPosAvailableByDistance(hit.point, treesPoints, 15.0f))
                    {
                        // objectを生成する。
                        GameObject tree = Instantiate(logsPrefabs[Random.Range(0, logsPrefabs.Length)], hit.point, Quaternion.identity);
                        tree.transform.eulerAngles = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 360f), Random.Range(-7.5f, 7.5f));

                        // 次生まれる木は今の木と半径を維持する必要がありますので、今の木の位置を貯蔵します。
                        logsPoints.Add(hit.point);
                    }
                }
            }
        }
    }
}
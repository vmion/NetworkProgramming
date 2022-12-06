using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _12_06_QueueExample : MonoBehaviour
{
    Queue<int> queData;
    Queue<int[]> queData2;
    Queue<byte[]> queData3;
    void Start()
    {
        queData = new Queue<int>();
        queData.Enqueue(1);
        queData.Enqueue(2);
        queData.Enqueue(3);
        queData.Enqueue(4);
        queData.Dequeue();        
        Debug.Log(queData.Peek());
        Debug.Log(queData.Dequeue());
        if (queData.Contains(3))
            Debug.Log("ť�� 3�̶�� �����Ͱ� �����Ѵ�.");

        //Queue�� �����ϴ� �ڷ����� ������ �迭�� ���
        queData2 = new Queue<int[]>();
        int[] d1 = new int[4];
        for(int i = 0; i < d1.Length; i++)
        {
            d1[i] = i;
        }
        int[] d2 = new int[8];
        for (int i = 0; i < d2.Length; i++)
        {
            d2[i] = i;
        }
        int[] d3 = new int[32];
        for (int i = 0; i < d3.Length; i++)
        {
            d3[i] = i;
        }
        queData2.Enqueue(d1);
        queData2.Enqueue(d2);
        queData2.Enqueue(d3);        
        int[] removeDatas = queData2.Dequeue(); //d1
        Debug.Log(removeDatas);
        removeDatas = queData2.Dequeue();  //d2
        Debug.Log(removeDatas);
        removeDatas = queData2.Dequeue();  //d3
        Debug.Log(removeDatas);
        queData2.Clear(); //Queue�� �ִ� ��� ������ ����
    }

    void Update()
    {
        
    }
}

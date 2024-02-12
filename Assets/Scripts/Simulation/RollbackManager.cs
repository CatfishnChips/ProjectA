using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollbackManager : MonoBehaviour
{

    private static int rollbackBufferSize = 7;

    private FighterRecordData[] fighterRecords = new FighterRecordData[rollbackBufferSize];
    private InputRecordData[] inputRecords = new InputRecordData[rollbackBufferSize];
    private int bufferIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bufferIndex = GameSimulator.Instance.TickCount % rollbackBufferSize;
    }
}

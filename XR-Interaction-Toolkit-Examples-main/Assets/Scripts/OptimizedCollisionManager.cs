using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Text;

public class OptimizedCollisionManager : MonoBehaviour
{
    SerialPort serialPort;
    string portName = "COM4"; // Change this to your Arduino port
    int baudRate = 115200; // Match the Arduino baud rate

    Dictionary<string, ColScript[,]> cellCols = new Dictionary<string, ColScript[,]>();
    Dictionary<string, int[]> lastResults = new Dictionary<string, int[]>();

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);

        try
        {
            serialPort.Open();
            Debug.Log("Serial port opened successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error opening serial port: {e.Message}");
        }
        
        string[] cellTypes = { "TCell1", "ICell1", "MCell1", "RCell1", "LCell1" };
        foreach (var cellType in cellTypes)
        {
            var cellArray = new ColScript[2, 8];
            InitializeColScripts(cellType, cellArray);
            cellCols[cellType] = cellArray;
            lastResults[cellType] = new int[2];
        }

        Debug.Log("Initialized Successfully");
    }

    void Update()
    {
        string finalResult = GenerateFinalResult();
        if (!string.IsNullOrEmpty(finalResult))
        {
            serialPort.Write("<" + finalResult + ">");
        }
    }

    void InitializeColScripts(string cellName, ColScript[,] cellCols)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                string objectName = $"{cellName}_{j + 1}";
                GameObject cellObject = GameObject.Find(objectName);
                if (cellObject != null)
                {
                    cellCols[i, j] = cellObject.GetComponent<ColScript>();
                }
                else
                {
                    Debug.LogError($"GameObject {objectName} not found!");
                }
            }
        }
    }

    string GenerateFinalResult()
    {
        StringBuilder finalResultBuilder = new StringBuilder();

        foreach (var entry in cellCols)
        {
            int[] results = GetCellResults(entry.Value);
            // Directly update the lastResults without checking if it is stable
            //lastResults[entry.Key] = results;

            finalResultBuilder.Append($"{results[0]:000}{results[1]:000}");
        }

        return finalResultBuilder.ToString();
    }

    int[] GetCellResults(ColScript[,] cellCols)
    {
        int[] results = new int[2];

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                results[i] += (cellCols[i, j] != null && cellCols[i, j].IsInContact()) ? (int)Mathf.Pow(2, j) : 0;
            }
        }

        return results;
    }

    void OnDestroy()
    {
        CloseSerialPort();
    }

    void CloseSerialPort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed successfully.");
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class History
{
    private List<HistoryStep> steps;
    private List<HistoryDrawStep> drawSteps;
    private string id;
    private BinaryWriter writer;
    List<byte> appendBytes;

    public History(string id)
    {
        appendBytes = new List<byte>();
        this.id = id;
        string path = GameData.HistoryPathForId(id);
        if (GameData.isDrawPixel)
        {
            drawSteps = new List<HistoryDrawStep>();
            if (File.Exists(path))
            {
                byte[] bytes = Helper.ReadFile(path);
                for (int i = 0; i < bytes.Length / 5; i++)
                {
                    HistoryDrawStep step = new HistoryDrawStep() { x = bytes[i * 5], y = bytes[i * 5 + 1], colorR = bytes[i * 5 + 2], colorG = bytes[i * 5 + 3], colorB = bytes[i * 5 + 4] };
                    drawSteps.Add(step);
                    appendBytes.Add(bytes[i * 5]);
                    appendBytes.Add(bytes[i * 5 + 1]);
                    appendBytes.Add(bytes[i * 5 + 2]);
                    appendBytes.Add(bytes[i * 5 + 3]);
                    appendBytes.Add(bytes[i * 5 + 4]);
                }
            }
        }
        else
        {
            steps = new List<HistoryStep>();
            if (File.Exists(path))
            {
                byte[] bytes = Helper.ReadFile(path);
                for (int i = 0; i < bytes.Length / 2; i++)
                {
                    HistoryStep step = new HistoryStep() { x = bytes[i * 2], y = bytes[i * 2 + 1] };
                    steps.Add(step);
                }
            }
        }
    }

    public List<HistoryStep> Steps
    {
        get => steps;
    }
    public List<HistoryDrawStep> DrawSteps
    {
        get => drawSteps;
    }


    public void AddStep(int x, int y)
    {
        HistoryStep step = new HistoryStep();
        step.x = (byte)x;
        step.y = (byte)y;
        steps.Add(step);
        appendBytes.Add(step.x);
        appendBytes.Add(step.y);
    }
    public void AddStepDraw(int x, int y, Color32 color)
    {
        RemoveValueContains(x, y);
        HistoryDrawStep step = new HistoryDrawStep();
        step.x = (byte)x;
        step.y = (byte)y;
        step.colorR = color.r;
        step.colorG = color.g;
        step.colorB = color.b;
        drawSteps.Add(step);
        appendBytes.Add(step.x);
        appendBytes.Add(step.y);
        appendBytes.Add(step.colorR);
        appendBytes.Add(step.colorG);
        appendBytes.Add(step.colorB);
    }
    public void RemoveValueContains(int x, int y)
    {
        int length = appendBytes.Count;
        for (int i = 0; i < length / 5; i++)
        {
            if (appendBytes[i * 5] == x && appendBytes[i * 5 + 1] == y)
            {
                appendBytes.RemoveAt(i * 5);
                appendBytes.RemoveAt(i * 5);
                appendBytes.RemoveAt(i * 5);
                appendBytes.RemoveAt(i * 5);
                appendBytes.RemoveAt(i * 5);
                break;
            }
        }
        for (int i = 0; i < drawSteps.Count; i++)
        {
            if (drawSteps[i].x == x && drawSteps[i].y == y)
            {
                drawSteps.RemoveAt(i);
                break;
            }
        }
    }
    public void RemoveHistory()
    {
        appendBytes.Clear();
        steps.Clear();
        Steps.Clear();
        string path = GameData.HistoryPathForId(id);
        if (File.Exists(path))
        {
            Debug.Log("============ remove history");
            File.Delete(path);
        }
    }      
    bool stop;
    public void StopRecord()
    {
        if (stop)
            return;
        if (appendBytes.Count > 0 || (GameData.picChoice.DrawPic && GameData.isEdit))
        {
            Debug.Log("stop" + appendBytes.Count);
            string path = GameData.HistoryPathForId(id);
            if (!Directory.Exists(GameData.HISTORY_PATH))
            {
                Directory.CreateDirectory(GameData.HISTORY_PATH);
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, GameData.isDrawPixel ? FileMode.Create : FileMode.Append)))
            {
                writer.Write(appendBytes.ToArray());
            }
            appendBytes.Clear();
            if (GameData.picChoice.DrawPic)
                stop = true;
        }
    }

    public short[] IntSteps
    {
        get
        {
            short[] ret = new short[steps.Count];
            for (int i = 0; i < steps.Count; i++)
            {
                ret[i] = (short)((steps[i].x << 8) | steps[i].y);
            }
            return ret;
        }
    }

}

public struct HistoryStep
{
    public byte x;
    public byte y;
}
public struct HistoryDrawStep
{
    public byte x;
    public byte y;
    public byte colorR;
    public byte colorG;
    public byte colorB;
}

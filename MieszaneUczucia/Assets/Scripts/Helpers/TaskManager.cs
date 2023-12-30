using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TaskManager
{
    CancellationTokenSource cts;

    public async Task RunTaskAsync(Func<CancellationToken, Task> taskFunc)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        try
        {
            await taskFunc(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Task was cancelled.");
        }
    }

    public void CancelCurrentTask()
    {
        if (cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
        }
    }
}
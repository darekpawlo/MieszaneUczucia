using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TaskManager
{
    private CancellationTokenSource cts;
    private bool isTaskRunning = false; // Flag to track if a task is currently running

    public async Task RunTaskAsync(Func<CancellationToken, Task> taskFunc)
    {
        if (isTaskRunning)
        {
            Debug.Log("A task is already running.");
            return; // Return early if a task is already running
        }

        isTaskRunning = true; // Set flag to indicate task is running
        cts?.Cancel();
        cts = new CancellationTokenSource();

        try
        {
            await taskFunc(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Task was cancelled.");
            // Handle any cleanup or additional actions needed on cancellation
        }
        //catch (Exception ex)
        //{
        //    Debug.Log($"An error occurred: {ex.Message}");
        //    // Handle other exceptions if necessary
        //}
        finally
        {
            isTaskRunning = false; // Reset flag when task completes, is cancelled, or an exception occurs
        }
    }

    public void CancelCurrentTask()
    {
        if (cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
        }
    }

    public bool IsTaskRunning
    {
        get { return isTaskRunning; }
    }
}
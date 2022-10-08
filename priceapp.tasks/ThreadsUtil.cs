using Microsoft.Extensions.Configuration;

namespace priceapp.tasks;

public class ThreadsUtil
{
    private readonly int _threadsCount;
    private List<(Task Task, Priority Priority)> _actions = new();
    private bool _isRunning;

    public ThreadsUtil(IConfiguration configuration)
    {
        _threadsCount = bool.Parse(configuration["Threads:UseSystem"])
            ? Environment.ProcessorCount
            : int.Parse(configuration["Threads:DefaultCount"]);
    }

    public async Task AddTask(Task action, Priority priority = Priority.Low)
    {
        if (priority == Priority.Low)
        {
            _actions.Add((action, priority));
        }
        else
        {
            var index = _actions.FindLastIndex(x => x.Priority == priority);
            if (index == -1)
            {
                if (priority == Priority.Medium)
                {
                    index = _actions.FindLastIndex(x => x.Priority == Priority.High);
                }
            }

            _actions.Insert(index + 1, (action, priority));
        }

        if (_isRunning) return;
        _isRunning = true;
        Process();
        await Task.Delay(100);
    }

    private async Task Process()
    {
        for (var j = 0; j < _threadsCount - 1; j++)
        {
            Console.WriteLine("Start thread " + j);
            ProcessPerThread();
            await Task.Delay(100);
        }
    }

    private async Task ProcessPerThread()
    {
        for (var i = 0; _actions.Count != 0 && _isRunning;)
        {
            try
            {
                var action = _actions[i];
                _actions.RemoveAt(i);

                await action.Task;
            }
            catch (Exception)
            {
                await Task.Delay(50);
            }
        }

        _isRunning = false;
    }
}

public enum Priority
{
    Low = 1,
    Medium = 2,
    High = 3
}
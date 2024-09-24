namespace MakimaBot.Model.Processors;

public class ProcessorsTestInfo
{
    public ProcessorsTestInfo(TestProcessorType processorId, List<TestProcessorType> executionQueue)
    {
        ProcessorType = processorId;
        ExecutionQueue = executionQueue;
    }

    public TestProcessorType ProcessorType { get; set; }
    public List<TestProcessorType> ExecutionQueue { get; }

    public void ExecuteProcessor()
    {
        ExecutionQueue.Add(ProcessorType);
    }
}
public enum TestProcessorType
{
    DailyActivity,
    GptMessage,
    HealthCheck,
    RandomPhrase,
    TrustedChat,
    UntrustedChat
}

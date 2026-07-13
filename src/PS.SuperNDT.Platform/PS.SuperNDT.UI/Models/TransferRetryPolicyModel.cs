namespace PS.SuperNDT.UI.Models;

public sealed class TransferRetryPolicyModel
{
    public bool Enabled { get; set; }

    public int MaximumRetryCount { get; set; }

    public int RetryIntervalSeconds { get; set; }

    public bool RetryOnNetworkFailure { get; set; }

    public bool RetryOnFileAccessFailure { get; set; }

    public bool RetryOnDestinationUnavailable { get; set; }

    public bool RetryOnTimeout { get; set; }

    public int TotalRetriesPerformed { get; set; }

    public int SuccessfulRetries { get; set; }

    public int FailedRetries { get; set; }

    public string LastRetryReason { get; set; } = string.Empty;
}
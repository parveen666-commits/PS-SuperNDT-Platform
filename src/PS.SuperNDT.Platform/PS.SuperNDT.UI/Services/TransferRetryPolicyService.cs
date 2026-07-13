using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferRetryPolicyService
{
    private readonly TransferRetryPolicyModel _policy;

    public TransferRetryPolicyService()
    {
        _policy = new TransferRetryPolicyModel
        {
            Enabled = true,
            MaximumRetryCount = 3,
            RetryIntervalSeconds = 10,
            RetryOnNetworkFailure = true,
            RetryOnFileAccessFailure = true,
            RetryOnDestinationUnavailable = true,
            RetryOnTimeout = true
        };
    }

    public TransferRetryPolicyModel GetPolicy()
    {
        return _policy;
    }

    public void UpdatePolicy(TransferRetryPolicyModel policy)
    {
        _policy.Enabled = policy.Enabled;
        _policy.MaximumRetryCount = policy.MaximumRetryCount;
        _policy.RetryIntervalSeconds = policy.RetryIntervalSeconds;
        _policy.RetryOnNetworkFailure = policy.RetryOnNetworkFailure;
        _policy.RetryOnFileAccessFailure = policy.RetryOnFileAccessFailure;
        _policy.RetryOnDestinationUnavailable = policy.RetryOnDestinationUnavailable;
        _policy.RetryOnTimeout = policy.RetryOnTimeout;
    }

    public void RegisterSuccessfulRetry()
    {
        _policy.TotalRetriesPerformed++;
        _policy.SuccessfulRetries++;
    }

    public void RegisterFailedRetry(string reason)
    {
        _policy.TotalRetriesPerformed++;
        _policy.FailedRetries++;
        _policy.LastRetryReason = reason;
    }

    public bool CanRetry(int currentRetryCount)
    {
        if (!_policy.Enabled)
            return false;

        return currentRetryCount < _policy.MaximumRetryCount;
    }
}
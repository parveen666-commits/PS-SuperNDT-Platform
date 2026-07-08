using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationValidator"/>.
    /// </summary>
    public sealed class NavigationValidator : INavigationValidator
    {
        /// <inheritdoc/>
        public bool Validate(NavigationRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            return request.ViewModelType != null;
        }
    }
}
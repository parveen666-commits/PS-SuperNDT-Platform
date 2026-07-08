using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for handling navigation parameters.
    /// </summary>
    public interface INavigationParameterService
    {
        /// <summary>
        /// Stores a navigation parameter.
        /// </summary>
        /// <param name="key">Parameter key.</param>
        /// <param name="value">Parameter value.</param>
        void Set(string key, object? value);

        /// <summary>
        /// Gets a navigation parameter.
        /// </summary>
        /// <param name="key">Parameter key.</param>
        /// <returns>The stored value.</returns>
        object? Get(string key);

        /// <summary>
        /// Removes a navigation parameter.
        /// </summary>
        /// <param name="key">Parameter key.</param>
        void Remove(string key);

        /// <summary>
        /// Clears all stored parameters.
        /// </summary>
        void Clear();
    }
}
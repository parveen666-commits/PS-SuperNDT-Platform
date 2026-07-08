using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service responsible for tracking navigation permissions.
    /// </summary>
    public interface INavigationPermissionService
    {
        /// <summary>
        /// Determines whether navigation to a ViewModel is permitted.
        /// </summary>
        /// <param name="viewModelType">Target ViewModel type.</param>
        /// <returns>True if navigation is permitted; otherwise false.</returns>
        bool CanNavigate(Type viewModelType);

        /// <summary>
        /// Grants navigation permission for a ViewModel type.
        /// </summary>
        /// <param name="viewModelType">ViewModel type.</param>
        void Grant(Type viewModelType);

        /// <summary>
        /// Removes navigation permission for a ViewModel type.
        /// </summary>
        /// <param name="viewModelType">ViewModel type.</param>
        void Revoke(Type viewModelType);
    }
}
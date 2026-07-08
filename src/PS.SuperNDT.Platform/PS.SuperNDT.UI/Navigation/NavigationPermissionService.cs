using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationPermissionService"/>.
    /// </summary>
    public sealed class NavigationPermissionService : INavigationPermissionService
    {
        private readonly HashSet<Type> _permissions = new();

        /// <inheritdoc/>
        public bool CanNavigate(Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            return _permissions.Contains(viewModelType);
        }

        /// <inheritdoc/>
        public void Grant(Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            _permissions.Add(viewModelType);
        }

        /// <inheritdoc/>
        public void Revoke(Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            _permissions.Remove(viewModelType);
        }
    }
}
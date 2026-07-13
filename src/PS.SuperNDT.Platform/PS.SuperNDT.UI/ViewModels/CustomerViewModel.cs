using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class CustomerViewModel : INotifyPropertyChanged
{
    private readonly CustomerService _customerService = new();

    private CustomerModel? _selectedCustomer;
    private string _searchText = string.Empty;

    public ObservableCollection<CustomerModel> Customers { get; } = new();

    public CustomerModel? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            OnPropertyChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public CustomerViewModel()
    {
        LoadCustomers();
    }

    public void LoadCustomers()
    {
        Customers.Clear();

        foreach (var customer in _customerService.GetAll())
        {
            Customers.Add(customer);
        }
    }

    public void Search()
    {
        Customers.Clear();

        foreach (var customer in _customerService.Search(SearchText))
        {
            Customers.Add(customer);
        }
    }

    public void Save(CustomerModel customer)
    {
        _customerService.Save(customer);
        LoadCustomers();
    }

    public void DeleteSelected()
    {
        if (SelectedCustomer == null)
            return;

        _customerService.Delete(SelectedCustomer.Id);

        LoadCustomers();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
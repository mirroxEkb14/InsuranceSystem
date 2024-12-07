﻿#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class AddBranchViewModel : ObservableObject
{
    private readonly DatabaseContext _context;

    [ObservableProperty] private string? _branchName;
    [ObservableProperty] private string? _phone;
    [ObservableProperty] private ObservableCollection<Adresa>? _availableAddresses;
    [ObservableProperty] private Adresa? _selectedAddress;
    [ObservableProperty] private string? _errorMessage;

    public AddBranchViewModel(DatabaseContext context)
    {
        _context = context;
        LoadAvailableAddresses();
    }
    [RelayCommand]
    private void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        try
        {
            
            _context.Database.ExecuteSqlRaw(
                "BEGIN AddPobocka(:p_NAZEV, :p_TELEFON, :p_ADRESA_ID_ADRESA); END;",
                new OracleParameter("p_NAZEV", BranchName!),
                new OracleParameter("p_TELEFON", Phone!),
                new OracleParameter("p_ADRESA_ID_ADRESA", SelectedAddress!.IdAdresa)
            );

            MessageBoxDisplayer.ShowInfo(MessageContainer.AddBranchSuccess);
            CloseWindow();
        }
        catch (Exception ex)
        {
            ErrorMessage = MessageContainer.GetUnexpectedErrorMessage(ex.Message);
        }
    }


    [RelayCommand]
    private void Cancel() =>
        CloseWindow();

    #region Private Helper Methods
    private void CloseWindow()
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window.DataContext == this)
            {
                window.Close();
                break;
            }
        }
    }

    private void LoadAvailableAddresses()
    {
        var addresses = _context.Adresy.ToList();
        AvailableAddresses = new ObservableCollection<Adresa>(addresses);
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(BranchName))
        {
            ErrorMessage = MessageContainer.AddBranchRequiredName;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Phone))
        {
            ErrorMessage = MessageContainer.AddClientRequiredPhone;
            return false;
        }
        if (SelectedAddress == null)
        {
            ErrorMessage = MessageContainer.AddBranchRequiredAddress;
            return false;
        }
        return true;
    }
    #endregion
}
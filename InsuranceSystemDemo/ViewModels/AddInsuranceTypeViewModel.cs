using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using System;
using System.Windows;

namespace InsuranceSystemDemo.ViewModels;

public partial class AddInsuranceTypeViewModel : ObservableObject
{
    private readonly DatabaseContext _context;

    [ObservableProperty]
    private string? _dostupnost;

    [ObservableProperty]
    private string? _podminky;

    [ObservableProperty]
    private string? _popis;

    [ObservableProperty]
    private decimal _maximalneKryti;

    [ObservableProperty]
    private decimal _minimalneKryti;

    [ObservableProperty]
    private DateTime _datumZacatku = DateTime.Now;

    [ObservableProperty]
    private string? _errorMessage;

    public AddInsuranceTypeViewModel(DatabaseContext context)
    {
        _context = context;
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        try
        {
            var newType = new TypPojistky
            {
                Dostupnost = Dostupnost!,
                Podminky = Podminky!,
                Popis = Popis,
                MaximalneKryti = MaximalneKryti,
                MinimalneKryti = MinimalneKryti,
                DatumZacatku = DatumZacatku
            };

            _context.TypPojistky.Add(newType);
            _context.SaveChanges();

            MessageBox.Show("Insurance type added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            CloseWindow();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Cancel() => CloseWindow();

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

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(Dostupnost))
        {
            ErrorMessage = "Availability is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Podminky))
        {
            ErrorMessage = "Conditions are required.";
            return false;
        }

        if (MaximalneKryti <= 0)
        {
            ErrorMessage = "Maximum coverage must be greater than zero.";
            return false;
        }

        if (MinimalneKryti <= 0)
        {
            ErrorMessage = "Minimum coverage must be greater than zero.";
            return false;
        }

        if (MinimalneKryti > MaximalneKryti)
        {
            ErrorMessage = "Minimum coverage cannot exceed maximum coverage.";
            return false;
        }

        return true;
    }
}

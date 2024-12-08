#region Improts
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Types;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class AddZamestnanecViewModel : ObservableObject
{
    [ObservableProperty] private string? _role;
    [ObservableProperty] private string? _jmeno;
    [ObservableProperty] private string? _prijmeni;
    [ObservableProperty] private string? _email;
    [ObservableProperty] private long _telefon;
    [ObservableProperty] private string? _popis;
    [ObservableProperty] private Pobocka? _selectedPobocka;
    [ObservableProperty] private Adresa? _selectedAdresa;
    [ObservableProperty] private string? _errorMessage;

    public ObservableCollection<Pobocka> PobockyList { get; set; }
    public ObservableCollection<Adresa> AdresyList { get; set; }

    private readonly Action _onClose;

    public AddZamestnanecViewModel(Action onClose)
    {
        _onClose = onClose;

        using var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
        PobockyList = new ObservableCollection<Pobocka>(context.Pobocky.ToList());
        AdresyList = new ObservableCollection<Adresa>(context.Adresy.ToList());
    }

    [RelayCommand]
    public void Save()
    {
        try
        {
            if (!ValidateInputs())
                return;

            var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
            var newId = AddZamestnanecToDatabase(context);

        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(ex.Message);
        }
    }

    [RelayCommand]
    public void Cancel() =>
        _onClose.Invoke();

    #region Private Helper Methods
    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(Role))
        {
            ErrorMessage = MessageContainer.AddEmployeeInvalidRole;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Jmeno))
        {
            ErrorMessage = MessageContainer.AddEmployeeInvalidName;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Prijmeni))
        {
            ErrorMessage = MessageContainer.AddEmployeeInvalidSurname;
            return false;
        }
        if (Telefon == 0)
        {
            ErrorMessage = MessageContainer.AddEmployeeInvalidPhone;
            return false;
        }
        if (SelectedPobocka == null)
        {
            ErrorMessage = MessageContainer.AddEmployeeInvalidBranch;
            return false;
        }
        if (SelectedAdresa == null)
        {
            ErrorMessage = MessageContainer.AddEmployeeInvalidAddress;
            return false;
        }
        return true;
    }

    private int AddZamestnanecToDatabase(DatabaseContext context)
    {
        var idParam = new OracleParameter("p_id_zamestnance", OracleDbType.Decimal)
        {
            Direction = ParameterDirection.Output
        };

        context.Database.ExecuteSqlRaw(
            "BEGIN ADDZAMESTNANEC(:p_role, :p_pobocky_id, :p_jmeno, :p_prijmeni, :p_email, :p_telefon, :p_adresa_id, :p_popis, :p_id_zamestnance); END;",
            new OracleParameter("p_role", Role),
            new OracleParameter("p_pobocky_id", SelectedPobocka?.IdPobocky),
            new OracleParameter("p_jmeno", Jmeno),
            new OracleParameter("p_prijmeni", Prijmeni),
            new OracleParameter("p_email", Email ?? (object)DBNull.Value),
            new OracleParameter("p_telefon", Telefon),
            new OracleParameter("p_adresa_id", SelectedAdresa?.IdAdresa),
            new OracleParameter("p_popis", Popis ?? (object)DBNull.Value),
            idParam
        );

        if (idParam.Value is OracleDecimal oracleDecimal)
            return oracleDecimal.ToInt32();

        throw new Exception(MessageContainer.AddEmployeeErrorRetrievingID);
    }
    #endregion
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Types;

public partial class AddZamestnanecViewModel : ObservableObject
{
    [ObservableProperty] private string? _role;
    [ObservableProperty] private string? _jmeno;
    [ObservableProperty] private string? _prijmeni;
    [ObservableProperty] private string? _email;
    [ObservableProperty] private long _telefon;
    [ObservableProperty] private string? _popis;

    public ObservableCollection<Pobocka> PobockyList { get; set; }
    public ObservableCollection<Adresa> AdresyList { get; set; }

    [ObservableProperty] private Pobocka? _selectedPobocka;
    [ObservableProperty] private Adresa? _selectedAdresa;

    private readonly Action _onClose;

    public AddZamestnanecViewModel(Action onClose)
    {
        _onClose = onClose;

        // Загрузка данных для выпадающих списков
        using var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
        PobockyList = new ObservableCollection<Pobocka>(context.Pobocky.ToList());
        AdresyList = new ObservableCollection<Adresa>(context.Adresy.ToList());
    }

    [RelayCommand]
    public void Save()
    {
        try
        {
            if (SelectedPobocka == null || SelectedAdresa == null)
            {
                MessageBox.Show("Please select a branch and an address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
            var newId = AddZamestnanecToDatabase(context);

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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

        // Преобразование OracleDecimal в int
        if (idParam.Value is OracleDecimal oracleDecimal)
        {
            return oracleDecimal.ToInt32();
        }

        throw new Exception("Failed to retrieve the ID of the new Zamestnanec.");
    }


    [RelayCommand]
    public void Cancel()
    {
        _onClose.Invoke();
    }
}

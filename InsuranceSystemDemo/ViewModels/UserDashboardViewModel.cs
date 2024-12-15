using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;

namespace InsuranceSystemDemo.ViewModels
{
    public partial class UserDashboardViewModel : ObservableObject
    {
        private readonly string _currentUsername;

        
        [ObservableProperty]
        private BitmapImage? userPhoto;

    
        [ObservableProperty]
        private PojistnaSmlouva? foundPojistnaSmlouva;

     
        [ObservableProperty]
        private string policyId;

      
        public IRelayCommand UploadPhotoCommand { get; }
        public IRelayCommand SearchPolicyCommand { get; }

        public IRelayCommand DeletePhotoCommand { get; }

        public UserDashboardViewModel(string currentUsername)
        {
            _currentUsername = currentUsername;

            UploadPhotoCommand = new RelayCommand(UploadPhoto);
            SearchPolicyCommand = new RelayCommand(SearchPolicy);
            DeletePhotoCommand = new RelayCommand(DeletePhoto);

            LoadPhotoFromDatabase();
        }

        private void UploadPhoto()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a photo",
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var photoData = File.ReadAllBytes(openFileDialog.FileName);
                var fileName = openFileDialog.FileName; 

                SavePhotoToDatabase(photoData, fileName);

                UserPhoto = LoadImageFromBytes(photoData);
            }
        }


        private void LoadPhotoFromDatabase()
        {
            using var dbContext = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());

            var user = dbContext.Users.FirstOrDefault(u => u.Username == _currentUsername);

            if (user?.Photo != null)
            {
                UserPhoto = LoadImageFromBytes(user.Photo);
            }
        }

        private void SavePhotoToDatabase(byte[] photoData, string fileName)
        {
            using var dbContext = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());

            var user = dbContext.Users.FirstOrDefault(u => u.Username == _currentUsername);

            if (user != null)
            {
                user.Photo = photoData;
                user.FileExtension = Path.GetExtension(fileName).TrimStart('.').ToLower();
                dbContext.SaveChanges();
            }
        }


        private BitmapImage LoadImageFromBytes(byte[] imageData)
        {
            using var ms = new MemoryStream(imageData);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }


        private void DeletePhoto()
        {
            using var dbContext = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());

            var user = dbContext.Users.FirstOrDefault(u => u.Username == _currentUsername);

            if (user != null && user.Photo != null)
            {
                user.Photo = null;
                dbContext.SaveChanges();

                UserPhoto = null;
                MessageBoxDisplayer.ShowInfo("Photo deleted successfully.");
            }
            else
            {
                MessageBoxDisplayer.ShowError("No photo to delete.");
            }
        }

        private ObservableCollection<PojistnaSmlouva> _foundPolicies;
        public ObservableCollection<PojistnaSmlouva> FoundPolicies
        {
            get => _foundPolicies;
            set => SetProperty(ref _foundPolicies, value);
        }

        private void SearchPolicy()
        {
            if (int.TryParse(PolicyId, out int policyId))
            {
                using var dbContext = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());

                var result = dbContext.PojistneSmlouvy.Where(p => p.IdPojistky == policyId).ToList();

                if (result.Any())
                {
                    FoundPolicies = new ObservableCollection<PojistnaSmlouva>(result);
                }
                else
                {
                    FoundPolicies.Clear();
                    MessageBoxDisplayer.ShowError("Insurance policy not found.");
                }
            }
            else
            {
                MessageBoxDisplayer.ShowError("Invalid policy ID.");
            }
        }
    }
}

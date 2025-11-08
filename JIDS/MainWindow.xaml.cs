using JetInteriorApp.Data;
using JetInteriorApp.Services.Configuration;
using JetInteriorApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Windows;

namespace JetInteriorApp
{
    public partial class MainWindow : Window
    {
        private readonly Guid _userId;

        public MainWindow(Guid userId)
        {
            InitializeComponent();
            _userId = userId;

            // Build database path same way as in App.xaml.cs
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDirectory, "Data", "jetconfigs.db");

            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            var dbContext = new JetDbContext(options);
            dbContext.Database.EnsureCreated();

            var repo = new JsonConfigurationRepository(dbContext, _userId);
            var manager = new ConfigurationManager(repo, _userId);

            // Here is the key — pass the manager into VM
            DataContext = new JetConfigurationListVM(manager);
        }
    }
}

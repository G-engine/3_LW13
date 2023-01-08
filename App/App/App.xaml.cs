using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DBm;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace App
{
    public partial class App : Application
    {
        public const string DBFILENAME = "pharmacy.db";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create;
        public App()
        {
            InitializeComponent();
 
            string dbPath = DependencyService.Get<IPath>().GetDatabasePath(DBFILENAME);
            using (var db = new PharmacyContext(dbPath))
            {
                db.Database.EnsureCreated();
            }
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
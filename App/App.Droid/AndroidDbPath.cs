using Xamarin.Forms;
using System;
using System.IO;
using App.Droid;

[assembly: Dependency(typeof(AndroidDbPath))]
namespace App.Droid
{
    public class AndroidDbPath : IPath
    {
        public string GetDatabasePath(string filename)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
        }
    }
}
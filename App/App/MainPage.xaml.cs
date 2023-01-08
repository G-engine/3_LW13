using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using DBm;
using WebApp.CRUD;
using Xamarin.Forms;

namespace App
{
    public partial class MainPage : ContentPage
    {
        private string dbPath;
        public List<Employees> mylist;
        private EmployeeRepo _employeeRepo;
         
        public MainPage()
        {
            Title = "Employee Page";
            InitializeComponent ();
            dbPath = DependencyService.Get<IPath>().GetDatabasePath(App.DBFILENAME);
            _employeeRepo = new EmployeeRepo(new PharmacyContext(dbPath));
        }
        
        private async void Button_Clicked(object sender, EventArgs e)
        {

            mylist = (await _employeeRepo.GetAllAsync()).ToList();

            StringBuilder str = new StringBuilder();
            foreach (var emp in mylist)
            {
                str.Append(emp.Id);
                str.Append(' ');
                str.Append(emp.Name);
                str.Append(": ");
                str.Append(emp.Job);
                str.Append(", salary: ");
                str.Append(emp.Salary);
                str.Append(", schedule: ");
                str.Append(emp.Schedule);
                str.Append(", vac: ");
                str.Append(emp.InVacation);
                str.Append('\n');
            }

            text.Text = str.ToString();
        }
        
        private async void Button_Add_Clicked(object sender, EventArgs e)
        {
            var v = editor.Text.Split('+');
            Employees employee = new Employees(v[0], v[1], Int32.Parse(v[2]), v[3], Int32.Parse(v[4]));
            await _employeeRepo.CreateAsync(employee);
            text.Text = "sent";
        }

        private async void Button_Delete_Clicked(object sender, EventArgs e)
        {
            var v = editor.Text.Split('\n', ' ');
            await _employeeRepo.DeleteAsync(Int32.Parse(v[0]));
            text.Text = "deleted";
        }
        
        private async void Button_Update_Clicked(object sender, EventArgs e)
        {
            var v = editor.Text.Split('+');
            Employees employee = new Employees(v[1], v[2], Int32.Parse(v[3]), v[4], Int32.Parse(v[5]))
                { Id = Int32.Parse(v[0]) };
            await _employeeRepo.UpdateAsync(employee.Id, employee);
            text.Text = "updated";
        }
        
        private async void Button_Supplier_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SupplierPage());
        }
    }
}
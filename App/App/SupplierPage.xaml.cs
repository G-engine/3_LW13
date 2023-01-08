using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBm;
using WebApp.CRUD;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SupplierPage : ContentPage
    {
        private string dbPath;
        public List<Suppliers> mylist;
        private SupplierRepo _supplierRepo;
        private ProductRepo pRepo;
         
        public SupplierPage()
        {
            Title = "Supplier Page";
            InitializeComponent ();
            dbPath = DependencyService.Get<IPath>().GetDatabasePath(App.DBFILENAME);
            _supplierRepo = new SupplierRepo(new PharmacyContext(dbPath));
            pRepo = new ProductRepo(new PharmacyContext(dbPath));
        }
        
        private async void Button_Clicked(object sender, EventArgs e)
        {

            mylist = (await _supplierRepo.GetAllAsync()).ToList();

            StringBuilder str = new StringBuilder();
            foreach (var sup in mylist)
            {
                str.Append(sup.Id);
                str.Append(' ');
                str.Append(sup.Name);
                str.Append(": ");
                foreach (var p in sup.Products)
                {
                    str.Append(" SN:");
                    str.Append(p.SerialNumber);
                    str.Append(", ");
                    str.Append(p.Name);
                    str.Append(';');
                }
                str.Append('\n');
            }

            text.Text = str.ToString();
        }
        
        private async void Button_Add_Clicked(object sender, EventArgs e)
        {
            var v = editor.Text.Split('+');
            Suppliers supplier = new Suppliers() { Name = v[0] };
            Products product = new Products(Int32.Parse(v[2]), v[1], Int32.Parse(v[3]), Int32.Parse(v[4]), supplier);
            supplier.Products.Add(product);
            await _supplierRepo.CreateAsync(supplier);
            text.Text = "sent";
        }

        private async void Button_Delete_Clicked(object sender, EventArgs e)
        {
            var v = editor.Text.Split('\n', ' ');
            var existing = await _supplierRepo.GetAsync(Int32.Parse(v[0]));
            if (existing != null)
            {
                foreach (var p in existing.Products.ToList())
                    await pRepo.DeleteAsync(p.Id);
                await _supplierRepo.DeleteAsync(Int32.Parse(v[0]));
            }
            text.Text = "deleted";
        }
        
        private async void Button_Update_Clicked(object sender, EventArgs e)
        {
            var v = editor.Text.Split('+');
            Suppliers supplier = new Suppliers { Id = Int32.Parse(v[0]), Name = v[1] };
            await _supplierRepo.UpdateNameAsync(supplier.Id, supplier);
            text.Text = "updated";
        }
        
        private async void Button_Product_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductPage());
        }
    }
}
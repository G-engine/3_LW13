using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBm;
using WebApp.CRUD;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductPage : ContentPage
    {
        private string dbPath;
        public List<Products> mylist;
        private ProductRepo _productRepo;
        private SupplierRepo sRepo;
         
        public ProductPage()
        {
            Title = "Product Page";
            InitializeComponent ();
            dbPath = DependencyService.Get<IPath>().GetDatabasePath(App.DBFILENAME);
            _productRepo = new ProductRepo(new PharmacyContext(dbPath));
            sRepo = new SupplierRepo(new PharmacyContext(dbPath));
        }
        
        private async void Button_Clicked(object sender, EventArgs e)
        {

            mylist = (await _productRepo.GetAllAsync()).ToList();

            StringBuilder str = new StringBuilder();
            foreach (var p in mylist)
            {
                str.Append(p.Id);
                str.Append(' ');
                str.Append(p.Name);
                str.Append(", SN: ");
                str.Append(p.SerialNumber);
                str.Append(", price: ");
                str.Append(p.Price);
                str.Append(", number: ");
                str.Append(p.Number);
                str.Append(", supplier: ");
                str.Append(p.SupplierId);
                str.Append('\n');
            }

            text.Text = str.ToString();
        }
        
        private async void Button_Add_Clicked(object sender, EventArgs e)
        {
            var v = editor.Text.Split('+');
            var product = new Products(Int32.Parse(v[1]), v[0], Int32.Parse(v[2]), Int32.Parse(v[3]), Int32.Parse(v[4]));
            await _productRepo.CreateAsync(product);
            text.Text = "sent";
        }

        private async void Button_Delete_Clicked(object sender, EventArgs e)
        {
            var v = editor.Text.Split('\n', ' ');
            await _productRepo.DeleteAsync(Int32.Parse(v[0]));
            text.Text = "deleted";
        }
        
        private async void Button_Update_Clicked(object sender, EventArgs e)
        {
            var v = editor.Text.Split('+');
            var product = new Products(Int32.Parse(v[2]), v[1], Int32.Parse(v[3]), Int32.Parse(v[4]), Int32.Parse(v[5]))
                { Id = Int32.Parse(v[0]) };
            Suppliers supplier = await sRepo.GetAsync(product.SupplierId);
            supplier.Products.Remove(supplier.Products.FirstOrDefault(p => p.Id == product.Id));
            supplier.Products.Add(product);
            await _productRepo.UpdateAsync(product.Id, product);
            await sRepo.UpdateAsync(product.SupplierId, supplier);
            text.Text = "updated";
        }
    }
}
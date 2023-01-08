using System;
using System.Collections.Generic;

namespace DBm;

public partial class Suppliers
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();
}

using System.Linq;

public class ManufacturerUtils
{
    private static readonly string[] _manufacturers = {
        "Acura",
        "Alfa Romeo",
        "Alpine",
        "Aston Martin",
        "Audi",
        "Bentley",
        "BMW",
        "Cadillac",
        "Chevrolet",
        "Corvette",
        "Cupra",
        "Cadillac",
        "Ferrari",
        "Ford",
        "Ginetta",
        "Honda",
        "Hyundai",
        "Infiniti",
        "Isotta Franchini",
        "KTM",
        "Lamborghini",
        "Lexus",
        "Ligier",
        "Lynk & Co",
        "Maserati",
        "Ford",
        "McLaren",
        "Mercedes",
        "Nissan",
        "Norma",
        "Oreca",
        "Opel",
        "Panoz",
        "Peugeot",
        "Porsche",
        "Renault",
        "Toyota",
        "Vauxhall",
        "Volkswagen"
    };

    public static string GetManufacturer(string value)
    {
        return _manufacturers.Where(m => value.ToLower().StartsWith(m.ToLower())).FirstOrDefault();
    }
}


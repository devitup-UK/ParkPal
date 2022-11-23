namespace ParkPal.Common.Models;

public class Destination
{
    public string DestinationId { get; set; }
    public string Name { get; set; }

    public string Image => DestinationId + ".jpeg";

    public string Location
    {
        get
        {
            switch (DestinationId)
            {
                case "e957da41-3552-4cf6-b636-5babc5cbc4e5":
                case "89db5d43-c434-4097-b71f-f6869f495a22":
                    return "Florida, United States";
                case "e8d0207f-da8a-4048-bec8-117aa946b2c2":
                    return "Paris, France";
                case "9fc68f1c-3f5e-4f09-89f2-aab2cf1a0741":
                case "bfc89fd6-314d-44b4-b89e-df1a89cf991e":
                    return "Los Angeles, California";
                case "818f544a-38db-4255-b5db-6bf2cb39b7b3":
                    return "Chertsey, England";
                case "8e6bf2ae-77ac-403d-8e10-d7cd9b6c05d7":
                    return "Stoke-on-Trent, England";
            }
            return null;
        }
    }

    public List<Park> Parks { get; set; }

    public Destination(string destinationId, string name)
    {
        DestinationId = destinationId;
        Name = name;
        Parks = new List<Park>();
    }
    
    public bool Hidden
    {
        get
        {
            if(!String.IsNullOrEmpty(DestinationId)) {
                switch (DestinationId) {
                    case "":
                        return true;
                }
            }
            return false;
        }
    }
}
namespace ParkPal.Common.Models;

public class Destination
{
    public string DestinationId { get; set; }
    public string Name { get; set; }

    public string Image => DestinationId + ".jpeg";

    public string? Location
    {
        get
        {
            switch (DestinationId)
            {
                case "e957da41-3552-4cf6-b636-5babc5cbc4e5":
                case "89db5d43-c434-4097-b71f-f6869f495a22":
                case "7a4adf8d-8c3f-4300-b277-19707e4f8e12":
                    return "Florida, United States";
                case "e8d0207f-da8a-4048-bec8-117aa946b2c2":
                    return "Paris, France";
                case "9fc68f1c-3f5e-4f09-89f2-aab2cf1a0741":
                case "bfc89fd6-314d-44b4-b89e-df1a89cf991e":
                    return "Los Angeles, California";
                case "818f544a-38db-4255-b5db-6bf2cb39b7b3":
                    return "Chertsey, United Kingdom";
                case "8e6bf2ae-77ac-403d-8e10-d7cd9b6c05d7":
                    return "Stoke-on-Trent, United Kingdom";
                case "abcfffe7-01f2-4f92-ae61-5093346f5a68":
                    return "Lantau Island, Hong Kong";
                case "faff60df-c766-4470-8adb-dee78e813f42":
                    return "Urayasu, Japan";
                case "9c6a0987-e519-4d6e-b011-e6c47a60641b":
                    return "Biddinghuizen, Netherlands";
                case "c0eddd5b-da82-4161-9a5f-2eb4ab5f82e7":
                    return "De Panne, Belgium";
                case "85e3b542-af91-4f8a-8d28-445868a7c8fd":
                    return "Rust, Germany";
                case "8fba5a14-8d04-455c-acf8-eccaaa0f58d9":
                case "c4231018-dc6f-4d8d-bfc2-7a21a6c9e9fa":
                    return "Missouri, United States";
                case "6c3cd0cc-57b5-431b-926c-2658e8104057":
                    return "Tennessee, United States";
                case "0257ff9f-c73c-4855-b5b4-774755c4d146":
                    return "Brühl, Germany";
                case "22489737-c1f7-4ac9-b79b-b664e5efd866":
                    return "Rhineland-Palatinate, Germany";
                case "6e1c96c1-dafc-4c26-a3d3-1b28c888daa8":
                    return "Pennsylvania, United States";
                case "ee2ec4b5-3bc3-403c-9e30-7fa607e6311e":
                    return "Ohio, United States";
                case "c4307928-fc3c-47df-b976-305026751727":
                    return "Chessington, United Kingdom";
                case "5dd95124-888c-449d-9a65-46d7ecc8878c":
                    return "Texas, United States";
                case "a2cfe9e9-6734-4b9e-90c2-6427fa303e5c":
                    return "New England, United States";
                case "0976f1b8-5782-4cda-887f-dc1d537d8d6e":
                    return "Soltau, Germany";
                case "b1387675-bb84-4eb4-aae6-af96d586c3d6":
                    return "Gothenburg, Sweden";
                case "d6e6386c-02c0-4e0e-b1f4-e9f831a1d3e6":
                    return "Majivali, India";
                case "21776b5a-1444-4924-8ab2-6c66d9219628":
                    return "Kaatsheuvel, Netherlands";
                case "c11ad942-8f5d-4642-862c-30d8b7a414e3":
                    return "Kronenberg, Netherlands";
                case "6cc48df2-f126-4f28-905d-b4c2c15765f2":
                    return "Plailly, France";
                case "6e1464ca-1e9b-49c3-8937-c5c6f6675057":
                    return "Shanghai, China";
                case "8d8e39eb-4b0a-48b9-ac67-444dd6e97519":
                    return "New Jersey, United States";
                case "17e01e63-d22f-414f-b65b-1786acbd918c":
                    return "California, United States";
                case "7c48a21b-221c-42f8-8339-6334c5f2fb12":
                    return "Windsor, United Kingdom";
                case "96fc6528-d143-4c6c-a2ac-01e3c1192d21":
                    return "Illinois, United States";
                case "119fce4a-9662-484f-ac3a-d62d16bdc7ab":
                    return "California, United States";
                case "da6388a0-cbfe-49f9-9c63-12e1f63dda62":
                    return "Texas, United States";
                case "b3c6033a-4dc2-476c-98d2-ce31c6b961a7":
                    return "Maryland, United States";
                case "be901819-52b2-4a98-8f31-5fecb993bcd6":
                    return "Province of Verona, Italy";
                case "d0f897aa-0598-4219-abbc-50b95985da01":
                    return "Oklahoma, United States";
                case "32fd247c-f2dd-44d6-aa2b-10c158df162f":
                    return "California, United States";
                case "a8ea944a-5ab7-42ed-bb02-ed08e64f125a":
                    return "Georgia, United States";
                case "dfa64b30-f5af-444c-a7f8-3db78537a0f8":
                    return "New York, United States";
                case "b23d90e6-47f4-4258-8690-e74d777fca0f":
                    return "Ontario, Canada";
                case "71f82221-5f3e-4e2c-b09c-31c149e0dd59":
                    return "Virginia, United States";
                case "1f1f9558-4e81-48a7-aad5-9879b633802b":
                    return "California, United States";
                case "211e981b-ee64-4ff9-8b06-0abf26e63874":
                    return "Texas, United States";
                case "643e837e-b244-4663-8d3a-148c26ecba9c":
                    return "Florida, United States";
                case "d5f3aa8d-2ef9-4436-9829-b1f6774f592b":
                    return "Pennsylvania, United States";
                case "025c75ed-80a8-4bba-8b80-192e2ceff58c":
                    return "North Carolina, United States";
                case "6a33b034-2e39-46ea-8808-a06b29b9b2d6":
                    return "Michigan, United States";
                case "694e1f6e-d6a2-4c86-9749-5da1a9cb8924":
                    return "Ohio, United States";
                case "d4c0e0c4-18c6-4918-a505-209d839c2615":
                    return "Minnesota, United States";
                case "b1444147-b93a-4f73-b12d-28f9b1f7ec7c":
                    return "California, United States";
                case "498b1747-cc17-4490-aee9-a45147f0f706":
                    return "Tarragona, Spain";
                case "0704cf65-5c67-42f3-a054-f45e03a412cf":
                    return "Virginia, United States";
                case "1d92560c-474f-4425-906d-e9dd2f2da6ca":
                    return "Florida, United States";
                case "85c5cdc5-95c3-4190-9a05-9707b634889d":
                    return "Ieper, Belgium";
                case "314e42e5-95e7-448f-a127-01699a4fba04":
                    return "Missouri, United States";
                case "6d6099c4-170d-4beb-85c2-73b26249eead":
                    return "Montreal, Canada";
                case "271f07d8-9dcc-4529-925e-8760be79ffcd":
                    return "Mexico, United States";
                case "be4e3681-7e3c-43a5-89e6-bb4863d8fe35":
                    return "California, United States";
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
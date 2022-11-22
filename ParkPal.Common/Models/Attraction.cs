using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ParkPal.Common.Models.Database.Entities.Notification;
using ParkPal.Common.Models.Enums;

namespace ParkPal.Common.Models;

public class Attraction
{
    public string AttractionId { get; set; }
    public string Name { get; set; }
    public AttractionStatus Status { get; set; }
    public string Image => AttractionId + ".jpeg";

    public bool Thrill
    {
        get
        {
            if(!String.IsNullOrEmpty(AttractionId)) {
                switch (AttractionId) {
                    case "de3309ca-97d5-4211-bffe-739fed47e92f":
                    case "9d4d5229-7142-44b6-b4fb-528920969a2c":
                    case "a5241f3b-4ab5-4902-b5ba-435132ef553d":
                    case "b2260923-9315-40fd-9c6b-44dd811dbe64":
                    case "37ae57c5-feaf-4e47-8f27-4b385be200f0":
                    case "6f6998e8-a629-412c-b964-2cb06af8e26b":
                    case "1a2e70d9-50d5-4140-b69e-799e950f7d18":
                    case "e516f303-e82d-4fd3-8fbf-8e6ab624cf89":
                    case "64a6915f-a835-4226-ba5c-8389fc4cade3":
                    case "24cf863c-b6ba-4826-a056-0b698989cbf7":
                    case "ec25d9a7-b4d4-4ebf-a6c4-c18389351764":
                    case "70ac72a3-9675-4c41-a1b1-e4801072927a":
                    case "2c72d1d0-7106-439d-9672-5bf95795ccea":
                    case "578bbd12-1975-4ec3-9879-ea641c780342":
                    case "61079a31-4165-4fb0-b36f-c01c5971f80a":
                    case "fa743143-281b-4b5b-b87b-d49fcb006772":
                    case "2f95b213-daaa-4370-8349-c2cd57be470e":
                    case "db5b2165-15c2-4e51-8bd1-611e9c351866":
                    case "905d7888-b866-4e74-90d1-07fc6ef6706f":
                    case "6af80308-647d-4d8b-bcf6-37517a93bdbc":
                    case "5d0ce227-7ad0-4402-a95b-6cf56f25a8ec":
                    case "ddca340c-7ba1-4f23-89eb-0d3d52c84bda":
                    case "f0d4b531-e291-471b-9527-00410c2bbd65":
                    case "d2ec9363-a215-4904-b297-b66734ea9a00":
                    case "15bad9c3-8378-4ac5-ab90-2f8d0ee09d26":
                    case "ff2e7d6f-5aaf-47b8-8695-53a66e9fe073":
                    case "a10b25e2-3176-449a-a8a5-4119902887bb":
                    case "73cc9242-3eea-4a34-8553-9aded86329dc":
                    case "8215f2cf-6356-421d-80fa-0e9b26f57bcd":
                    case "9a706e7e-1e52-4603-b170-86c9b8243fc6":
                        return true;
                }
            }
            return false;
        }
    }

    public bool Hidden
    {
        get
        {
            if(!String.IsNullOrEmpty(AttractionId)) {
                switch (AttractionId) {
                    case "4bef7560-ed81-47c7-b178-6544abe3daaf":
                    case "15700490-3749-45cf-a737-3cba56e13704":
                    case "2ebfb38c-5cb5-4de1-86c0-f7af14188022":
                    case "30fe3c64-af71-4c66-a54b-aa61fd7af177":
                    case "0f57cecf-5502-4503-8bc3-ba84d3708ace":
                    case "52ac3730-b955-452a-a7cc-17e3b06182ac":
                    case "365816dc-e99c-4b55-bafa-ba2cecd3ed96":
                    case "90d79335-c907-4069-a021-d0fe1ec73ae2":
                    case "f010bc01-b450-4476-a5f3-a5f2813104b2":
                    case "cf9d12fb-1e06-4c8e-ae6f-1e7f1e5c861c":
                    case "888fb4a4-7adf-47a1-8ba2-c258cc64fd75":
                    case "de737ffc-306b-4f32-8bbb-34e5d370ec8f":
                    case "4d34dd80-4042-4ada-a74f-ef35a0887b4f":
                    case "37bb0aca-0dcf-4fa0-bc03-81d1b4ef9ac0":
                    case "7969166f-feef-4350-b26e-6a6c745528f4":
                    case "3e5f26ee-c02d-47fd-891e-5e4479073444":
                    case "66ff36de-9cb3-4d9a-b891-1665d19ffb3e":
                    case "0f40274d-420a-425a-9377-29fd6e49484f":
                    case "4f0df9e7-d4c1-45b5-93e2-4a7bc92547b0":
                    case "3d8f8f8f-f984-4d2e-8dea-5a79432bdf05":
                    case "2ecc4fff-2994-476f-9926-24a4af173838":
                    case "9053240b-7f7f-44fe-970b-bd7956cd5d4f":
                    case "9244365a-ffad-4ea1-ad8d-a36ac3a0e383":
                    case "86ea784f-eed1-4696-ac01-ebb65589bf0e":
                    case "3ace01d1-15fc-4fbb-99e4-81a696cb2d05":
                    case "329c91db-3b4f-40e5-adaa-f902e6f450f9":
                    case "ade9aa6d-2eb0-4046-b04d-a0f98d0682d9":
                    case "18c533d6-a395-4ae4-9488-80fce9c497fe":
                    case "6f1d3b25-42c9-4e99-9dce-6c20d7a5deea":
                    case "8f8746cb-c714-4c60-848d-e2dc4e6f586b":
                    case "61fb49f8-e62f-4e1c-ae0e-8ab9929037bc":
                    case "07dbeaea-85fa-45f2-872f-02f9e7510419":
                    case "35ed719b-f7f0-488f-8346-4fbf8055d373":
                    case "8c8cd77d-97f6-4309-b285-42aad90e9f15":
                    case "e3549451-b284-453d-9c31-e3b1207abd79":
                    case "482169b9-2889-4747-8aef-f9d13a37d940":
                    case "ee070d46-6a64-41c0-9f12-69dcfcca10a0":
                    case "00666fe9-7774-4b53-9fb7-3d333f8aa503":
                    case "20b61c85-1c69-4576-8d8e-ba7b16915577":
                    case "380c7bc4-646e-439a-8699-be1cd603a36d":
                    case "d581fdde-6679-46e5-821d-a33c9b4cc7ed":
                    case "a86b36a4-a439-43cd-8994-4030a159e261":
                    case "bc1ffa86-9b1a-4ce9-84a5-b479dfa3cb53":
                    case "a15ce7cf-342a-4c7a-9372-7a1fa1054747":
                    case "f39dbb08-35db-4ad4-885d-e4787de5bde6":
                    case "1dd9eb9b-ce6d-451b-b8ef-f39b3435b35a":
                    case "125d9166-634b-41ec-a29c-182bb0a71dba":
                    case "7f97be37-1210-462a-8039-1751c9b0b6a8":
                    case "6fbe6d02-4057-43bb-80a3-047b1e8a50ca":
                    case "759e7496-565c-420f-b0bd-777dfd598f08":
                    case "3655bbe2-8a41-4f35-a99f-e4d9d582c5fa":
                    case "1897c9b3-436c-4046-937d-b6ff98233177":
                    case "c616884f-c55f-4f74-b58d-1a8374717380":
                    case "239d5e6c-28c4-4ed4-80a9-d332692c0758":
                    case "d04ec77c-c082-46fd-a253-c7c69bb14374":
                    case "127eb7ec-9d9f-43bc-a259-9eb03c467fef":
                    case "f0e0d1a3-b1a1-42a1-b514-df96b7951db2":
                    case "97552d3f-8ddf-4aa5-b7b1-e14d5bd05568":
                    case "dd0392bc-2ebb-48ab-b1b5-1af1193acee2":
                    case "f9e169bf-1e27-47c9-9f0f-ce8c6df4da53":
                    case "811984d7-56f4-4ca6-848d-896d46aca846":
                    case "4edb7625-3f86-4da6-9a31-c0cd72fa94fc":
                    case "0941f812-f214-4837-9e67-ee1216ceed50":
                    case "8b2bcfc1-23f6-4893-aca8-8225b1884a07":
                    case "17ecfc8c-8013-4d48-805b-f37f94cc2452":
                    case "904f17dd-0ef3-4794-be7e-58b884bf81b5":
                    case "861bf387-f362-4904-9082-99e093efa6ee":
                    case "b9be5b8f-fe1f-4a77-bcad-5399429ecaa6":
                    case "bc997600-fcc0-4f6f-b908-a1419b26cfd8":
                    case "4d27b0d7-2b0a-4569-90fa-e79f117ec7ef":
                    case "43e5558e-2be0-4989-b80b-074afa8302a9":
                    case "1a8ea967-229a-42a0-8290-59b036c84e14":
                    case "3b7309a5-7b57-4edf-b4e6-36ad958ac21e":
                    case "4f391f0e-52be-4f9d-99d6-b3ae0373b43c":
                    case "e7976e25-4322-4587-8ded-fb1d9dcbb83c":
                    case "6ef1b126-5b0b-46a1-8608-4fcf98ab92c8":
                    case "d6c61231-8f9b-40fa-943a-fe1454e185e7":
                    case "ced66356-4eca-482d-86ef-958e4a7dc87f":
                    case "9056906c-0030-4951-be5e-44ac1f273491":
                    case "7ff5d512-aed5-479a-8b3d-9bd1efb4177b":
                    case "78b5e028-1574-4ea5-9ac1-8d5356400044":
                    case "03871aa6-a0b5-41ee-8fd3-b2f4b8dccc79":
                    case "8ed1472a-4d2e-47f1-b848-353de0bc3670":
                        return true;
                }
            }
            return false;
        }
    }

    public int? WaitTime { get; set; }

    public Attraction(string attractionId, string name, AttractionStatus status, int? waitTime)
    {
        AttractionId = attractionId;
        Name = name;
        Status = status;
        WaitTime = waitTime;
    }
}
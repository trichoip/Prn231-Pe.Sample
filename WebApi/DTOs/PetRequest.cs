using DataAccess.Models;
using WebApi.Mappings;

namespace WebApi.DTOs;
public class PetRequest : IMapFrom<Pet>
{
    public int PetId { get; set; }

    public string PetName { get; set; } = default!;

    public DateTime ImportDate { get; set; }

    public string PetDescription { get; set; } = default!;

    public int Quantity { get; set; }

    public double PetPrice { get; set; }

    public string PetGroupId { get; set; } = default!;
}

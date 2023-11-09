using AutoMapper;
using DataAccess.Models;
using WebApi.Mappings;

namespace WebApi.DTOs;
public class PetResponse : IMapFrom<Pet>
{
    public int PetId { get; set; }

    public string PetName { get; set; } = null!;

    public DateTime? ImportDate { get; set; }

    public string? PetDescription { get; set; }

    public int? Quantity { get; set; }

    public double? PetPrice { get; set; }

    public string? PetGroupId { get; set; }

    public string PetGroupName { get; set; } = default!;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Pet, PetResponse>()
            .ForMember(d => d.PetGroupName, opt => opt.MapFrom(s => s.PetGroup!.PetGroupName));
    }
}

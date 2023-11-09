using AutoMapper;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Repository.Helpers;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "2")]
    public class PetsController : ControllerBase
    {
        private readonly ServiceBase<Pet> _service;
        private readonly ServiceBase<PetGroup> _servicePetGroup;
        private readonly IMapper _mapper;

        public PetsController(
            ServiceBase<Pet> service,
            ServiceBase<PetGroup> servicePetGroup,
            IMapper mapper)
        {
            _service = service;
            _servicePetGroup = servicePetGroup;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<PetResponse>>> GetPets(
            string? PetGroupId,
            DateTime? MinImportDate,
            DateTime? MaxImportDate,
            int pageIndex,
            int pageSize)
        {
            var pet = await _service.FindAsync<PetResponse>(
                pageIndex,
                pageSize,
                p => (string.IsNullOrEmpty(PetGroupId) || p.PetGroupId == PetGroupId) &&
                     (!MinImportDate.HasValue || p.ImportDate >= MinImportDate) &&
                     (!MaxImportDate.HasValue || p.ImportDate <= MaxImportDate));
            return Ok(pet.ToPaginatedResponse());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetResponse>> GetPet(int id)
        {
            var pet = await _service.FindByAsync(p => p.PetId == id);
            if (pet == null)
            {
                return Problem(detail: $"pet id {id} not found", statusCode: 404);
            }
            var petResponse = _mapper.Map<PetResponse>(pet);
            return Ok(petResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPet(int id, PetRequest petRequest)
        {
            if (id != petRequest.PetId)
            {
                return Problem(detail: $"pet id {id} not match with pet id {petRequest.PetId}", statusCode: 400);
            }

            var pet = await _service.FindByAsync(p => p.PetId == id);
            if (pet == null)
            {
                return Problem(detail: $"pet id {id} not found", statusCode: 404);
            }

            if (!await _servicePetGroup.ExistsByAsync(p => p.PetGroupId == petRequest.PetGroupId))
            {
                return Problem(detail: $"pet group id {petRequest.PetGroupId} not found", statusCode: 404);
            }

            _mapper.Map(petRequest, pet);
            await _service.UpdateAsync(pet);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Pet>> PostPet(PetRequest petRequest)
        {
            if (await _service.ExistsByAsync(p => p.PetId == petRequest.PetId))
            {
                return Problem(detail: $"pet id {petRequest.PetId} already exists", statusCode: 400);
            }

            if (!await _servicePetGroup.ExistsByAsync(p => p.PetGroupId == petRequest.PetGroupId))
            {
                return Problem(detail: $"pet group id {petRequest.PetGroupId} not found", statusCode: 404);
            }

            var pet = _mapper.Map<Pet>(petRequest);
            await _service.CreateAsync(pet);
            return CreatedAtAction(nameof(GetPet), new { id = pet.PetId }, _mapper.Map<PetResponse>(pet));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            var pet = await _service.FindByAsync(p => p.PetId == id);
            if (pet == null)
            {
                return Problem(detail: $"pet id {id} not found", statusCode: 404);
            }
            await _service.DeleteAsync(pet);
            return NoContent();
        }
    }
}

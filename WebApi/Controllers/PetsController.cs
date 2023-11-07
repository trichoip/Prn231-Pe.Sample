using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // de keu role nao ghi vao do
    [Authorize(Roles = "2")]
    public class PetsController : ControllerBase
    {
        private readonly ServiceBase<Pet> _service;
        private readonly ServiceBase<PetGroup> _servicePetGroup;

        public PetsController(
            ServiceBase<Pet> service,
            ServiceBase<PetGroup> servicePetGroup)
        {
            _service = service;
            _servicePetGroup = servicePetGroup;
        }

        // GET: api/Pets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPets(string PetGroupId, DateTime? MinImportDate, DateTime? MaxImportDate)
        {

            if (MinImportDate == null || MaxImportDate == null)
            {
                return await _service.FindAllAsync(p => p.PetGroupId == PetGroupId);
            }

            return await _service.FindAllAsync(
                        p => p.PetGroupId == PetGroupId &&
                             p.ImportDate >= MinImportDate &&
                             p.ImportDate <= MaxImportDate);
        }

        // GET: api/Pets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetPet(int id)
        {
            var pet = await _service.FindOneAsync(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            return pet;
        }

        // PUT: api/Pets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPet(int id, Pet pet)
        {
            if (id != pet.PetId)
            {
                return BadRequest();
            }

            var entity = await _service.FindOneAsync(p => p.PetId == id);
            if (entity == null)
            {
                return NotFound();
            }

            var petGroup = await _servicePetGroup.FindOneAsync(p => p.PetGroupId == pet.PetGroupId);
            if (petGroup == null)
            {
                return NotFound();
            }

            await _service.UpdateAsync(pet);

            return NoContent();
        }

        // POST: api/Pets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pet>> PostPet(Pet pet)
        {
            var entity = await _service.FindOneAsync(p => p.PetId == pet.PetId);
            if (entity != null)
            {
                return BadRequest();
            }

            var petGroup = await _servicePetGroup.FindOneAsync(p => p.PetGroupId == pet.PetGroupId);
            if (petGroup == null)
            {
                return NotFound();
            }

            await _service.CreateAsync(pet);

            return CreatedAtAction("GetPet", new { id = pet.PetId }, pet);
        }

        // DELETE: api/Pets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {

            var entity = await _service.FindOneAsync(p => p.PetId == id);
            if (entity == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(entity);

            return NoContent();
        }
    }
}

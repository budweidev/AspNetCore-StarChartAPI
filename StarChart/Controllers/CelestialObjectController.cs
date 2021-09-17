using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;
using System.Collections.Generic;
using System.Linq;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestial = _context.CelestialObjects.Find(id);

            if (celestial is null)
            {
                return NotFound();
            }

            celestial.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == id).ToList();

            return Ok(celestial);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestials = _context.CelestialObjects.Where(oc => oc.Name == name).ToList();

            if (celestials is null || !celestials.Any())
            {
                return NotFound();
            }

            SetSatellites(celestials);

            return Ok(celestials);
        }


        [HttpGet("", Name = "GetAll")]
        public IActionResult GetAll()
        {
            var celestials = _context.CelestialObjects.ToList();

            if (celestials is null || !celestials.Any())
            {
                return NotFound();
            }

            SetSatellites(celestials);

            return Ok(celestials);
        }

        [HttpPost(Name = "Create")]
        public IActionResult Create([FromBody]CelestialObject celestialToCreate)
        {
            _context.CelestialObjects.Add(celestialToCreate);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialToCreate.Id }, celestialToCreate);
        }

        [HttpPut("{id}", Name = "Update")]
        public IActionResult Update(int id, CelestialObject newCelestial)
        {
            var celestialToUpdate = _context.CelestialObjects.Find(id);

            if (celestialToUpdate is null)
            {
                return NotFound();
            }

            celestialToUpdate.Name = newCelestial.Name;
            celestialToUpdate.OrbitalPeriod = newCelestial.OrbitalPeriod;
            celestialToUpdate.OrbitedObjectId = newCelestial.OrbitedObjectId;
            _context.CelestialObjects.Update(celestialToUpdate);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialToUpdate = _context.CelestialObjects.Find(id);

            if (celestialToUpdate is null)
            {
                return NotFound();
            }

            celestialToUpdate.Name = name;
            _context.CelestialObjects.Update(celestialToUpdate);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}", Name = "Delete")]
        public IActionResult Delete(int id)
        {
            var celestialsToDelete = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id);

            if (celestialsToDelete is null || !celestialsToDelete.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialsToDelete);
            _context.SaveChanges();

            return NoContent();
        }

        private void SetSatellites(List<CelestialObject> celestials)
        {
            foreach (var celestial in celestials)
            {
                celestial.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestial.Id).ToList();
            }
        }
    }
}

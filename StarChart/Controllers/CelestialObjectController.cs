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

        private void SetSatellites(List<CelestialObject> celestials)
        {
            foreach (var celestial in celestials)
            {
                celestial.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestial.Id).ToList();
            }
        }
    }
}

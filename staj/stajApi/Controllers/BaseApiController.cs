using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stajApi.Data;

namespace stajApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController<T> : ControllerBase where T : class
    {
        protected readonly ApplicationDbContext _context;

        public BaseApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public virtual IActionResult GetAll()
        {
            var data = _context.Set<T>().ToList();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public virtual IActionResult GetById(int id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity == null)
                return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public virtual IActionResult Create([FromBody] T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = (entity as dynamic).Id }, entity);
        }

        [HttpPut("{id}")]
        public virtual IActionResult Update(int id, [FromBody] T entity)
        {
            if (id != (entity as dynamic).Id)
                return BadRequest("ID uyumsuzluğu");

            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(int id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity == null)
                return NotFound();

            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

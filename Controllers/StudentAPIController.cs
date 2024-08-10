using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPCoreWebAPICRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPCoreWebAPICRUD.Controllers
{
    [Route("api/[controller]")]
    public class StudentAPIController : Controller
    {
        private readonly My_dbContext context;

        public StudentAPIController(My_dbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public  async Task<ActionResult<List<Student>>> GetStudents()
        {
            var data = await context.Students.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudentById(int id)
        {
            var student = await context.Students.FindAsync(id);
            if(student == null)
            {
                return NotFound();
            }
            return student;
        }
        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student std)
        {
            await context.Students.AddAsync(std);
            await context.SaveChangesAsync();

            return Ok(std);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Student>> UpdateStudent(int id, Student std)
        {
            if (id != std.Id)
            {
                return BadRequest();
            }

            context.Entry(std).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    // Fetch the current values from the database to handle concurrency conflict
                    var currentValues = await context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                    if (currentValues == null)
                    {
                        return NotFound();
                    }

                    // Optionally, you can log the conflict or inform the user about the concurrency issue
                    return Conflict(new { message = "Concurrency conflict detected. The record has been modified by another user." });
                }
            }

            return Ok(std);
        }

        private bool StudentExists(int id)
        {
            return context.Students.Any(e => e.Id == id);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> DeleteStudent(int id)
        {
            var std  = await context.Students.FindAsync(id);

            if(std == null)
            {
                return NotFound();
            }
            context.Students.Remove(std);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}


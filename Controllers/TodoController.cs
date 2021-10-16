using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTodo.Data;
using MyTodo.Models;
using MyTodo.ViewModels;

namespace MyTodo.Controllers
{
    [ApiController]
    [Route("v1")]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        [Route("todos")]
        public async Task<IActionResult> Get([FromServices] AppDbContext context)
        {
            var todos = await context.Todos
                .AsNoTracking()
                .ToListAsync();

            return Ok(todos);
        }

        [HttpGet]
        [Route("todos/{id}")]
        public async Task<IActionResult> GetById([FromServices] AppDbContext context, [FromRoute] int id)
        {
            var todo = await context.Todos
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null)
                return NotFound();

            return Ok(todo);
        }

        [HttpPost("todos")]
        public async Task<IActionResult> Create([FromServices] AppDbContext context, [FromBody] CreateTodoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var todo = new Todo
            {
                Title = model.Title
            };

            try
            {
                await context.Todos.AddAsync(todo);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }

            return Created($"v1/todos/{todo.Id}", todo);
        }

        [HttpPut("todos/{id}")]
        public async Task<IActionResult> Put([FromServices] AppDbContext context, [FromRoute] int id, [FromBody] CreateTodoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var todo = await context.Todos.FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null)
                return NotFound();

            todo.Title = model.Title;

            try
            {
                context.Todos.Update(todo);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }

            return Ok(todo);
        }

        [HttpDelete("todos/{id}")]
        public async Task<IActionResult> Delete([FromServices] AppDbContext context, [FromRoute] int id)
        {
            var todo = await context.Todos.FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null)
                return NotFound();

            try
            {
                context.Todos.Remove(todo);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }

            return Ok();
        }
    }
}
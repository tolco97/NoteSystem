using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NoteSystem.DomainModel;
using NoteSystem.Repositories.Interfaces;

namespace NoteSystem.Server.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;

        public NotesController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        // GET: notes/
        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            IList<Note> allNotes = await _noteRepository.GetAllNotes();

            // 200
            return Ok(allNotes);
        }

        // GET: notes/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote(int id)
        {
            bool noteExists = await _noteRepository.ContainsNote(id);
            if (!noteExists)
            {
                // 404
                return NotFound();
            }
            
            // 200
            Note note = await _noteRepository.GetNoteById(id);
            return Ok(note);
        }

        // POST: notes/
        [HttpPost]
        public async Task<IActionResult> PostNote([FromBody] Note note)
        {
            if (!ModelState.IsValid)
            {
                // 400
                return BadRequest(ModelState);
            }

            bool noteExists = await _noteRepository.ContainsNote(note.Id);
            if (noteExists)
            {
                // 409
                return Conflict(ModelState);
            }

            // 201
            Note newNote = await _noteRepository.AddNote(note);
            return CreatedAtRoute("GetNote", newNote.Id, newNote);
        }

        // PUT: notes/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(int id, [FromBody] Note note)
        {
            if (!ModelState.IsValid)
            {
                // 400
                return BadRequest(ModelState);
            }

            bool noteExists = await _noteRepository.ContainsNote(id);
            if (!noteExists)
            {
                // 201
                await _noteRepository.AddNote(note);
                return CreatedAtAction("GetNote", id, note);
            }

            // 204 
            await _noteRepository.UpdateNote(note);
            return NoContent();
        }

        // DELETE: notes/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            bool noteHasBeenRemoved = await _noteRepository.RemoveNote(id);
            if (!noteHasBeenRemoved)
            {
                // 404
                return NotFound();
            }
            
            // 200
            return Ok();
        }
    }
}
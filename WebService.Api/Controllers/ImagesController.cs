using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using WebService.Api.Data;
using WebService.Api.Models;

namespace WebService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imageDirectory = Path.Combine("C:", "Images");

        public ImageController(AppDbContext context)
        {
            _context = context;

            // Ensure the directory exists when the controller is instantiated
            if (!Directory.Exists(_imageDirectory))
            {
                Directory.CreateDirectory(_imageDirectory);
            }
        }

        // GET: api/Image/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = await _context.Images
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
            {
                return NotFound("Image record not found in the database.");
            }

            var filePath = Path.Combine(_imageDirectory, image.FilePath);

            // Ensure the file exists before attempting to read it
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found on the server.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, image.ContentType ?? "application/octet-stream");  // Use ContentType from database or default
        }

        // POST: api/Image/Upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Generate a unique file name to avoid conflicts
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_imageDirectory, uniqueFileName);

            // Save the uploaded file to the C: drive root directory
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Fetch the user from the database
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return BadRequest("Invalid user ID.");
            }

            // Create a new Image record in the database
            var image = new Image
            {
                FileName = file.FileName,
                FilePath = uniqueFileName,  // Store only the unique file name as the relative path
                ContentType = file.ContentType,
                UserId = userId,
                User = user
            };

            // Add the image to the database and save changes
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            // Return the created image metadata with a link to access it
            return CreatedAtAction(nameof(GetImage), new { id = image.Id }, image);
        }

        // DELETE: api/Image/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.Images.FindAsync(id);

            if (image == null)
            {
                return NotFound("Image record not found in the database.");
            }

            var filePath = Path.Combine(_imageDirectory, image.FilePath);

            // Check if the file exists before deleting
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);  // Delete the file from the server
            }

            // Remove the image record from the database
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

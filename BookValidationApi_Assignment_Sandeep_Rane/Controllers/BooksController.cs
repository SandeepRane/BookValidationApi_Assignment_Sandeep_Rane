using BookValidationApi_Assignment_Sandeep_Rane.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookValidationApi_Assignment_Sandeep_Rane.Controllers
{

    [ApiController]
    [Route("books")]
    public class BooksController : ControllerBase
    {
        private readonly XmlBookParser _parser;

        public BooksController(XmlBookParser parser)
        {
            _parser = parser;
        }

        [HttpGet("valid")]
        public IActionResult GetValidBooks()
        {
            try
            {
                var result = _parser.ParseBooks();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to process XML: " + ex.Message });
            }
        }
    }
}
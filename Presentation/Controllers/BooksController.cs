using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Entities.Exceptions.NotFoundException;

namespace Presentation.Controllers
{
    //[ApiVersion("1.0")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [ApiController]
    [Route("api/books")]
    [ApiExplorerSettings(GroupName = "v1")]
    //[ResponseCache(CacheProfileName ="5mins")]
    //[HttpCacheExpiration(CacheLocation =CacheLocation.Public, MaxAge =80)]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;
        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }

        [Authorize(Roles ="User,Editor,Admin")]
        [HttpHead]
        [HttpGet(Name ="GetAllBooksAsync")]
        [ServiceFilter(typeof (ValidateMediaTypeAttribute))]
        //[ResponseCache(Duration =60)]
        public async Task< IActionResult> GetAllBooksAsync([FromQuery]BookParameters bookParameters)
        {
            var linkParameters = new LinkParameters()
            {
                BookParameters = bookParameters,
                HttpContext = HttpContext
            };
                var result =await _manager.BookService.GetAllBooksAsync(linkParameters,false);
            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(result.metaData));
            return result.linkResponse.HasLinks ?
            Ok(result.linkResponse.LinkedEntities) :
            Ok(result.linkResponse.ShapedEntities);
        }

        [Authorize]
        [HttpGet("details")]
        public async Task<IActionResult> GetAllBooksWithDetailsAsync()
        {
            return Ok(await _manager.BookService.GetAllBooksWithDetailsAsync(false));
        }

        [Authorize(Roles = "User,Editor,Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBooksByIdAsync([FromRoute(Name = "id")] int id)
        {
                var book = await _manager.BookService.GetBookByIdAsync(id, false); //linq sorgusu
                
            return Ok(book);
        }

        [Authorize(Roles = "Editor,Admin,User")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost(Name ="CreateBookAsync")]
        public async Task<IActionResult> CreateBookAsync([FromBody] BookDtoForInsertion bookDto)
        {   
                var book=await _manager.BookService.CreateBookAsync(bookDto);
                return StatusCode(201, book);  
        }

        [Authorize(Roles = "Editor,Admin")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task< IActionResult> UpdateBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate bookDto)
        {
            await _manager.BookService.UpdateBookAsync(id, bookDto, false);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBookAsync([FromRoute(Name = "id")] int id)
        {
              await _manager.BookService.DeleteBookAsync(id, false);
                return NoContent();  
        }
        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET,PUT,POST, DELETE, HEAD, OPTIONS");
            return Ok();
        }

    }
}

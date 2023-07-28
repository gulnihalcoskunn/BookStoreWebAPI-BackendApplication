 using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Entities.Exceptions.BadRequestException;
using static Entities.Exceptions.NotFoundException;

namespace Services
{
    public class BookManager : IBookService
    {
        private readonly ICategoryService _categoryService;
        private readonly IRepositoryManager _manager;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly IBookLinks _bookLinks;
        public BookManager(IRepositoryManager manager, ILoggerService logger, IMapper mapper, IBookLinks bookLinks, ICategoryService categoryService)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
            _bookLinks = bookLinks;
            _categoryService = categoryService;
        }
        public async Task<BookDto> CreateBookAsync(BookDtoForInsertion bookDto)
        {
            var category = await _categoryService.GetCategoryByIdAsync(bookDto.CategoryId, false);

            var entity=_mapper.Map<Book>(bookDto);
            entity.CategoryId=bookDto.CategoryId;

            _manager.Book.CreateBook(entity);
            await _manager.SaveAsync();
            return _mapper.Map<BookDto>(entity);
        }

        public async Task DeleteBookAsync(int id, bool trackChanges)
        {
            var entity = await GetBookByIdAndCheckExists(id, trackChanges);
            _manager.Book.DeleteBook(entity);
            await _manager.SaveAsync();
        }

        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetAllBooksAsync(LinkParameters linkParameters, bool trackChanges)
        {
            if (!linkParameters.BookParameters.ValidPriceRange)
            {
                throw new PriceOutOfRangeBadRequest();
            }
            var booksWithMetaData= await _manager.Book.GetAllBooksAsync(linkParameters.BookParameters,trackChanges);
            var booksDto=_mapper.Map<IEnumerable<BookDto>>(booksWithMetaData);
            var links = _bookLinks.TryGenerateLinks(booksDto, linkParameters.BookParameters.Fields, linkParameters.HttpContext);
            return (linkRepsonse:links, metaData:booksWithMetaData.MetaData);
        }

        public async Task<List<Book>> GetAllBooksAsync(bool trackChanges)
        {
            var books = await _manager.Book.GetAllBooksAsync(trackChanges);
            return books;
        }

        public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(bool trackChanges)
        {
            return await _manager.Book.GetAllBooksWithDetailsAsync(trackChanges);
        }

        public async Task<BookDto> GetBookByIdAsync(int id, bool trackChanges)
        {
            var book = await GetBookByIdAndCheckExists(id, trackChanges);
          
            return _mapper.Map<BookDto>(book);
        }

        public async Task UpdateBookAsync(int id, BookDtoForUpdate bookDto, bool trackChanges)
        {
            var entity = await GetBookByIdAndCheckExists(id, trackChanges);

            //mapping
            //entity.Title = book.Title;
            //entity.Price = book.Price;

            entity = _mapper.Map<Book>(bookDto);

            _manager.Book.Update(entity);
            await _manager.SaveAsync();
        }
        private async Task<Book>GetBookByIdAndCheckExists(int id,bool trackChanges)
        {
            var entity = await _manager.Book.GetBooksByIdAsync(id, trackChanges);
            if (entity == null)
            {
                throw new BookNotFoundException(id);
            }
            return entity;
        }
    }


}

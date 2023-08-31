using Agorus.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using File = Agorus.Data.Models.File;

namespace Agorus.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IFileRepository _fileRepository;

        public FilesController(ILogger<FilesController> logger, IFileRepository fileRepository)
        {
            _logger = logger;
            _fileRepository = fileRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<File>>> Get(CancellationToken cancellationToken)
        {
            var files = await _fileRepository.GetAllAsync(cancellationToken);
            return files.ToList();
        }

        [HttpGet("{fileId}")]
        public async Task<ActionResult<IEnumerable<File>>> Get([FromRoute] Guid fileId, CancellationToken cancellationToken)
        {
            var files = await _fileRepository.GetAllAsync(fileId, cancellationToken);
            return files.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<File>> Get([FromRoute] int id, CancellationToken cancellationToken)
        {
            return await _fileRepository.GetAsync(id, cancellationToken) ?? (ActionResult<File>)NotFound();
        }

        [HttpGet("{fileId}")]
        public async Task<ActionResult<File>> Get([FromRoute] Guid fileId, [FromQuery] int version, CancellationToken cancellationToken)
        {
            return await _fileRepository.GetAsync(fileId, version, cancellationToken) ?? (ActionResult<File>)NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<File>> Create([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);

            var newFile = new File
            {
                Content = memoryStream.ToArray(),
            };
            var createdFile = await _fileRepository.AddAsync(newFile, cancellationToken);

            return createdFile == null
                ? StatusCode(StatusCodes.Status500InternalServerError)
                : CreatedAtAction(nameof(Get), new { id = createdFile.Id }, createdFile);
        }

        [HttpPut("{fileId}")]
        public async Task<ActionResult<File>> Update([FromRoute] Guid fileId, [FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);

            var dbFile = new File
            {
                FileId = fileId,
                Content = memoryStream.ToArray(),
            };
            var updatedFile = await _fileRepository.UpdateAsync(dbFile, cancellationToken);
            return updatedFile == null
                ? StatusCode(StatusCodes.Status500InternalServerError)
                : CreatedAtAction(nameof(Get), new { fileId = updatedFile.FileId, version = updatedFile.Version }, updatedFile);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            return await _fileRepository.DeleteAsync(id, cancellationToken);
        }

        [HttpDelete("{fileId}")]
        public async Task<ActionResult<bool>> Delete([FromRoute] Guid fileId, int version, CancellationToken cancellationToken)
        {
            return await _fileRepository.DeleteAsync(fileId, version, cancellationToken);
        }
    }
}
using Agorus.Data.Repositories;
using Agorus.Web.Api.Dtos;
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

        /// <summary>
        /// Gets all files, optionally filtered by file ID.
        /// </summary>
        /// <param name="fileId">(optional) The file ID to filter by.</param>
        /// <param name="cancellationToken" />
        /// <returns>A list of file DTOs.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileDto>>> Get([FromQuery] Guid fileId = default, CancellationToken cancellationToken = default)
        {
            var files = await _fileRepository.GetAllAsync(fileId, cancellationToken);
            return files.Select(f => (FileDto)f).ToList();
        }

        /// <summary>
        /// Gets a file by database ID.
        /// </summary>
        /// <param name="id">The database ID.</param>
        /// <param name="cancellationToken" />
        /// <returns>The requested file.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<FileDto>> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var file = await _fileRepository.GetByIdAsync(id, cancellationToken);
            return file == null ? NotFound() : File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>
        /// Gets a file by the file ID and version.
        /// </summary>
        /// <param name="fileId">The file ID.</param>
        /// <param name="version">The file version.</param>
        /// <param name="cancellationToken" />
        /// <returns>The requested file.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{fileId}/{version}")]
        public async Task<ActionResult<FileDto>> GetByFileIdAndVersion([FromRoute] Guid fileId, [FromRoute] int version, CancellationToken cancellationToken)
        {
            var file = await _fileRepository.GetByFileIdAndVersionAsync(fileId, version, cancellationToken);
            return file == null ? NotFound() : File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>
        /// Creates a new file.
        /// </summary>
        /// <param name="files">The list containing the file to create.</param>
        /// <param name="cancellationToken" />
        /// <returns>A DTO for the created file.</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult<FileDto>> Create([FromForm] IEnumerable<IFormFile> files, CancellationToken cancellationToken)
        {
            if (!files.Any() || files.Count() > 1) // only support 1 file for now
            {
                return BadRequest();
            }

            var file = files.First();
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);

            var newFile = new File
            {
                Content = memoryStream.ToArray(),
                FileName = file.FileName,
                ContentType = file.ContentType,
            };
            var createdFile = await _fileRepository.AddAsync(newFile, cancellationToken);

            return createdFile == null
                ? StatusCode(StatusCodes.Status500InternalServerError)
                : CreatedAtAction(nameof(GetById), new { id = createdFile.Id }, (FileDto)createdFile);
        }

        /// <summary>
        /// Updates a file by adding a new version.
        /// </summary>
        /// <param name="fileId">The file ID of the file to update.</param>
        /// <param name="files">The list containing the file to create.</param>
        /// <param name="cancellationToken" />
        /// <returns>A DTO for the updated file.</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{fileId}")]
        public async Task<ActionResult<FileDto>> Update([FromRoute] Guid fileId, [FromForm] IEnumerable<IFormFile> files, CancellationToken cancellationToken)
        {
            if (!files.Any() || files.Count() > 1) // only support 1 file for now
            {
                return BadRequest();
            }

            var file = files.First();
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);

            var dbFile = new File
            {
                FileId = fileId,
                Content = memoryStream.ToArray(),
                FileName = file.FileName,
                ContentType = file.ContentType,
            };
            var updatedFile = await _fileRepository.UpdateAsync(dbFile, cancellationToken);
            return updatedFile == null
                ? StatusCode(StatusCodes.Status500InternalServerError)
                : CreatedAtAction(nameof(GetById), new { fileId = updatedFile.FileId, version = updatedFile.Version }, (FileDto)updatedFile);
        }

        /// <summary>
        /// Deletes a file by database ID.
        /// </summary>
        /// <param name="id">The database ID.</param>
        /// <param name="cancellationToken" />
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            return await _fileRepository.DeleteByIdAsync(id, cancellationToken) ? Ok() : NotFound();
        }

        /// <summary>
        /// Deletes a file by file ID and version.
        /// </summary>
        /// <param name="fileId">The file ID.</param>
        /// <param name="version">The file version.</param>
        /// <param name="cancellationToken" />
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{fileId}/{version}")]
        public async Task<ActionResult> Delete([FromRoute] Guid fileId, int version, CancellationToken cancellationToken)
        {
            return await _fileRepository.DeleteByFileIdAndVersionAsync(fileId, version, cancellationToken) ? Ok() : NotFound();
        }
    }
}
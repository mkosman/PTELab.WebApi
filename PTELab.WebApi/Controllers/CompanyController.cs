using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using PTELab.Repositories.CompanyRepos;
using PTELab.Repositories.Dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PTELab.WebApi.Controllers
{
    [Route("/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepo _companyRepo;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ICompanyRepo companyRepo, ILogger<CompanyController> logger)
        {
            _companyRepo = companyRepo;
            _logger = logger;
        }

        /// <summary>
        /// For test only
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var companies = await _companyRepo.GetAllCompanies();
            return Ok(companies);
        }

        /// <summary>
        /// Create new company
        /// </summary>
        /// <param name="company">Company definitions</param>
        /// <response code="201">Company created</response>
        /// <response code="400">Error in create</response>
        /// <response code="401">Unathorized</response>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create(CompanyDto company)
        {
            if (await _companyRepo.IsCompanyNameExistsAsync(company.Name))
            {
                return BadRequest("Company name already exists");
            }

            var result = await _companyRepo.CreateAsync(company);
            if (result > 0)
            {
                return Created(string.Empty, new { Id = result });
            }

            var modelStateDic = new ModelStateDictionary();
            modelStateDic.AddModelError("create", $"{_companyRepo.InternalExeption?.Message}");
            if(_companyRepo.InternalExeption?.InnerException != null) {     
                modelStateDic.AddModelError("create innerException", $"{_companyRepo.InternalExeption?.InnerException.Message}");
            }
            return BadRequest(modelStateDic);
        }

        [HttpPost("search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(SearchRequest searchRequest)
        {
            var searchResponse = new SearchResponse();
            searchResponse.Results = await _companyRepo.Search(searchRequest);
            return Ok(searchResponse);
        }

        /// <summary>
        /// Update company information
        /// </summary>
        /// <param name="id">Company Id</param>
        /// <param name="companyItem">Full company information</param>
        /// <response code="200">Company update successfully</response>
        /// <response code="204">No content</response>
        /// <response code="401">Unathorized</response>
        /// <response code="404">Company not found</response>
        /// <returns></returns>
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([Required] long id, CompanyDto companyItem)
        {
            if (id != companyItem.Id)
            {
                return BadRequest("Wrong company id");
            }

            var company = await _companyRepo.FindCompanyByNameAsync(companyItem.Name);
            if (company != null)
            {
                _logger.LogWarning($"Company with name '{companyItem.Name}' exist with Id={company.Id}");
                return BadRequest($"Company with name '{companyItem.Name}' exist with Id={company.Id}");
            }

            await _companyRepo.Update(companyItem);
            return NoContent();
        }

        /// <summary>
        /// Delete company with all assigned employees by companyId
        /// </summary>
        /// <remarks>
        /// HttpDelete /Company/{id}
        /// </remarks>
        /// <param name="id">Compnay Id</param>
        /// <response code="200">Company deleted successfully</response>
        /// <response code="401">Unathorized</response>
        /// <response code="404">Company not found</response>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([Required] long id)
        {
            if (await _companyRepo.IsCompanyIdExistsAsync(id))
            {
                var companyItem = await _companyRepo.FindCompanyByIdAsync(id);
                await _companyRepo.Delete(companyItem);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using PTELab.Database.Entities;
using PTELab.Repositories.Dto;

namespace PTELab.Repositories.CompanyRepos
{
    public interface ICompanyRepo
    {
        Exception InternalExeption { get; set; }
        Task<bool> IsCompanyIdExistsAsync(long id, CancellationToken cancellationToken = default);
        Task<bool> IsCompanyNameExistsAsync(string companyName, CancellationToken cancellationToken = default);
        Task<CompanyDto> FindCompanyByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<bool> Delete(CompanyDto companyItem, CancellationToken cancellationToken = default);
        Task<long> CreateAsync(CompanyDto companyItem, CancellationToken cancellationToken = default);
        Task<CompanyDto> FindCompanyByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> Update(CompanyDto companyItem, CancellationToken cancellationToken = default);
        Task<List<CompanyDto>> GetAllCompanies(CancellationToken cancellationToken = default);
        Task<List<CompanyDto>> Search(SearchRequest searchRequest, CancellationToken cancellationToken = default);
    }
    public class CompanyRepo : ICompanyRepo
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyRepo> _logger;

        public CompanyRepo(ISession session, IMapper mapper, ILogger<CompanyRepo> logger)
        {
            _session = session;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> IsCompanyIdExistsAsync(long id, CancellationToken cancellationToken = default)
        {
            var company = await _session.GetAsync<Company>(id, cancellationToken);
            return company != null;
        }

        public async Task<bool> IsCompanyNameExistsAsync(string companyName, CancellationToken cancellationToken = default)
        {
            var company = await _session.Query<Company>().FirstOrDefaultAsync(x => x.Name.Equals(companyName), cancellationToken: cancellationToken);
            return company != null;
        }

        public async Task<CompanyDto> FindCompanyByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var company = await _session.Query<Company>().FirstOrDefaultAsync(x => x.Name.Equals(name), cancellationToken: cancellationToken);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }

        public async Task<bool> Update(CompanyDto companyItem, CancellationToken cancellationToken = default)
        {
            using var transaction = _session.BeginTransaction();
            try
            {
                var company = await _session.GetAsync<Company>(companyItem.Id, cancellationToken);
                _mapper.Map<CompanyDto, Company>(companyItem, company);
                await _session.UpdateAsync(company, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (Exception e)
            {
                InternalExeption = e;
                _logger.LogError(e, "Update company item");
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }
        }

        public async Task<List<CompanyDto>> GetAllCompanies(CancellationToken cancellationToken = default)
        {
            var l = await _session.Query<Company>().ToListAsync(cancellationToken);
            var q = _mapper.Map<List<CompanyDto>>(l);
            return q;
        }

        public async Task<List<CompanyDto>> Search(SearchRequest searchRequest, CancellationToken cancellationToken = default)
        {
            var list = _session.Query<Company>();

            var keyword = searchRequest.Keyword?.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(x => x.Name.Like(keyword) || x.Employees.Any(employee => employee.FirstName.Like(keyword) || employee.LastName.Like(keyword)));
            }

            if (searchRequest.EmployeeDateOfBirthFrom.HasValue)
            {
                list = list.Where(x => x.Employees.Any(employee => employee.DateOfBirth >= searchRequest.EmployeeDateOfBirthFrom.Value));
            }
            if (searchRequest.EmployeeDateOfBirthTo.HasValue)
            {
                list = list.Where(x => x.Employees.Any(employee => employee.DateOfBirth <= searchRequest.EmployeeDateOfBirthTo.Value));
            }

            if (searchRequest.EmployeeJobTitles != null && searchRequest.EmployeeJobTitles.Any())
            {
                var listJobTitles = new List<string>(searchRequest.EmployeeJobTitles);
                list = list.Where(x => x.Employees.Any(employee => listJobTitles.Contains(employee.JobTitle)));
            }

            var result = _mapper.Map<List<CompanyDto>>(await list.ToListAsync(cancellationToken));
            return result;
        }

        public async Task<CompanyDto> FindCompanyByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var company = await _session.GetAsync<Company>(id, cancellationToken);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }

        public async Task<long> CreateAsync(CompanyDto companyItem, CancellationToken cancellationToken = default)
        {
            var company = _mapper.Map<Company>(companyItem);
            company.Id = 0;
            using var transaction = _session.BeginTransaction();
            try
            {
                var id = (long)await _session.SaveAsync(company, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return id;
            }
            catch (Exception e)
            {
                InternalExeption = e;
                _logger.LogError(e, "Create new company item");
                await transaction.RollbackAsync(cancellationToken);
                return -1;
            }
        }

        public async Task<bool> Delete(CompanyDto companyItem, CancellationToken cancellationToken = default)
        {
            using var transaction = _session.BeginTransaction();
            try
            {
                var company = await _session.GetAsync<Company>(companyItem.Id, cancellationToken);
                await _session.DeleteAsync(company, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (Exception e)
            {
                InternalExeption = e;
                _logger.LogError(e, "Delete company item");
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }
        }


        public Exception InternalExeption { get; set; }
    }

}

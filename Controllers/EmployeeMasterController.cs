using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practice.Data;
using practice.DTOs;
using practice.Models;

namespace practice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeMasterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeMasterController(AppDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.EmployeeMasters
                .Select(e => new
                {
                    e.EmpId,
                    e.EmployeeCode,
                    e.FullName,
                    e.Department,
                    e.DateOfJoining
                })
                .ToListAsync();

            return Ok(employees);
        }



        [HttpPost("register")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto dto)
        {
           
            var emailExists = await _context.EmployeeMasters
                .AnyAsync(e => e.EmailId == dto.EmailId);

            if (emailExists)
                return BadRequest("Email already exists");

            
            var codeExists = await _context.EmployeeMasters
                .AnyAsync(e => e.EmployeeCode == dto.EmployeeCode);

            if (codeExists)
                return BadRequest("Employee Code already exists");

            var employee = new EmployeeMaster
            {
                EmployeeCode = dto.EmployeeCode,
                FullName = dto.FullName,
                EmailId = dto.EmailId,
                Phone = dto.Phone,
                EmergencyContact = dto.EmergencyContact,
                Department = dto.Department,
                Designation = dto.Designation,
                DateOfJoining = dto.DateOfJoining,
                EmploymentType = dto.EmploymentType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.EmployeeMasters.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Employee registered successfully",
                employee.EmpId
            });
        }

        
        [HttpPost("{id}/address")]
        public async Task<IActionResult> SaveAddress(int id, [FromBody] EmployeeAddressDto dto)
        {
            var employee = await _context.EmployeeMasters
                .FirstOrDefaultAsync(e => e.EmpId == id);

            if (employee == null)
                return NotFound("Employee not found");

            var address = await _context.EmployeeAddresses
                .FirstOrDefaultAsync(a => a.EmpId == id);

            if (address == null)
            {
                address = new EmployeeAddress
                {
                    EmpId = id
                };

                _context.EmployeeAddresses.Add(address);
            }

            
            address.CurrentDoorNo = dto.CurrentDoorNo;
            address.CurrentStreet = dto.CurrentStreet;
            address.CurrentArea = dto.CurrentArea;
            address.CurrentCity = dto.CurrentCity;
            address.CurrentState = dto.CurrentState;
            address.CurrentPincode = dto.CurrentPincode;
            address.CurrentCountry = dto.CurrentCountry;

            address.PermanentDoorNo = dto.PermanentDoorNo;
            address.PermanentStreet = dto.PermanentStreet;
            address.PermanentArea = dto.PermanentArea;
            address.PermanentCity = dto.PermanentCity;
            address.PermanentState = dto.PermanentState;
            address.PermanentPincode = dto.PermanentPincode;
            address.PermanentCountry = dto.PermanentCountry;

            await _context.SaveChangesAsync();

            return Ok("Address saved successfully");
        }


        [HttpPost("{id}/personal-details")]
        public async Task<IActionResult> SavePersonalDetails(int id, [FromBody] EmployeePersonalDetailsDto dto)
        {
            var employee = await _context.EmployeeMasters
                .FirstOrDefaultAsync(e => e.EmpId == id);

            if (employee == null)
                return NotFound("Employee not found");

            var personal = await _context.EmployeePersonalDetails
                .FirstOrDefaultAsync(p => p.EmpId == id);

            if (personal == null)
            {
                personal = new EmployeePersonalDetails
                {
                    EmpId = id
                };

                _context.EmployeePersonalDetails.Add(personal);
            }

            personal.FirstName = dto.FirstName;
            personal.LastName = dto.LastName;
            personal.FullName = dto.FullName;
            personal.Religion = dto.Religion;
            personal.Mobile = dto.Mobile;
            personal.AlternateContact = dto.AlternateContact;
            if (dto.DateOfBirth.HasValue)
            {
                personal.DateOfBirth = DateTime.SpecifyKind(dto.DateOfBirth.Value, DateTimeKind.Utc);
            }
            personal.MaritalStatus = dto.MaritalStatus;
            personal.BloodGroup = dto.BloodGroup;
            personal.Nationality = dto.Nationality;
            personal.Gender = dto.Gender;

            await _context.SaveChangesAsync();

            return Ok("Personal details saved successfully");
        }



        [HttpPost("{id}/education/bulk")]
        public async Task<IActionResult> AddMultipleEducation(
    int id,
    [FromBody] List<EmployeeEducationDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest("Education list is empty");

            var employeeExists = await _context.EmployeeMasters
                .AnyAsync(e => e.EmpId == id);

            if (!employeeExists)
                return NotFound("Employee not found");

            var newEducations = new List<EmployeeEducation>();

            foreach (var dto in dtos)
            {
                
                var exists = await _context.EmployeeEducations.AnyAsync(e =>
                    e.EmpId == id &&
                    e.Qualification == dto.Qualification &&
                    e.DegreeName == dto.DegreeName &&
                    e.University == dto.University &&
                    e.YearOfPassing == dto.YearOfPassing);

                if (!exists)
                {
                    newEducations.Add(new EmployeeEducation
                    {
                        EmpId = id,
                        Qualification = dto.Qualification,
                        DegreeName = dto.DegreeName,
                        University = dto.University,
                        YearOfPassing = dto.YearOfPassing,
                        Percentage = dto.Percentage,
                        Certifications = dto.Certifications
                    });
                }
            }

            if (!newEducations.Any())
                return BadRequest("All education records already exist");

            _context.EmployeeEducations.AddRange(newEducations);
            await _context.SaveChangesAsync();

            return Ok($"{newEducations.Count} education record(s) added successfully");
        }




        [HttpPost("{id}/compensation")]
        public async Task<IActionResult> SaveCompensation(
    int id,
    [FromBody] EmployeeCompensationDto dto)
        {
            var employee = await _context.EmployeeMasters
                .FirstOrDefaultAsync(e => e.EmpId == id);

            if (employee == null)
                return NotFound("Employee not found");

            var compensation = await _context.EmployeeCompensations
                .FirstOrDefaultAsync(c => c.EmpId == id);

            if (compensation == null)
            {
                compensation = new EmployeeCompensation
                {
                    EmpId = id
                };

                _context.EmployeeCompensations.Add(compensation);
            }

           
            compensation.AccountHolderName = dto.AccountHolderName;
            compensation.BankName = dto.BankName;
            compensation.BranchName = dto.BranchName;
            compensation.AccountNumber = dto.AccountNumber;
            compensation.IFSCCode = dto.IFSCCode;
            compensation.AccountType = dto.AccountType;
            compensation.TaxInfo = dto.TaxInfo;
            compensation.Benefits = dto.Benefits;

            await _context.SaveChangesAsync();

            return Ok("Compensation details saved successfully");
        }



        [HttpPost("{id}/previous-employment")]
        public async Task<IActionResult> SavePreviousEmployment(
    int id,
    [FromBody] List<EmployeePreviousEmploymentDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest("No employment data provided");

            var employeeExists = await _context.EmployeeMasters
                .AnyAsync(e => e.EmpId == id);

            if (!employeeExists)
                return NotFound("Employee not found");

            var employments = dtos.Select(dto => new EmployeePreviousEmployment
            {
                EmpId = id,
                CompanyName = dto.CompanyName,
                Designation = dto.Designation,
                Experience = dto.Experience,
                DateOfJoining = dto.DateOfJoining,
                DateOfRelieving = dto.DateOfRelieving,
                ReasonForLeaving = dto.ReasonForLeaving,
                LastDrawnSalary = dto.LastDrawnSalary
            }).ToList();

            _context.EmployeePreviousEmployments.AddRange(employments);
            await _context.SaveChangesAsync();

            return Ok($"{employments.Count} employment record(s) saved successfully");
        }



    }
}
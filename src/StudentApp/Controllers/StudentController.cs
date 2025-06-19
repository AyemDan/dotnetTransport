using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Transport.Shared.DTOs;
using Transport.Shared.Entities;
using Transport.Shared.Entities.MongoDB;
using Transport.Shared.Enums;

namespace StudentApp.Controllers;

[ApiController]
[Route("api/students")]
public class StudentController : ControllerBase
{
    private readonly IMongoCollection<Student> _students;
    private readonly IMongoCollection<Booking> _bookings;
    private readonly IMongoCollection<RFIDCard> _rfidCards;
    private readonly IMongoCollection<UserPreferences> _preferences;
    private readonly ILogger<StudentController> _logger;
    private readonly HttpClient _httpClient;

    public StudentController(
        IMongoDatabase database,
        ILogger<StudentController> logger,
        HttpClient httpClient
    )
    {
        _students = database.GetCollection<Student>("Students");
        _bookings = database.GetCollection<Booking>("Bookings");
        _rfidCards = database.GetCollection<RFIDCard>("RFIDCards");
        _preferences = database.GetCollection<UserPreferences>("UserPreferences");
        _logger = logger;
        _httpClient = httpClient;
    }

    // Student Management
    [HttpPost("register")]
    public async Task<IActionResult> RegisterStudent(
        [FromBody] StudentRegistrationDto registrationDto
    )
    {
        try
        {
            var existingStudent = await _students
                .Find(s => s.Email == registrationDto.Email)
                .FirstOrDefaultAsync();
            if (existingStudent != null)
            {
                return BadRequest("Student with this email already exists");
            }

            var student = new Student(registrationDto.Email, registrationDto.Password)
            {
                Name = registrationDto.Name,
                Grade = registrationDto.Grade,
                ParentName = registrationDto.ParentName,
                ParentPhone = registrationDto.ParentPhone,
                EmergencyContact = registrationDto.EmergencyContact,
                EmergencyPhone = registrationDto.EmergencyPhone,
                School = registrationDto.School,
                Address = registrationDto.Address,
                DateOfBirth = registrationDto.DateOfBirth,
                Gender = registrationDto.Gender,
                BloodGroup = registrationDto.BloodGroup,
                Allergies = registrationDto.Allergies,
                MedicalConditions = registrationDto.MedicalConditions,
                SpecialNeeds = registrationDto.SpecialNeeds,
                Notes = registrationDto.Notes,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            await _students.InsertOneAsync(student);

            // Create user preferences
            var preferences = new UserPreferences
            {
                Id = ObjectId.GenerateNewId(),
                UserId = student.Id,
                CreatedAt = DateTime.UtcNow,
            };
            await _preferences.InsertOneAsync(preferences);

            // Notify auth service
            await _httpClient.PostAsJsonAsync(
                "http://localhost:5128/api/auth/register",
                new
                {
                    UserId = student.Id,
                    Email = student.Email,
                    Password = registrationDto.Password,
                    Role = student.Role.ToString(),
                }
            );

            return Ok(new { Message = "Student registered successfully", StudentId = student.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering student");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{studentId}")]
    public async Task<IActionResult> GetStudent(string studentId)
    {
        try
        {
            if (!Guid.TryParse(studentId, out var studentGuid))
            {
                return BadRequest("Invalid student ID format");
            }

            var student = await _students.Find(s => s.Id == studentGuid).FirstOrDefaultAsync();
            if (student == null)
            {
                return NotFound("Student not found");
            }

            return Ok(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{studentId}")]
    public async Task<IActionResult> UpdateStudent(
        string studentId,
        [FromBody] StudentUpdateDto updateDto
    )
    {
        try
        {
            if (!Guid.TryParse(studentId, out var studentGuid))
            {
                return BadRequest("Invalid student ID format");
            }

            var update = Builders<Student>
                .Update.Set(s => s.Name, updateDto.Name)
                .Set(s => s.Grade, updateDto.Grade)
                .Set(s => s.ParentPhone, updateDto.ParentPhone)
                .Set(s => s.UpdatedAt, DateTime.UtcNow);

            var result = await _students.UpdateOneAsync(s => s.Id == studentGuid, update);
            if (result.MatchedCount == 0)
            {
                return NotFound("Student not found");
            }

            return Ok("Student updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student");
            return StatusCode(500, "Internal server error");
        }
    }

    // Booking Management
    [HttpPost("bookings")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingDto bookingDto)
    {
        try
        {
            // Verify student exists
            var student = await _students
                .Find(s => s.Id == bookingDto.UserId)
                .FirstOrDefaultAsync();
            if (student == null)
            {
                return BadRequest("Student not found");
            }

            var booking = new Booking
            {
                Id = ObjectId.GenerateNewId(),
                UserId = bookingDto.UserId,
                TripId = bookingDto.TripId,
                BoardingStop = bookingDto.BoardingStop,
                DropOffStop = bookingDto.DropOffStop,
                SegmentPrice = bookingDto.SegmentPrice,
                BookingDate = bookingDto.BookingDate,
                Status = bookingDto.Status,
                CreatedAt = DateTime.UtcNow,
            };

            await _bookings.InsertOneAsync(booking);

            // Notify notification service
            await _httpClient.PostAsJsonAsync(
                "http://localhost:5264/api/notifications/send",
                new
                {
                    UserId = student.Id,
                    Message = $"Your booking for {bookingDto.BookingDate:MMM dd, yyyy} has been created",
                    Type = "Booking",
                }
            );

            return Ok(new { Message = "Booking created successfully", BookingId = booking.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("bookings/{studentId}")]
    public async Task<IActionResult> GetStudentBookings(
        string studentId,
        [FromQuery] string? status = null
    )
    {
        try
        {
            if (!Guid.TryParse(studentId, out var studentGuid))
            {
                return BadRequest("Invalid student ID format");
            }

            var filter = Builders<Booking>.Filter.Eq(b => b.UserId, studentGuid);
            if (!string.IsNullOrEmpty(status))
            {
                filter &= Builders<Booking>.Filter.Eq(b => b.Status, status);
            }

            var bookings = await _bookings.Find(filter).ToListAsync();
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("bookings/{bookingId}")]
    public async Task<IActionResult> CancelBooking(string bookingId)
    {
        try
        {
            if (!ObjectId.TryParse(bookingId, out var bookingObjectId))
            {
                return BadRequest("Invalid booking ID format");
            }

            var update = Builders<Booking>.Update.Set(b => b.Status, "Cancelled");

            var result = await _bookings.UpdateOneAsync(b => b.Id == bookingObjectId, update);
            if (result.MatchedCount == 0)
            {
                return NotFound("Booking not found");
            }

            return Ok("Booking cancelled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking");
            return StatusCode(500, "Internal server error");
        }
    }

    // RFID Card Management
    [HttpPost("rfid-cards")]
    public async Task<IActionResult> AssignRFIDCard([FromBody] RFIDCardDto cardDto)
    {
        try
        {
            // Verify student exists
            var student = await _students.Find(s => s.Id == cardDto.UserId).FirstOrDefaultAsync();
            if (student == null)
            {
                return BadRequest("Student not found");
            }

            var rfidCard = new RFIDCard
            {
                Id = ObjectId.GenerateNewId(),
                UserId = cardDto.UserId,
                CardNumber = cardDto.CardNumber,
                Status = "Active",
                IssueDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };

            await _rfidCards.InsertOneAsync(rfidCard);

            return Ok(new { Message = "RFID card assigned successfully", CardId = rfidCard.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning RFID card");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("rfid-cards/{studentId}")]
    public async Task<IActionResult> GetStudentRFIDCard(string studentId)
    {
        try
        {
            if (!Guid.TryParse(studentId, out var studentGuid))
            {
                return BadRequest("Invalid student ID format");
            }

            var rfidCard = await _rfidCards
                .Find(c => c.UserId == studentGuid && c.Status == "Active")
                .FirstOrDefaultAsync();
            if (rfidCard == null)
            {
                return NotFound("No active RFID card found for this student");
            }

            return Ok(rfidCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving RFID card");
            return StatusCode(500, "Internal server error");
        }
    }

    // User Preferences
    [HttpGet("preferences/{studentId}")]
    public async Task<IActionResult> GetUserPreferences(string studentId)
    {
        try
        {
            if (!Guid.TryParse(studentId, out var studentGuid))
            {
                return BadRequest("Invalid student ID format");
            }

            var preferences = await _preferences
                .Find(p => p.UserId == studentGuid)
                .FirstOrDefaultAsync();
            if (preferences == null)
            {
                return NotFound("User preferences not found");
            }

            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user preferences");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("preferences/{studentId}")]
    public async Task<IActionResult> UpdateUserPreferences(
        string studentId,
        [FromBody] UserPreferences preferences
    )
    {
        try
        {
            if (!Guid.TryParse(studentId, out var studentGuid))
            {
                return BadRequest("Invalid student ID format");
            }

            var update = Builders<UserPreferences>.Update.Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _preferences.UpdateOneAsync(p => p.UserId == studentGuid, update);
            if (result.MatchedCount == 0)
            {
                return NotFound("User preferences not found");
            }

            return Ok("User preferences updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user preferences");
            return StatusCode(500, "Internal server error");
        }
    }
}

// DTOs for Student Application
public class StudentRegistrationDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string? ParentName { get; set; }
    public string? ParentPhone { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string? School { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string? Allergies { get; set; }
    public string? MedicalConditions { get; set; }
    public string? SpecialNeeds { get; set; }
    public string? Notes { get; set; }
}

public class StudentUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string? ParentPhone { get; set; }
}

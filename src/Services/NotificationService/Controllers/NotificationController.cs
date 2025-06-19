using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Transport.Shared.Entities;
using Transport.Shared.Entities.MongoDB;
using Transport.Shared.DTOs;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly IMongoCollection<Notification> _notifications;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(IMongoDatabase database, ILogger<NotificationController> logger)
    {
        _notifications = database.GetCollection<Notification>("notifications");
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationDto notificationDto)
    {
        try
        {
            var notification = new Notification
            {
                UserId = notificationDto.UserId,
                Type = notificationDto.Type,
                Message = notificationDto.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notifications.InsertOneAsync(notification);

            // In a real implementation, you would send the actual notification here
            // For now, we'll just log it
            _logger.LogInformation($"Notification sent to user {notificationDto.UserId}: {notificationDto.Message}");

            return Ok(new { Message = "Notification sent successfully", NotificationId = notification.Id.ToString() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserNotifications(string userId, [FromQuery] bool? unreadOnly = false)
    {
        try
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.UserId, Guid.Parse(userId));
            if (unreadOnly == true)
            {
                filter &= Builders<Notification>.Filter.Eq(n => n.IsRead, false);
            }

            var notifications = await _notifications.Find(filter)
                .SortByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(string notificationId)
    {
        try
        {
            var update = Builders<Notification>.Update
                .Set(n => n.IsRead, true);

            var result = await _notifications.UpdateOneAsync(n => n.Id == ObjectId.Parse(notificationId), update);
            if (result.MatchedCount == 0)
            {
                return NotFound("Notification not found");
            }

            return Ok("Notification marked as read");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(string notificationId)
    {
        try
        {
            var result = await _notifications.DeleteOneAsync(n => n.Id == ObjectId.Parse(notificationId));
            if (result.DeletedCount == 0)
            {
                return NotFound("Notification not found");
            }

            return Ok("Notification deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification");
            return StatusCode(500, "Internal server error");
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Transport.Application.Services;
using Transport.Domain.Entities.MongoDB;
using Transport.API.DTOs;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/rfidcard")]
    public class RFIDCardController : ControllerBase
    {
        private readonly RFIDCardService _cardService;
        public RFIDCardController(RFIDCardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
            var cards = await _cardService.GetAllCardsAsync();
            return Ok(cards);
        }

        [HttpGet("{cardNumber}")]
        public async Task<IActionResult> GetCardByNumber(string cardNumber)
        {
            var card = await _cardService.GetCardByNumberAsync(cardNumber);
            if (card == null) return NotFound();
            return Ok(card);
        }

        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] RFIDCardDto dto)
        {
            var card = new RFIDCard
            {
                CardNumber = dto.CardNumber,
                UserId = dto.UserId,
                Balance = dto.Balance,
                Status = dto.Status,
                CreatedAt = System.DateTime.UtcNow
            };
            await _cardService.AddCardAsync(card);
            return Ok();
        }

        [HttpPost("topup")]
        public async Task<IActionResult> TopUp([FromBody] TopUpDto dto)
        {
            await _cardService.TopUpAsync(dto.CardNumber, dto.Amount);
            return Ok();
        }

        [HttpPost("tapin")]
        public async Task<IActionResult> TapIn([FromBody] TapInDto dto)
        {
            var result = await _cardService.TapInAsync(dto.CardNumber, dto.Fare, dto.TripId, dto.StudentId, dto.Stop);
            return Ok(new { success = result });
        }

        [HttpPost("tapout")]
        public async Task<IActionResult> TapOut([FromBody] TapOutDto dto)
        {
            var (success, message) = await _cardService.TapOutAsync(dto.TripId, dto.StudentId, dto.Stop);
            return Ok(new { success, message });
        }

        [HttpPost("vet-tapout")]
        public async Task<IActionResult> VetTapOut([FromBody] VetTapOutDto dto)
        {
            await _cardService.VetTapOutAsync(dto.AttendanceId, dto.Approve, dto.Reason);
            return Ok();
        }

        [HttpGet("balance/{cardNumber}")]
        public async Task<IActionResult> GetBalance(string cardNumber)
        {
            var balance = await _cardService.GetBalanceAsync(cardNumber);
            if (balance == null) return NotFound();
            return Ok(balance);
        }
    }
} 
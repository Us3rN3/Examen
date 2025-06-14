using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.DTO.Reservation;
using VivesRental.Services.Interfaces;

namespace VivesRental.Controllers.Api
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationApiController : ControllerBase
    {
        private readonly IService<ArticleReservation> _reservationService;
        private readonly IMapper _mapper;

        public ReservationApiController(IService<ArticleReservation> reservationService, IMapper mapper)
        {
            _reservationService = reservationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleReservationDto>>> GetAll()
        {
            var reservations = await _reservationService.GetAllAsync();
            if (reservations == null)
                return NotFound();

            var dtos = _mapper.Map<IEnumerable<ArticleReservationDto>>(reservations);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleReservationDto>> GetById(Guid id)
        {
            var reservation = await _reservationService.FindByIdAsync(id);
            if (reservation == null)
                return NotFound();

            var dto = _mapper.Map<ArticleReservationDto>(reservation);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ArticleReservationCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservations = await _reservationService.GetAllAsync();
            if (reservations == null)
                return StatusCode(500, "Fout bij ophalen van reservaties.");

            bool overlaps = reservations.Any(r =>
                r.ArticleId == createDto.ArticleId &&
                r.UntilDateTime > createDto.FromDateTime &&
                r.FromDateTime < createDto.UntilDateTime
            );

            if (overlaps)
                return BadRequest("Dit artikel is al gereserveerd binnen deze periode.");

            var reservation = _mapper.Map<ArticleReservation>(createDto);
            reservation.Id = Guid.NewGuid();

            await _reservationService.AddAsync(reservation);

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, null);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] ArticleReservationUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingReservation = await _reservationService.FindByIdAsync(id);
            if (existingReservation == null)
                return NotFound();

            var reservations = await _reservationService.GetAllAsync();
            if (reservations == null)
                return StatusCode(500, "Fout bij ophalen van reservaties.");

            // Enkel overlappingen op hetzelfde artikel
            bool overlaps = reservations.Any(r =>
                r.Id != id &&
                r.ArticleId == existingReservation.ArticleId &&
                r.UntilDateTime > updateDto.FromDateTime &&
                r.FromDateTime < updateDto.UntilDateTime
            );

            if (overlaps)
                return BadRequest("Dit artikel is al gereserveerd binnen deze periode.");

            _mapper.Map(updateDto, existingReservation);

            await _reservationService.UpdateAsync(existingReservation);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var existingReservation = await _reservationService.FindByIdAsync(id);
            if (existingReservation == null)
                return NotFound();

            await _reservationService.DeleteAsync(existingReservation);

            return NoContent();
        }

        // 🔍 Filter op klant en/of datum
        [HttpGet("search")]
        public async Task<IActionResult> SearchReservations([FromQuery] Guid? customerId, [FromQuery] DateTime? from, [FromQuery] DateTime? until)
        {
            var reservations = await _reservationService.GetAllAsync();
            if (reservations == null)
                return NotFound();

            var filtered = reservations
                .Where(r =>
                    (!customerId.HasValue || r.CustomerId == customerId) &&
                    (!from.HasValue || r.UntilDateTime >= from) &&
                    (!until.HasValue || r.FromDateTime <= until)
                );

            var dtos = _mapper.Map<IEnumerable<ArticleReservationDto>>(filtered);
            return Ok(dtos);
        }
    }
}

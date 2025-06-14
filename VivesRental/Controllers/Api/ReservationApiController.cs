using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.DTO.Reservation;
using VivesRental.Services.Interfaces;

namespace VivesRental.Controllers.Api
{
    [Route("api/reservations")]
    [ApiController]
    [Authorize]
    public class ReservationApiController : ControllerBase
    {
        private readonly IService<ArticleReservation> _reservationService;
        private readonly IMapper _mapper;

        public ReservationApiController(IService<ArticleReservation> reservationService, IMapper mapper)
        {
            _reservationService = reservationService;
            _mapper = mapper;
        }

        // GET all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleReservationDto>>> GetAll(
            [FromQuery] Guid? customerId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? until = null)
        {
            var reservations = await _reservationService.GetAllAsync();
            if (reservations == null)
                return NotFound();

            if (customerId.HasValue)
                reservations = reservations.Where(r => r.CustomerId == customerId.Value);

            if (from.HasValue)
                reservations = reservations.Where(r => r.FromDateTime >= from.Value);

            if (until.HasValue)
                reservations = reservations.Where(r => r.UntilDateTime <= until.Value);

            var dtos = _mapper.Map<IEnumerable<ArticleReservationDto>>(reservations);
            return Ok(dtos);
        }

        // GET by id
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleReservationDto>> GetById(Guid id)
        {
            var reservation = await _reservationService.FindByIdAsync(id);
            if (reservation == null)
                return NotFound();

            var dto = _mapper.Map<ArticleReservationDto>(reservation);
            return Ok(dto);
        }

        // POST
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ArticleReservationCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { Errors = errors });
            }

            try
            {
                var reservations = await _reservationService.GetAllAsync();

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
            catch (Exception)
            {
                // Log exception
                return StatusCode(500, "Er is een onverwachte fout opgetreden bij het aanmaken van de reservatie.");
            }
        }


        // PUT
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] ArticleReservationUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { Errors = errors });
            }

            try
            {
                var existingReservation = await _reservationService.FindByIdAsync(id);
                if (existingReservation == null)
                    return NotFound($"Reservatie met id {id} niet gevonden.");

                var reservations = await _reservationService.GetAllAsync();
                bool overlaps = reservations.Any(r =>
                    r.Id != id &&
                    r.ArticleId == updateDto.ArticleId &&
                    r.UntilDateTime > updateDto.FromDateTime &&
                    r.FromDateTime < updateDto.UntilDateTime
                );

                if (overlaps)
                    return BadRequest("Dit artikel is al gereserveerd binnen deze periode.");

                _mapper.Map(updateDto, existingReservation);
                await _reservationService.UpdateAsync(existingReservation);

                return NoContent();
            }
            catch (Exception)
            {
                // Log de exception eventueel hier of via een logging service
                return StatusCode(500, "Er is een onverwachte fout opgetreden bij het updaten van de reservatie.");
            }
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
    }
}

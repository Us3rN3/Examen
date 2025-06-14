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
    }
}

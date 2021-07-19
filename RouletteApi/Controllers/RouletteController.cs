using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Roulette.BI.DTORequest.Roulette;
using Roulette.BI.DTOResponse.Roulette;
using Roulette.BI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouletteApi.Controllers
{
    [Route("[controller]")]
    public class RouletteController : ControllerBase
    {
        private readonly RouletteService _rouletteService;
        private readonly ILogger<RouletteController> _logger;

        public RouletteController(RouletteService rouletteService, ILogger<RouletteController> logger)
        {
            _rouletteService = rouletteService;
            _logger = logger;
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> AddRoulette()
        {
            try
            {
                AddRouletteResponseDTO responseAddRoulette = await _rouletteService.AddRoulette();

                return Ok(responseAddRoulette);
            }
            catch (Exception err)
            {
                _logger.LogError(err.ToString());
                var errorResponse = new ErrorResponseDTO
                {
                    Message = "Ha ocurrido un error al crear la ruleta"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }


        [HttpGet("[Action]/{rouletteId}")]
        public async Task<IActionResult> OpenRoulette(Guid rouletteId)
        {
            try
            {
                OpenRouletteResponseDTO responseAddRoulette = await _rouletteService.OpenRoulette(rouletteID: rouletteId);

                return Ok(responseAddRoulette);
            }
            catch (Exception err)
            {
                _logger.LogError(err.ToString());
                var errorResponse = new ErrorResponseDTO
                {
                    Message = "Ha ocurrido un error al abrir la ruleta"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }


        [HttpPost("[Action]")]
        public async Task<IActionResult> AddRouletteBet([FromBody] AddRouletteBetRequestDTO addRouletteBetRequestDTO, [FromHeader(Name = "userId")] Guid userId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AddRouletteBetResponseDTO responseAddRoulette = await _rouletteService.AddRouletteBet(addRouletteBetRequestDTO: addRouletteBetRequestDTO, userId: userId);

                    return Ok(responseAddRoulette);
                }
                else
                {
                    var listadoErrores = ModelState
                                        .Where(y => y.Value.ValidationState == ModelValidationState.Invalid)
                                        .Select(y => new ErrorFieldsResponseDTO
                                        {
                                            Field = new string(y.Key?.ToArray()),
                                            Error = y.Value?.Errors?.FirstOrDefault()?.ErrorMessage
                                        })
                                        .ToList();

                    return StatusCode(StatusCodes.Status400BadRequest, listadoErrores);
                }
            }
            catch (Exception err)
            {
                _logger.LogError(err.ToString());
                var errorResponse = new ErrorResponseDTO
                {
                    Message = "Ha ocurrido un error al agregar apuestas a la ruleta"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpGet("[Action]/{rouletteId}")]
        public async Task<IActionResult> CloseRoulette(Guid rouletteId)
        {
            try
            {
                List<CloseRouletteResponseDTO> responseAddRoulette = await _rouletteService.CloseRoulette(rouletteID: rouletteId);

                return Ok(responseAddRoulette);
            }
            catch (Exception err)
            {
                _logger.LogError(err.ToString());
                var errorResponse = new ErrorResponseDTO
                {
                    Message = "Ha ocurrido un error al cerrar la ruleta"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> RoulettesList()
        {
            try
            {
                List<RouletteResponseDTO> responseAddRoulette = await _rouletteService.RoulettesList();

                return Ok(responseAddRoulette);
            }
            catch (Exception err)
            {
                _logger.LogError(err.ToString());
                var errorResponse = new ErrorResponseDTO
                {
                    Message = "Ha ocurrido un error al cargar el listado de las ruletas"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}

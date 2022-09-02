using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts;
using PSD.FeatureToggle.Entities;
using PSD.FeatureToggle.Extensions;
using PSD.FeatureToggle.Features;
using PSD.FeatureToggle.Features.CustomFilter;

namespace PSD.FeatureToggle.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "ChapterDayScheme")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService) => 
            _cardService = cardService;

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _cardService.GetAllAsync());
        }
    }
}

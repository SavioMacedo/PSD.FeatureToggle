using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts;
using PSD.FeatureToggle.Features;

namespace PSD.FeatureToggle.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AlunoController : ControllerBase
    {
        private readonly IFeatureToggleManager _featureToggleManager;

        public AlunoController(IFeatureToggleManager featureToggleManager) => _featureToggleManager = featureToggleManager;

        [HttpPost("calcular-media")]
        public async Task<IActionResult> Post(AvaliacaoDto avaliacaoDto)
        {
            if (await _featureToggleManager.IsEnabledAsync(FeatureConstants.MEDIA_APROVACAO_CINCO))
                return Ok(CalcularNota(avaliacaoDto));

            return Ok(CalcularNotaNovo(avaliacaoDto));
        }

        private string CalcularNota(AvaliacaoDto avaliacaoDto)
        {
            decimal media = avaliacaoDto.Notas.Average();
            return media >= 5m ? "O aluno foi aprovado" : "O aluno foi reprovado";
        }

        private string CalcularNotaNovo(AvaliacaoDto avaliacaoDto)
        {
            decimal media = avaliacaoDto.Notas.Average();
            return media >= 7m ? "O aluno foi aprovado" : "O aluno foi reprovado";
        }
    }
}

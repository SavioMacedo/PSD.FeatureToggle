using Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts;
using PSD.FeatureToggle.Features.CustomFilter;
using PSD.FeatureToggle.Features;
using PSD.FeatureToggle.Extensions;

namespace PSD.FeatureToggle.Entities
{
    public class Card
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Link { get; set; }
    }

    public interface ICardService
    {
        Card Get(int id);
        Task<IEnumerable<Card>> GetAllAsync();
    }

    public class CardService : ICardService
    {
        private readonly IFeatureToggleManager _featureToggleManager;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly List<Card> _cards = new() {
            new Card
            {
                Id = 1,
                Nome = "Salas Virtuais",
                Link = "www.salasvirtuais.com.br"
            },
            new Card
            {
                Id = 2,
                Nome = "Plano Semanal",
                Link = "www.planosemanal.com.br"
            },
            new Card
            {
                Id = 3,
                Nome = "Livro Digital",
                Link = "www.livrodigital.com.br"
            },
            new Card
            {
                Id = 4,
                Nome = "Avaliações",
                Link = "www.avaliacoes.com.br"
            }            
        };

        public CardService(IFeatureToggleManager featureToggleManager, IHttpContextAccessor contextAccessor) => 
            (_featureToggleManager, _contextAccessor) = (featureToggleManager, contextAccessor);

        public Card Get(int id) => _cards.FirstOrDefault(a => a.Id == id);

        public async Task<IEnumerable<Card>> GetAllAsync()
        {
            EscolaFeatureContext context = new()
            {
                EscolaId = _contextAccessor.HttpContext.User.GetEscolaId()
            };

            if (await _featureToggleManager.IsEnabledAsync(FeatureConstants.PLANAO_DISPONIVEL, "escolas", context))
            {
                _cards.Add(new Card
                {
                    Id = 5,
                    Nome = "Planão",
                    Link = "www.planao.com.br"
                });
            }

            if (await _featureToggleManager.IsEnabledByRoleAsync(FeatureConstants.VER_RESPOSTAS_AVALIACAO, _contextAccessor.HttpContext.User.GetRole()))
            {
                _cards.Add(new Card
                {
                    Id = 6,
                    Nome = "Avaliações (Respostas)",
                    Link = "www.avaliacoesrespostas.com.br"
                });
            }

            return _cards.AsEnumerable();
        }
    }
}

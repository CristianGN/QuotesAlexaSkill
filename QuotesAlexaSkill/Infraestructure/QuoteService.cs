using QuotesAlexaSkill.Core;
using QuotesAlexaSkill.Models;
using System.Linq;

namespace QuotesAlexaSkill.Infraestructure
{
       class QuoteService : IQuoteService
       {
            public string GetQuoteFromId(int id)
            {
                 return  QuoteModel.QuotesArray().FirstOrDefault(x => x.Id == id).Quote;
            }
       }
}

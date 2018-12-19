using System;
using System.Collections.Generic;
using System.Text;

namespace QuotesAlexaSkill.Core
{
    interface IQuoteService
    {
        string GetQuoteFromId(int id);
    }
}

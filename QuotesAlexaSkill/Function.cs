using System;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using QuotesAlexaSkill.Core;
using QuotesAlexaSkill.Infraestructure;
using QuotesAlexaSkill.Models;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace QuotesAlexaSkill
{
    public class Function
    {
        private ILambdaContext ctx = null;
        private SkillResponse response = null;

        private readonly IQuoteService quoteService = new QuoteService();

        #region Static String
        const string WELCOME_MESSAGE = "Bienvenido a frases de Antonio Recio!";
        const string HELP_MESSAGE = "Antonio Recio, mayorista. No limpio pescado. !Pideme un frase!";
        const string EXIT_SKILL_MESSAGE = "¡Gracias por usar frases de Antonio Recio! ¡Vuelva pronto!";
        const string UNKNOWN_MESSAGE = "Antonio Recio, mayorista. No limpio pescado. ¡No entiendo lo que me estas pidiendo!";
        const string PROBLEM_MESSAGE = "Me estoy tomando un colacao con Magdalenas, inténtalo mas tarde.";
        #endregion

        #region public
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
           ctx = context;
            try
            {
                SkillResponse response = new SkillResponse();
                response.Response = new ResponseBody();
                response.Response.ShouldEndSession = false;
                response.Version = "1.0";

                if (input.GetRequestType() == typeof(LaunchRequest))
                {
                    Log("Default LaunchRequest made");
                    response.Response.OutputSpeech = GetLaunchRequest();
                }
                else if (input.GetRequestType() == typeof(IntentRequest))
                {
                    var intentRequest = (IntentRequest)input.Request;
                    IOutputSpeech innerResponse = new PlainTextOutputSpeech();

                    switch (intentRequest.Intent.Name)
                    {
                        case "AMAZON.CancelIntent":
                        case "AMAZON.StopIntent":
                            Log($"AMAZON.CancelIntent or AMAZON.StopIntent: enviando mensaje de cancelación.");
                            (innerResponse as PlainTextOutputSpeech).Text = EXIT_SKILL_MESSAGE;
                            response.Response.ShouldEndSession = true;
                            break;
                        case "AMAZON.HelpIntent":
                            Log($"AMAZON.HelpIntent: enviando mensaje de ayuda");
                            (innerResponse as PlainTextOutputSpeech).Text = HELP_MESSAGE;
                            break;
                        case "QuoteIntent":
                            Log($"Pidiendo una nueva frase");
                            (innerResponse as PlainTextOutputSpeech).Text = GetQuote() ?? PROBLEM_MESSAGE;
                            break;
                        default:
                            Log($"Unknown intent: {intentRequest.Intent.Name}");
                            (innerResponse as PlainTextOutputSpeech).Text = UNKNOWN_MESSAGE;
                            break;
                    }
                }

                Log(JsonConvert.SerializeObject(response));

                return response;
            }
            catch (Exception ex)
            {
                Log($"error :" + ex.Message);
                response.Response = new ResponseBody();
                response.Response.ShouldEndSession = false;
                response.Version = "1.0";
            }

            (response.Response.OutputSpeech as PlainTextOutputSpeech).Text = HELP_MESSAGE;
            return response;
        }

        #endregion

        #region private
        private IOutputSpeech GetLaunchRequest()
        {
            IOutputSpeech innerResponse = new PlainTextOutputSpeech();
            (innerResponse as PlainTextOutputSpeech).Text = WELCOME_MESSAGE;

            return innerResponse;
        }


        private string GetQuote()
        {
            Random random = new Random();
            int randomNumber = random.Next(0, QuoteModel.QuotesArray().Count);

            return quoteService.GetQuoteFromId(randomNumber);
        }

        private void Log(string text)
        {
           if (ctx != null)
               ctx.Logger.LogLine(text);
        }

        #endregion
    }
}

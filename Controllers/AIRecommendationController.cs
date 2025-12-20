using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessCenterProject.Models;
using System.Text;
using System.Text.Json;

namespace FitnessCenterProject.Controllers
{
    [Authorize]
    public class AIRecommendationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIRecommendationController> _logger;
        private readonly HttpClient _httpClient;

        public AIRecommendationController(IConfiguration configuration, ILogger<AIRecommendationController> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: AIRecommendation
        public IActionResult Index()
        {
            return View(new AIRecommendationRequest());
        }

        // POST: AIRecommendation/GetRecommendation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetRecommendation(AIRecommendationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", request);
            }

            try
            {
                var response = new AIRecommendationResponse();

                // BMI hesapla
                if (request.Height.HasValue && request.Weight.HasValue)
                {
                    var heightInMeters = request.Height.Value / 100;
                    response.CalculatedBMI = request.Weight.Value / (heightInMeters * heightInMeters);
                    response.BMICategory = GetBMICategory(response.CalculatedBMI.Value);
                }

                // OpenAI API kullanarak öneri al
                var openAiApiKey = _configuration["OpenAI:ApiKey"];
                
                if (!string.IsNullOrEmpty(openAiApiKey))
                {
                    // OpenAI API ile öneri al
                    response = await GetRecommendationFromOpenAI(request, response, openAiApiKey);
                }
                else
                {
                    // OpenAI API key yoksa, basit kural tabanlı öneri sistemi kullan
                    response = GetRuleBasedRecommendation(request, response);
                }

                ViewBag.Recommendation = response;
                return View("Index", request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AI recommendation");
                ModelState.AddModelError("", "Öneri alınırken bir hata oluştu. Lütfen tekrar deneyin.");
                return View("Index", request);
            }
        }

        private async Task<AIRecommendationResponse> GetRecommendationFromOpenAI(
            AIRecommendationRequest request, 
            AIRecommendationResponse response, 
            string apiKey)
        {
            try
            {
                var prompt = BuildPrompt(request, response);

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "Sen bir fitness ve beslenme uzmanısın. Türkçe cevap ver." },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 1000,
                    temperature = 0.7
                };

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    var openAiResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);

                    if (openAiResponse?.choices != null && openAiResponse.choices.Length > 0)
                    {
                        var aiText = openAiResponse.choices[0].message.content;
                        ParseAIResponse(aiText, response);
                    }
                }
                else
                {
                    _logger.LogWarning("OpenAI API call failed: {StatusCode}", httpResponse.StatusCode);
                    // Fallback to rule-based
                    response = GetRuleBasedRecommendation(request, response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API");
                // Fallback to rule-based
                response = GetRuleBasedRecommendation(request, response);
            }

            return response;
        }

        private string BuildPrompt(AIRecommendationRequest request, AIRecommendationResponse response)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Aşağıdaki bilgilere göre egzersiz ve diyet önerisi ver:");
            sb.AppendLine();

            if (request.Height.HasValue)
                sb.AppendLine($"Boy: {request.Height} cm");
            if (request.Weight.HasValue)
                sb.AppendLine($"Kilo: {request.Weight} kg");
            if (request.Age.HasValue)
                sb.AppendLine($"Yaş: {request.Age}");
            if (!string.IsNullOrEmpty(request.Gender))
                sb.AppendLine($"Cinsiyet: {request.Gender}");
            if (!string.IsNullOrEmpty(request.Goal))
                sb.AppendLine($"Hedef: {request.Goal}");
            if (!string.IsNullOrEmpty(request.ActivityLevel))
                sb.AppendLine($"Aktivite Seviyesi: {request.ActivityLevel}");

            if (response.CalculatedBMI.HasValue)
            {
                sb.AppendLine($"BMI: {response.CalculatedBMI:F2} ({response.BMICategory})");
            }

            sb.AppendLine();
            sb.AppendLine("Lütfen şu formatta cevap ver:");
            sb.AppendLine("EGZERSİZ:");
            sb.AppendLine("[egzersiz önerileri]");
            sb.AppendLine();
            sb.AppendLine("DİYET:");
            sb.AppendLine("[diyet önerileri]");

            return sb.ToString();
        }

        private void ParseAIResponse(string aiText, AIRecommendationResponse response)
        {
            var lines = aiText.Split('\n');
            var currentSection = "";

            foreach (var line in lines)
            {
                if (line.Contains("EGZERSİZ", StringComparison.OrdinalIgnoreCase) || 
                    line.Contains("EXERCISE", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = "exercise";
                    continue;
                }
                else if (line.Contains("DİYET", StringComparison.OrdinalIgnoreCase) || 
                         line.Contains("DIET", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = "diet";
                    continue;
                }

                if (currentSection == "exercise" && !string.IsNullOrWhiteSpace(line))
                {
                    response.ExerciseRecommendation += line.Trim() + "\n";
                }
                else if (currentSection == "diet" && !string.IsNullOrWhiteSpace(line))
                {
                    response.DietRecommendation += line.Trim() + "\n";
                }
            }

            // Eğer parse edilemediyse, tüm metni analysis olarak ekle
            if (string.IsNullOrEmpty(response.ExerciseRecommendation) && 
                string.IsNullOrEmpty(response.DietRecommendation))
            {
                response.Analysis = aiText;
            }
        }

        private AIRecommendationResponse GetRuleBasedRecommendation(
            AIRecommendationRequest request, 
            AIRecommendationResponse response)
        {
            var sbExercise = new StringBuilder();
            var sbDiet = new StringBuilder();

            // BMI'ye göre öneriler
            if (response.CalculatedBMI.HasValue)
            {
                var bmi = response.CalculatedBMI.Value;

                if (bmi < 18.5m)
                {
                    sbExercise.AppendLine("• Ağırlık antrenmanları (kas kütlesi artışı için)");
                    sbExercise.AppendLine("• Protein ağırlıklı beslenme ile kombine edilmiş antrenman");
                    sbDiet.AppendLine("• Kalori fazlası ile beslenme (günde 300-500 kalori fazla)");
                    sbDiet.AppendLine("• Yüksek protein içerikli besinler (tavuk, balık, yumurta, baklagiller)");
                    sbDiet.AppendLine("• Sağlıklı karbonhidratlar (tam tahıllar, meyveler)");
                }
                else if (bmi >= 18.5m && bmi < 25m)
                {
                    sbExercise.AppendLine("• Kardiovasküler egzersizler (koşu, yürüyüş, bisiklet)");
                    sbExercise.AppendLine("• Güç antrenmanları (haftada 2-3 kez)");
                    sbExercise.AppendLine("• Esneklik egzersizleri (yoga, pilates)");
                    sbDiet.AppendLine("• Dengeli beslenme (protein, karbonhidrat, yağ dengesi)");
                    sbDiet.AppendLine("• Günde 5 porsiyon meyve-sebze");
                    sbDiet.AppendLine("• Yeterli su tüketimi (günde 2-3 litre)");
                }
                else if (bmi >= 25m && bmi < 30m)
                {
                    sbExercise.AppendLine("• Düşük etkili kardiyovasküler egzersizler (yürüyüş, yüzme)");
                    sbExercise.AppendLine("• Güç antrenmanları (metabolizma hızlandırmak için)");
                    sbExercise.AppendLine("• Haftada en az 150 dakika orta yoğunlukta egzersiz");
                    sbDiet.AppendLine("• Kalori açığı oluşturma (günde 500 kalori azaltma)");
                    sbDiet.AppendLine("• Protein ağırlıklı beslenme (tokluk hissi için)");
                    sbDiet.AppendLine("• İşlenmiş gıdalardan kaçınma");
                }
                else
                {
                    sbExercise.AppendLine("• Düşük etkili egzersizlerle başlama (yürüyüş, su aerobiği)");
                    sbExercise.AppendLine("• Yavaş yavaş yoğunluğu artırma");
                    sbExercise.AppendLine("• Profesyonel danışmanlık alınması önerilir");
                    sbDiet.AppendLine("• Sağlıklı ve sürdürülebilir kilo verme planı");
                    sbDiet.AppendLine("• Tıbbi danışmanlık alınması önerilir");
                    sbDiet.AppendLine("• Porsiyon kontrolü ve yavaş yeme");
                }
            }

            // Hedefe göre öneriler
            if (!string.IsNullOrEmpty(request.Goal))
            {
                if (request.Goal.Contains("Kilo verme", StringComparison.OrdinalIgnoreCase) || 
                    request.Goal.Contains("Zayıflama", StringComparison.OrdinalIgnoreCase))
                {
                    sbExercise.AppendLine("\nKilo Verme İçin:");
                    sbExercise.AppendLine("• HIIT antrenmanları");
                    sbExercise.AppendLine("• Yürüyüş ve koşu");
                    sbDiet.AppendLine("\nKilo Verme İçin:");
                    sbDiet.AppendLine("• Kalori açığı");
                    sbDiet.AppendLine("• Yüksek lifli besinler");
                }
                else if (request.Goal.Contains("Kas", StringComparison.OrdinalIgnoreCase) || 
                         request.Goal.Contains("Güç", StringComparison.OrdinalIgnoreCase))
                {
                    sbExercise.AppendLine("\nKas Kazanma İçin:");
                    sbExercise.AppendLine("• Ağırlık antrenmanları");
                    sbExercise.AppendLine("• Progressive overload");
                    sbDiet.AppendLine("\nKas Kazanma İçin:");
                    sbDiet.AppendLine("• Yüksek protein alımı (kg başına 1.6-2.2g)");
                    sbDiet.AppendLine("• Kalori fazlası");
                }
            }

            // Aktivite seviyesine göre
            if (!string.IsNullOrEmpty(request.ActivityLevel))
            {
                if (request.ActivityLevel.Contains("Sedanter", StringComparison.OrdinalIgnoreCase))
                {
                    sbExercise.AppendLine("\nBaşlangıç İçin:");
                    sbExercise.AppendLine("• Günde 30 dakika yürüyüş ile başlayın");
                    sbExercise.AppendLine("• Haftada 2-3 kez hafif egzersiz");
                }
            }

            response.ExerciseRecommendation = sbExercise.ToString();
            response.DietRecommendation = sbDiet.ToString();

            if (response.CalculatedBMI.HasValue)
            {
                response.Analysis = $"BMI değeriniz {response.CalculatedBMI:F2} olarak hesaplandı. " +
                                   $"Bu değer '{response.BMICategory}' kategorisindedir.";
            }

            return response;
        }

        private string GetBMICategory(decimal bmi)
        {
            if (bmi < 18.5m) return "Zayıf";
            if (bmi < 25m) return "Normal";
            if (bmi < 30m) return "Fazla Kilolu";
            return "Obez";
        }

        // OpenAI API response model
        private class OpenAIResponse
        {
            public Choice[]? choices { get; set; }
        }

        private class Choice
        {
            public Message? message { get; set; }
        }

        private class Message
        {
            public string? content { get; set; }
        }
    }
}

using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Text;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Entities;
using UrbanNoodle.Repository.Interface;
using UrbanNoodle.Services.Interface;
namespace UrbanNoodle.Services
{
    public class AIService : IAlService
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly IMemoryCache _cache;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingService;
        private readonly ILogger<AIService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IKnowledgeChunksRepository _knowledgeChunksRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ToolAlService _toolAlService;

        public AIService(Kernel kernel, IChatCompletionService chatCompletionService, IMemoryCache cache,
            IEmbeddingGenerator<string, Embedding<float>> embeddingService, ILogger<AIService> logger,
            ApplicationDbContext context, ToolAlService toolAlService,
            IKnowledgeChunksRepository knowledgeChunksRepository, IHttpContextAccessor httpContextAccessor
            )
        {
            _kernel = kernel;
            _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            _cache = cache;
            _embeddingService = embeddingService;
            _logger = logger;
            _context = context;
            _knowledgeChunksRepository = knowledgeChunksRepository;
            _httpContextAccessor = httpContextAccessor;
            _toolAlService = toolAlService;
        }

        public async Task<ApiResponse> ChatAsync(string text, int? accountId)
        {

            var chatMessage = new List<ChatMessages>();


            GeminiPromptExecutionSettings geminiSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(
                )
            };

            var json = GetHistory();
            ChatHistory? chatHistory;
            if (!string.IsNullOrEmpty(json))
            {

                chatHistory = JsonSerializer.Deserialize<ChatHistory>(json);
                UpdateAuthState(chatHistory, accountId);
            }
            else
            {

                chatHistory = new ChatHistory();
                chatHistory.AddSystemMessage(BuildSystemInstruction(accountId));

            }

            int maxMessages = 12;
            chatHistory.AddUserMessage(text);


            while (chatHistory.Count > maxMessages)
            {
                chatHistory.RemoveAt(1);
            }

            var vector = (await _embeddingService.GenerateAsync(new[] { text }))[0].Vector.ToArray();
            var contextChunks = await _knowledgeChunksRepository.SearchSimilarContextAsync(vector);
            var uniqueSentences = contextChunks
                .SelectMany(chunk => chunk.Split('.', StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();



            string contextText = string.Join(". ", uniqueSentences) + ".";


            var promptHistory = new ChatHistory();

            promptHistory.AddSystemMessage($"[NGỮ CẢNH TẠM THỜI]:\n{contextText}");

            foreach (var msg in chatHistory) { promptHistory.Add(msg); }
            _toolAlService.SetCurrentUser(accountId);


            var response = await _chatCompletionService.GetChatMessageContentAsync(
                     promptHistory,
                      executionSettings: geminiSettings,
                      kernel: _kernel
                 );


            chatHistory.AddAssistantMessage(response.Content);
            SaveHistory(JsonSerializer.Serialize(chatHistory));

            return new ApiResponse(200, response.Content);
        }







        private void SaveHistory(string json)
        {
            _httpContextAccessor.HttpContext?
                .Session
                .SetString("chat_history", json);
        }

        private string? GetHistory()
        {
            return _httpContextAccessor.HttpContext?
                .Session
                .GetString("chat_history");
        }
        private string BuildSystemInstruction(int? accountId)
        {
            string authLine = accountId.HasValue
                ? $"[AUTH]: ĐÃ ĐĂNG NHẬP | AccountId={accountId.Value}"
                : "[AUTH]: CHƯA ĐĂNG NHẬP";

            return
                $"{authLine}\n\n" +
                "# VAI TRÒ\n" +
                "Bạn là trợ lý ảo của quán ăn, hỗ trợ tư vấn món và đặt hàng.\n" +
                "Chỉ dùng thông tin từ [NGỮ CẢNH] và kết quả các hàm được cung cấp. Không tự bịa thông tin quán, món ăn, hay khách hàng.\n\n" +
                "# QUYỀN TRUY CẬP\n" +
                "- Xem menu / hỏi giá / hỏi thông tin quán: không cần đăng nhập.\n" +
                "- Đặt món, xem địa chỉ: bắt buộc đăng nhập.\n" +
                "- [AUTH] = CHƯA ĐĂNG NHẬP → yêu cầu đăng nhập, không gọi hàm nào.\n\n" +
                "# GỌI HÀM get_food\n" +
                "- PHẢI gọi hàm này bất cứ khi nào người dùng hỏi về menu, món ăn, quán có gì, giá món ăn... " +
                "- Chỉ gọi lại khi khách hỏi món/menu mà chưa có dữ liệu trong lịch sử, hoặc dữ liệu cũ không khớp món khách hỏi.\n\n" +
                "# QUY TẮC ID (quan trọng nhất — không được vi phạm)\n" +
                "- ID món/khách chỉ được lấy từ dữ liệu thật do get_food / get_customer_profile trả về, không được tạo giả id món ăn, id khách hàng, id địa chỉ của khách.\n" +
                "- Không tự đoán, tự đánh số thứ tự, hay gán ID nếu không thấy trong dữ liệu.\n" +
                "- Không tìm thấy ID món → báo khách \"chưa có thông tin, vui lòng chọn món khác hoặc đợi kiểm tra lại\", không gọi create_order.\n" +
                "- ID không được hiển thị ra tin nhắn cho khách — chỉ dùng nội bộ khi gọi hàm.\n\n" +
                "# QUY TRÌNH ĐẶT MÓN\n" +
                "1. Khách nêu món + số lượng → đối chiếu với danh sách món trong lịch sử/context để lấy đúng id. Tuyệt đối không được bịa id món ăn\n" +
                "2. Gọi get_customer_profile để lấy tên, SĐT, danh sách địa chỉ (không tự bịa các trường này).\n" +
                "3. Xử lý địa chỉ:\n" +
                "   - 1 địa chỉ → hỏi khách xác nhận dùng địa chỉ này không, hoặc nhập địa chỉ mới nếu khách từ chối.\n" +
                "   - ≥2 địa chỉ → yêu cầu khách chọn 1 địa chỉ trước, sau đó mới sang bước tóm tắt đơn.\n" +
                "   - 0 địa chỉ → yêu cầu khách nhập địa chỉ mới.\n" +
                "4. Tóm tắt đơn hàng đầy đủ: người nhận, SĐT, địa chỉ, danh sách món (tên + SL + đơn giá), tổng tiền.\n" +
                "5. Chỉ gọi create_order khi khách xác nhận rõ ràng (\"đồng ý\"/\"xác nhận\"/\"ok\").\n" +
                "6. Đặt thành công → cảm ơn khách, nhắc hotline 1900.1890 nếu cần hỗ trợ thêm.\n\n" +
                "# GIỌNG ĐIỆU & GIỚI HẠN\n" +
                "- Nhẹ nhàng, lịch sự, ngắn gọn.\n" +
                "- Không trả lời chủ đề ngoài dịch vụ quán (nếu bị hỏi, từ chối khéo và mời hỏi về menu/đặt món).\n" +
                "- Không dùng từ ngữ thô tục dù khách nói vậy.\n\n" +
                "# VÍ DỤ\n" +
                "Khách: \"cho 2 phở bò\"\n" +
                "→ Đối chiếu \"phở bò\" trong danh sách món đã có → lấy đúng id → tiếp tục bước lấy hồ sơ khách hàng.\n\n" +
                "Khách có 2 địa chỉ:\n" +
                "Bot: \"Dạ anh/chị muốn giao đến địa chỉ nào ạ: 1) ... 2) ...\"\n" +
                "(khách chọn xong) → Bot tóm tắt đơn và hỏi xác nhận.";
        }

        private void UpdateAuthState(ChatHistory chatHistory, int? accountId)
        {
            if (chatHistory == null || chatHistory.Count == 0) return;

            var systemMsg = chatHistory[0];
            if (systemMsg.Role != AuthorRole.System) return;

            var content = systemMsg.Content ?? "";

            string newAuthLine = accountId.HasValue
                ? $"[AUTH]: ĐÃ ĐĂNG NHẬP | AccountId={accountId.Value}"
                : "[AUTH]: CHƯA ĐĂNG NHẬP";

            var updated = System.Text.RegularExpressions.Regex.IsMatch(content, @"\[AUTH\]:")
                ? System.Text.RegularExpressions.Regex.Replace(content, @"\[AUTH\]:.*", newAuthLine)
                : newAuthLine + "\n\n" + content;

            chatHistory[0] = new ChatMessageContent(AuthorRole.System, updated);
        }

        public async Task<ApiResponse> Embedding()
        {
            var testText = """
1. THỜI GIAN HOẠT ĐỘNG VÀ ĐỊA CHỈ
Quán ăn mở cửa phục vụ khách hàng tất cả các ngày trong tuần, kể cả ngày lễ và Tết Nguyên Đán. 
Thời gian mở cửa hàng ngày bắt đầu từ lúc 06:00 sáng và đóng cửa vào lúc 22:00 đêm. 
Hiện tại quán có 1 cửa hàng chính đang hoạt động tại Cần Thơ. 
Quán ăn tại số 123 đường Mậu Thân, Quận Ninh Kiều. 

2. CHÍNH SÁCH GIAO HÀNG TẬN NƠI 
Để phục vụ khách hàng mua online, quán nhận giao hàng tận nơi trong khu vực trong quận Ninh Kiều. 
Ở Cái Răng nhận giao khu vực Hưng Phú, Lê Bình, Phú Thứ, gần khu vực bến xe Cần Thơ.

3. CHÍNH SÁCH ĐỔI TRẢ VÀ KHIẾU NẠI ĐỒ ĂN
Sự hài lòng của khách hàng là ưu tiên hàng đầu của chúng tôi, 
Nếu món ăn giao đến bị nhầm lẫn, thiếu sót hoặc không đảm bảo chất lượng vệ sinh an toàn thực phẩm, 
khách hàng có quyền khiếu nại trong vòng 60 phút kể từ khi nhận hàng, 
Vui lòng chụp ảnh lại phần ăn và liên hệ ngay với tổng đài chăm sóc khách hàng qua số 1900.1890 để được hỗ trợ, 
Quán cam kết sẽ hoàn tiền 100% hoặc giao lại một phần ăn mới hoàn toàn miễn phí cho khách hàng
trong trường hợp lỗi thuộc về phía nhà bếp.

4. thông tin chung của quán:
 Quán có bãi đậu xe miễn phí cho khách hàng ngay trước cửa hàng, 
đủ chỗ cho cả xe máy và ô tô. Nhân viên giữ xe sẽ hỗ trợ khách hàng khi đến quán.
quán có điều hòa, nước lạnh, rất giấy miễn phí.
""";
#pragma warning disable SKEXP0050
            var lines = TextChunker.SplitPlainTextLines(testText, maxTokensPerLine: 40);
            var chunks = TextChunker.SplitPlainTextParagraphs(
    lines,
    maxTokensPerParagraph: 100,
    overlapTokens: 15
);
            var embeddingOptions = new EmbeddingGenerationOptions
            {
                Dimensions = 768 // Ép Gemini cắt ngắn vector về 768 chiều, vẫn đảm bảo độ chính xác rất tốt
            };
            var results = await _embeddingService.GenerateAsync(chunks, embeddingOptions);
            var chunkListToSave = new List<KnowledgeChunks>();
            // Kiểm tra số lượng trước tiên — bước quan trọng nhất
            _logger.LogInformation("Số chunk: {ChunkCount}, Số embedding: {ResultCount}",
                chunks.Count, results.Count);

            if (results.Count != chunks.Count)
            {
                _logger.LogError("MISMATCH: số embedding không khớp số chunk!");
            }

            // In từng cặp chunk <-> vector để đối chiếu
            for (int i = 0; i < chunks.Count; i++)
            {
                float[] vector = results[i].Vector.ToArray();


                var newChunk = new KnowledgeChunks
                {
                    Content = chunks[i],
                    Embedding = new Pgvector.Vector(vector) // Đóng gói mảng float[] thành kiểu Vector của Postgres
                };
                chunkListToSave.Add(newChunk);
            }
            _context.KnowledgeChunks.AddRange(chunkListToSave);
            await _context.SaveChangesAsync();

            return new ApiResponse(200, "thành công");
        }

        public async Task<ApiResponse> SearchTopK(string text)
        {
            var vector = (await _embeddingService.GenerateAsync(new[] { text }))[0].Vector.ToArray();
            var contextChunks = await _knowledgeChunksRepository.SearchSimilarContextAsync(vector);
            var uniqueSentences = contextChunks
    .SelectMany(chunk => chunk.Split('.', StringSplitOptions.RemoveEmptyEntries))
    .Select(s => s.Trim())
    .Where(s => !string.IsNullOrWhiteSpace(s))
    .Distinct()
    .ToList();

            string contextText = string.Join(". ", uniqueSentences) + ".";
            return new ApiResponse(200, contextText);
        }
    }
}
